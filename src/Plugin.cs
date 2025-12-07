// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core.Hosting;
using Dalamud.Plugin;

namespace Archipendium;

/// <summary>
/// Represents a Dalamud plugin that manages its application lifecycle and integrates with the Dalamud plugin
/// environment.
/// </summary>
public class Plugin : IDalamudPlugin
{
    private readonly DalamudApplication _app;

    /// <summary>
    /// Initializes a new instance of the Plugin class using the specified Dalamud plugin interface.
    /// </summary>
    /// <param name="pluginInterface">The interface provided by Dalamud for interacting with the plugin environment. Cannot be null.</param>
    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        var builder = DalamudApplication.CreateBuilder(pluginInterface);

        _app = builder.Build();

        _app.Start();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _app.Stop();

        GC.SuppressFinalize(this);
    }
}
