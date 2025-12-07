// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace Archipendium.Windows;

/// <summary>
/// Represents a window that displays config-related information and controls within the plugin.
/// </summary>
public class ConfigWindow : Window
{
    private readonly IOptionsMonitor<Configuration> _config;

    /// <summary>
    /// Initializes a new instance of the ConfigWindow.
    /// </summary>
    /// <param name="config">The configuration options.</param>
    public ConfigWindow(IOptionsMonitor<Configuration> config) : base("Archipendium Config")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        Size = new Vector2(266, 160);
        SizeCondition = ImGuiCond.Always;

        _config = config;
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        var displayChatMessages = _config.CurrentValue.DisplayChatMessages;
        if (ImGui.Checkbox("Display chat messages", ref displayChatMessages))
        {
            _config.CurrentValue.DisplayChatMessages = displayChatMessages;
            _config.CurrentValue.Save();
        }

        var displayFoundHintMessages = _config.CurrentValue.DisplayFoundHintMessages;
        if (ImGui.Checkbox("Display hints on found items", ref displayFoundHintMessages))
        {
            _config.CurrentValue.DisplayFoundHintMessages = displayFoundHintMessages;
            _config.CurrentValue.Save();
        }

        var displayJoinLeaveMessages = _config.CurrentValue.DisplayJoinLeaveMessages;
        if (ImGui.Checkbox("Display join / leave notifications", ref displayJoinLeaveMessages))
        {
            _config.CurrentValue.DisplayJoinLeaveMessages = displayJoinLeaveMessages;
            _config.CurrentValue.Save();
        }

        var displayItemSentMessages = _config.CurrentValue.DisplayItemSentMessages;
        if (ImGui.Checkbox("Display sent items notifications", ref displayItemSentMessages))
        {
            _config.CurrentValue.DisplayItemSentMessages = displayItemSentMessages;
            _config.CurrentValue.Save();
        }

        var displayItemReceivedMessages = _config.CurrentValue.DisplayItemReceivedMessages;
        if (ImGui.Checkbox("Display received items notifications", ref displayItemReceivedMessages))
        {
            _config.CurrentValue.DisplayItemReceivedMessages = displayItemReceivedMessages;
            _config.CurrentValue.Save();
        }
    }
}
