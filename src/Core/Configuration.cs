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
    public string ArchipelagoHost { get; set; } = "archipelago.gg:12345";

    /// <summary>
    /// Gets or sets the name of the Archipelago slot.
    /// </summary>
    public string ArchipelagoSlotName { get; set; } = string.Empty;

    /// <summary>
    /// Saves the current configuration to persistent storage.
    /// </summary>
    public void Save()
    {
        Plugin.App.Services.GetRequiredService<IDalamudPluginInterface>()
            .SavePluginConfig(this);
    }
}
