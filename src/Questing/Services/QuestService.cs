// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core.Services;
using Archipendium.Questing.Config;
using Archipendium.Questing.Models;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Archipendium.Questing.Services;

/// <summary>
/// Provides background quest tracking and chat message parsing services for the application.
/// </summary>
/// <param name="hostEnvironment">The host environment in which the application is running.</param>
/// <param name="config">The configuration options for questing features.</param>
/// <param name="archipelagoService">The Archipelago service used for item and quest management.</param>
/// <param name="chatGui">The chat interface used to subscribe to and handle chat messages relevant to quest tracking.</param>
public partial class QuestService(IHostEnvironment hostEnvironment, IOptionsMonitor<QuestingConfig> config, ArchipelagoService archipelagoService, IChatGui chatGui) : IHostedService
{
    private readonly int[] _acceptedChatTypes =
    [
        2110, // General Items
        2238 // MGP
    ];

    [GeneratedRegex("[^\u0000-\u007F]+")]
    private static partial Regex FilterRegex();

    [GeneratedRegex("You obtain a? ?([0-9,]+)? ?([A-Za-z0-9 '\\(\\)]+)\\.$")]
    private static partial Regex ParseRegex();

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        chatGui.ChatMessage += OnChatMessage;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        chatGui.ChatMessage -= OnChatMessage;

        return Task.CompletedTask;
    }

    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (archipelagoService.IsConnected && message.TextValue.StartsWith("You obtain"))
        {
            if (_acceptedChatTypes.Contains((int)type))
            {
                var sanitizedMessage = FilterRegex().Replace(message.TextValue, "");
                var match = ParseRegex().Match(sanitizedMessage);

                if (match.Success)
                {
                    var entity = new QuestEntity()
                    {
                        Name = match.Groups[2].Value,
                        Count = string.IsNullOrEmpty(match.Groups[1].Value) ? 1 : int.Parse(match.Groups[1].Value.Replace(",", ""))
                    };

                    if (entity.Name != "archipelago tokens")
                    {
                        ProcessQuestEntity(entity);
                    }
                }
                else
                {
                    if (hostEnvironment.IsDevelopment())
                    {
                        chatGui.PrintError($"[Obtained Item Match Fail]: {sanitizedMessage}", "Archipendium");
                    }
                }
            }
            else
            {
                if (hostEnvironment.IsDevelopment())
                {
                    chatGui.PrintError($"[Obtained Item Message Type Fail]: {(int)type}", "Archipendium");
                }
            }
        }
    }

    private void ProcessQuestEntity(QuestEntity entity)
    {
        var questConfig = config.CurrentValue.Items
            .FirstOrDefault(i => i.Name == entity.Name);

        if (questConfig is not null)
        {
            var tokens = (int)Math.Round(entity.Count * questConfig.Multiplier);

            archipelagoService.DepositTokens(tokens);

            var message = new SeStringBuilder()
                .Append($"You obtain {tokens:N0} archipelago tokens.")
                .Build();

            chatGui.Print(new XivChatEntry()
            {
                Type = (XivChatType)2238,
                Message = message
            });
        }
    }
}
