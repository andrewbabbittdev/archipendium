// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        return builder.ConfigureConfiguration();
    }

    private static DalamudApplicationBuilder ConfigureConfiguration(this DalamudApplicationBuilder builder)
    {
        builder.Services.Configure<Configuration>(builder.Configuration);
        builder.Services.AddSingleton<IOptionsMonitor<Configuration>, OptionsMonitor<Configuration>>();

        return builder;
    }
}
