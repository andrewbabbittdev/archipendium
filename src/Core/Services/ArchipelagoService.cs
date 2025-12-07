// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Dalamud.Plugin.Services;

namespace Archipendium.Core.Services;

/// <summary>
/// Provides functionality for connecting to and interacting with an Archipelago multiworld server, including managing connection state and hint points for the user.
/// </summary>
/// <param name="chatGui">The chat interface used to display messages and interact with users.</param>
public class ArchipelagoService(IChatGui chatGui) : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the user is currently connected.
    /// </summary>
    public bool IsConnected => _client is not null && _client.Socket.Connected;

    /// <summary>
    /// Gets the number of credits currently available to the user.
    /// </summary>
    public int Credits { get; private set; }

    private ArchipelagoSession? _client;

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
            _client.DataStorage[Scope.Slot, "ArchipendiumCredits"].Initialize(0);
            Credits = _client.DataStorage[Scope.Slot, "ArchipendiumCredits"].To<int>();

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
            try
            {
                _client.Socket.DisconnectAsync().GetAwaiter().GetResult();
            }
            catch { }

            _client = null;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Disconnect();

        GC.SuppressFinalize(this);
    }
}
