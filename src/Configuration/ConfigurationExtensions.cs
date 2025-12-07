// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Archipendium.Configuration;

/// <summary>
/// Provides extension methods for configuring configuration in Archipendium.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Configures configurationfor the application builder.
    /// </summary>
    /// <param name="builder">The application builder to configure. Cannot be null.</param>
    /// <returns>The same instance of <paramref name="builder"/> with configuration configured.</returns>
    public static DalamudApplicationBuilder ConfigureConfiguration(this DalamudApplicationBuilder builder)
    {
        return builder.ConfigureAppConfiguration();
    }

    private static DalamudApplicationBuilder ConfigureAppConfiguration(this DalamudApplicationBuilder builder)
    {
        builder.Services.Configure<AppConfiguration>(builder.Configuration);
        builder.Services.AddSingleton<IOptionsMonitor<AppConfiguration>, OptionsMonitor<AppConfiguration>>();

        return builder;
    }
}
