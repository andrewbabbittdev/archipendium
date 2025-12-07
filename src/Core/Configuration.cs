// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Dalamud.Configuration;
using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace Archipendium.Core;

/// <summary>
/// Represents the configuration settings for the plugin state.
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration
{
    /// <summary>
    /// Gets or sets the version number associated with the current configuration.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the network address and port of the Archipelago server to connect to.
    /// </summary>
    public string Host { get; set; } = "archipelago.gg:12345";

    /// <summary>
    /// Gets or sets the name of the Archipelago slot.
    /// </summary>
    public string Slot { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether chat messages are displayed to the user.
    /// </summary>
    public bool DisplayChatMessages { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether found hint messages should be displayed to the user.
    /// </summary>
    public bool DisplayFoundHintMessages { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether join and leave messages are displayed to the user.
    /// </summary>
    public bool DisplayJoinLeaveMessages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether item received messages should be displayed to the user.
    /// </summary>
    public bool DisplayItemReceivedMessages { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether item sent messages should be displayed to the user.
    /// </summary>
    public bool DisplayItemSentMessages { get; set; }

    /// <summary>
    /// Saves the current configuration to persistent storage.
    /// </summary>
    public void Save()
    {
        Plugin.App.Services.GetRequiredService<IDalamudPluginInterface>()
            .SavePluginConfig(this);
    }
}
