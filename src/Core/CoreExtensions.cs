// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Dalamud.Hosting;

namespace Archipendium.Core;

/// <summary>
/// Provides extension methods for configuring core services in Archipendium.
/// </summary>
public static class CoreExtensions
{
    /// <summary>
    /// Configures core services for the application builder.
    /// </summary>
    /// <param name="builder">The application builder to configure. Cannot be null.</param>
    /// <returns>The same instance of <paramref name="builder"/> with core services configured.</returns>
    public static DalamudApplicationBuilder ConfigureCore(this DalamudApplicationBuilder builder)
    {
        return builder.ConfigureWindowing()
            .ConfigureCommands();
    }

    private static DalamudApplicationBuilder ConfigureWindowing(this DalamudApplicationBuilder builder)
    {
        builder.AddWindows<Plugin>();

        return builder;
    }

    private static DalamudApplicationBuilder ConfigureCommands(this DalamudApplicationBuilder builder)
    {
        builder.AddCommands<Plugin>();

        return builder;
    }
}
