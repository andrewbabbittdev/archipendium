// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Dalamud.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Archipendium.Configuration;

/// <summary>
/// Provides extension methods for configuring Archipendium configuration options in a Dalamud application builder.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Configures the application builder to support Archipendium configuration options, including general and questing-specific settings.
    /// </summary>
    /// <param name="builder">The application builder to configure. Must not be null.</param>
    /// <returns>The same <see cref="DalamudApplicationBuilder"/> instance so that additional configuration calls can be chained.</returns>
    public static DalamudApplicationBuilder ConfigureArchipendiumConfiguration(this DalamudApplicationBuilder builder)
    {
        builder.Services.Configure<MainConfig>(builder.Configuration);
        builder.Services.AddSingleton<IOptionsMonitor<MainConfig>, OptionsMonitor<MainConfig>>();

        builder.Services.Configure<QuestingConfig>(builder.Configuration.GetSection("Questing"));
        builder.Services.AddSingleton<IOptionsMonitor<QuestingConfig>, OptionsMonitor<QuestingConfig>>();

        return builder;
    }
}
