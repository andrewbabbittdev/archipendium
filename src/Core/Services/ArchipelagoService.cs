// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Dalamud.Plugin.Services;
using Lumina.Text;
using Microsoft.Extensions.Options;

namespace Archipendium.Core.Services;

/// <summary>
/// Provides functionality for connecting to and interacting with an Archipelago multiworld server, including managing connection state and hint points for the user.
/// </summary>
/// <param name="config">The configuration options.</param>
/// <param name="chatGui">The chat interface used to display messages and interact with users.</param>
/// <param name="seStringEvaluator">The evaluator for processing SeString formatted messages.</param>
public class ArchipelagoService(IOptionsMonitor<Configuration> config, IChatGui chatGui, ISeStringEvaluator seStringEvaluator) : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the user is currently connected.
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Gets the number of tokens currently available to the user.
    /// </summary>
    public int Tokens { get; private set; }

    /// <summary>
    /// Represents the number of tokens required to obtain a single hint.
    /// </summary>
    public const int TokensPerHint = 1000;

    private ArchipelagoSession? _client;
    private List<long> _knownHints = [];

    /// <summary>
    /// Attempts to establish a connection to an Archipelago server using the specified host, slot, and optional
    /// password.
    /// </summary>
    /// <param name="host">The address or hostname of the Archipelago server to connect to. Cannot be null or empty.</param>
    /// <param name="slot">The slot name to use for login. Represents the player or entity identity on the server. Cannot be null or empty.</param>
    /// <param name="password">The password required for authentication, if the server or slot is password-protected; otherwise, null.</param>
    public void Connect(string host, string slot, string? password)
    {
        _client = ArchipelagoSessionFactory.CreateSession(host);

        LoginResult? result;

        try
        {
            result = _client.TryConnectAndLogin(
                game: string.Empty,
                name: slot,
                itemsHandlingFlags: ItemsHandlingFlags.IncludeOwnItems,
                version: new(0, 6, 4),
                tags: ["AP", "FFXIV", "HintGenerator", "TextOnly"],
                uuid: null,
                password: password,
                requestSlotData: true);
        }
        catch (Exception ex)
        {
            result = new LoginFailure(ex.GetBaseException().Message);
        }

        if (result.Successful)
        {
            _client.DataStorage[Scope.Slot, "ArchipendiumTokens"].Initialize(0);
            Tokens = _client.DataStorage[Scope.Slot, "ArchipendiumTokens"].To<int>();

            _client.Socket.ErrorReceived += OnSocketErrorReceived;
            _client.MessageLog.OnMessageReceived += OnMessageReceived;
            _client.Hints.TrackHints(OnHintsUpdated, true);

            IsConnected = true;

            chatGui.Print($"Connected to Archipelago session at {host} as {slot}.", "Archipelago");
        }
        else
        {
            var failure = (LoginFailure)result;
            var errorMessage = string.Empty;

            foreach (var error in failure.Errors)
            {
                errorMessage += "\n" + error;
            }

            foreach (var error in failure.ErrorCodes)
            {
                errorMessage += $"\n{error}";
            }

            errorMessage = errorMessage.Trim('\n');

            chatGui.PrintError(errorMessage, "Archipelago");

            _client = null;
        }
    }

    /// <summary>
    /// Disconnects the client from the remote server and releases associated resources.
    /// </summary>
    public void Disconnect()
    {
        if (_client is not null)
        {
            _client.Socket.ErrorReceived -= OnSocketErrorReceived;
            _client.MessageLog.OnMessageReceived -= OnMessageReceived;

            try
            {
                _client.Socket.DisconnectAsync().GetAwaiter().GetResult();
            }
            catch { }

            _client = null;

            IsConnected = false;
        }
    }

    /// <summary>
    /// Sends a chat message.
    /// </summary>
    /// <param name="message">The text of the message to send. Cannot be null or empty.</param>
    public void SendChatMessage(string message)
    {
        if (_client is null)
        {
            chatGui.PrintError("You are not connected to a server.", "Archipelago");
        }
        else
        {
            _client.Say(message);
        }
    }

    /// <summary>
    /// Adds the specified number of tokens to the current balance.
    /// </summary>
    /// <param name="amount">The number of tokens to deposit. Must be a non-negative integer.</param>
    public void DepositTokens(int amount)
    {
        if (_client is not null)
        {
            Tokens += amount;
            _client.DataStorage[Scope.Slot, "ArchipendiumTokens"] = Tokens;
        }
    }

    /// <summary>
    /// Requests the purchase of a hint for a randomly selected missing location that has not yet been hinted.
    /// </summary>
    public void PurchaseHint()
    {
        if (_client is not null)
        {
            var missingLocations = _client.Locations.AllMissingLocations
                .Where(l => !_knownHints.Contains(l))
                .ToList();

            if (missingLocations.Count > 0)
            {
                var index = Random.Shared.Next(missingLocations.Count);
                var locationId = missingLocations[index];

                _client.Socket.SendPacket(new LocationScoutsPacket()
                {
                    Locations = [locationId],
                    CreateAsHint = 1
                });
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Disconnect();

        GC.SuppressFinalize(this);
    }

    private void OnSocketErrorReceived(Exception e, string message)
    {
        Task.Run(Disconnect);

        chatGui.PrintError("Connection lost, please sign back in.", "Archipelago");
    }

    private void OnMessageReceived(LogMessage message)
    {
        if (_client is null)
        {
            return;
        }

        var messageBuilder = new SeStringBuilder();

        foreach (var part in message.Parts)
        {
            messageBuilder.PushColorRgba(part.Color.R, part.Color.G, part.Color.B, 1)
                .Append(part.Text)
                .PopColor();
        }

        if ((message is ChatLogMessage || message is ServerChatLogMessage) && !config.CurrentValue.DisplayChatMessages)
        {
            return;
        }

        if (message is HintItemSendLogMessage && !config.CurrentValue.DisplayFoundHintMessages)
        {
            var messageString = messageBuilder.ToString() ?? string.Empty;

            if (messageString.EndsWith("(found)"))
            {
                return;
            }
        }

        if ((message is JoinLogMessage || message is LeaveLogMessage) && !config.CurrentValue.DisplayJoinLeaveMessages)
        {
            return;
        }

        if (message is ItemSendLogMessage itemMessage)
        {
            if (itemMessage.Sender.Name == _client.Players.ActivePlayer.Name)
            {
                if (!config.CurrentValue.DisplayItemSentMessages)
                {
                    return;
                }
            }
            else if (itemMessage.Receiver.Name == _client.Players.ActivePlayer.Name)
            {
                if (!config.CurrentValue.DisplayItemReceivedMessages)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        var result = seStringEvaluator.Evaluate(messageBuilder.ToReadOnlySeString());

        chatGui.Print(result, "Archipelago");
    }

    private void OnHintsUpdated(Hint[] hints)
    {
        if (_client is not null)
        {
            _knownHints = [.. hints.Where(h => h.FindingPlayer == _client.ConnectionInfo.Slot).Select(h => h.LocationId)];
        }
    }
}
