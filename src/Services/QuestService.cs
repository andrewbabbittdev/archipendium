// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Configuration;
using Archipendium.Services;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

using QuestEntity = (string Name, int Count);

namespace Archipendium.Questing.Services;

/// <summary>
/// Provides background quest tracking and chat message parsing services for the application.
/// </summary>
public partial class QuestService : IHostedService, IDisposable
{
    private readonly IOptionsMonitor<QuestingConfig> _config;
    private readonly ArchipelagoService _archipelagoService;
    private readonly IChatGui _chatGui;
    private readonly System.Timers.Timer _transactionTimer;

    private readonly int[] _acceptedChatTypes =
    [
        2110, // General Items
        2238 // MGP
    ];

    private int _transactionTokens;

    /// <summary>
    /// Initializes a new instance of the QuestService class.
    /// </summary>
    /// <param name="config">The configuration options for questing features.</param>
    /// <param name="archipelagoService">The Archipelago service used for item and quest management.</param>
    /// <param name="chatGui">The chat interface used to subscribe to and handle chat messages relevant to quest tracking.</param>
    public QuestService(IOptionsMonitor<QuestingConfig> config, ArchipelagoService archipelagoService, IChatGui chatGui)
    {
        _config = config;
        _archipelagoService = archipelagoService;
        _chatGui = chatGui;

        _transactionTimer = new(TimeSpan.FromMilliseconds(500))
        {
            Enabled = false,
            AutoReset = false
        };

        _chatGui.ChatMessage += OnChatMessage;
        _transactionTimer.Elapsed += TransactionTimerElapsed;
    }

    [GeneratedRegex("[^\u0000-\u007F]+")]
    private static partial Regex FilterRegex();

    [GeneratedRegex("You obtain a? ?([0-9,]+)? ?([A-Za-z0-9 '\\(\\)]+)\\.$")]
    private static partial Regex ParseRegex();

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _chatGui.ChatMessage -= OnChatMessage;

        _transactionTimer.Elapsed -= TransactionTimerElapsed;
        _transactionTimer.Enabled = false;
        _transactionTimer.Dispose();

        GC.SuppressFinalize(this);
    }

    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!message.TextValue.StartsWith("You obtain"))
        {
            return;
        }

        if (!_acceptedChatTypes.Contains((int)type))
        {
            _chatGui.PrintError($"[Obtained Item Message Type Fail]: {(int)type}", "Archipendium");
            return;
        }

        var sanitizedMessage = FilterRegex().Replace(message.TextValue, "");
        var match = ParseRegex().Match(sanitizedMessage);

        if (!match.Success)
        {
            _chatGui.PrintError($"[Obtained Item Match Fail]: {sanitizedMessage}", "Archipendium");
            return;
        }

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

    private void ProcessQuestEntity(QuestEntity entity)
    {
        if (!_archipelagoService.IsConnected)
        {
            return;
        }

        var questConfig = _config.CurrentValue.Items
            .FirstOrDefault(i => i.Name.Equals(entity.Name, StringComparison.OrdinalIgnoreCase));

        if (questConfig is null)
        {
            return;
        }

        _transactionTokens += (int)Math.Round(entity.Count * questConfig.Multiplier);

        if (!_transactionTimer.Enabled)
        {
            _transactionTimer.Start();
        }
    }

    private void TransactionTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _archipelagoService.DepositTokens(_transactionTokens);

        var message = new SeStringBuilder()
            .Append($"You obtain {_transactionTokens:N0} archipelago tokens.")
            .Build();

        _chatGui.Print(new XivChatEntry()
        {
            Type = (XivChatType)2238,
            Message = message
        });

        _transactionTokens = 0;
    }
}
