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
    private readonly IOptions<Configuration> _config;

    /// <summary>
    /// Initializes a new instance of the ConfigWindow.
    /// </summary>
    public ConfigWindow(IOptions<Configuration> config) : base("Archipendium Config")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        Size = new Vector2(266, 160);
        SizeCondition = ImGuiCond.Always;

        _config = config;
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        var displayChatMessages = _config.Value.DisplayChatMessages;
        if (ImGui.Checkbox("Display chat messages", ref displayChatMessages))
        {
            _config.Value.DisplayChatMessages = displayChatMessages;
            _config.Value.Save();
        }

        var displayFoundHintMessages = _config.Value.DisplayFoundHintMessages;
        if (ImGui.Checkbox("Display hints on found items", ref displayFoundHintMessages))
        {
            _config.Value.DisplayFoundHintMessages = displayFoundHintMessages;
            _config.Value.Save();
        }

        var displayJoinLeaveMessages = _config.Value.DisplayJoinLeaveMessages;
        if (ImGui.Checkbox("Display join / leave notifications", ref displayJoinLeaveMessages))
        {
            _config.Value.DisplayJoinLeaveMessages = displayJoinLeaveMessages;
            _config.Value.Save();
        }

        var displayItemSentMessages = _config.Value.DisplayItemSentMessages;
        if (ImGui.Checkbox("Display sent items notifications", ref displayItemSentMessages))
        {
            _config.Value.DisplayItemSentMessages = displayItemSentMessages;
            _config.Value.Save();
        }

        var displayItemReceivedMessages = _config.Value.DisplayItemReceivedMessages;
        if (ImGui.Checkbox("Display received items notifications", ref displayItemReceivedMessages))
        {
            _config.Value.DisplayItemReceivedMessages = displayItemReceivedMessages;
            _config.Value.Save();
        }
    }
}
