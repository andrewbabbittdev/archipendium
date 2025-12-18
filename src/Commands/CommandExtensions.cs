// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Dalamud.Hosting;

namespace Archipendium.Commands;

/// <summary>
/// Provides extension methods for configuring Archipendium commands in a Dalamud application builder.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Configures the application builder to support Archipendium commands.
    /// </summary>
    /// <param name="builder">The application builder to configure. Must not be null.</param>
    /// <returns>The same <see cref="DalamudApplicationBuilder"/> instance so that additional configuration calls can be chained.</returns>
    public static DalamudApplicationBuilder ConfigureArchipendiumCommands(this DalamudApplicationBuilder builder)
    {
        return builder.AddCommands<RootCommand>();
    }
}
