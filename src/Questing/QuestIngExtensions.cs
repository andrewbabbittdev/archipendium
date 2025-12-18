// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Questing.Config;
using Archipendium.Questing.Services;
using Dalamud.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Archipendium.Questing;

/// <summary>
/// Provides extension methods for configuring questing services in Archipendium.
/// </summary>
public static class QuestingExtensions
{
    /// <summary>
    /// Configures questing services for the application builder.
    /// </summary>
    /// <param name="builder">The application builder to configure. Cannot be null.</param>
    /// <returns>The same instance of <paramref name="builder"/> with core services configured.</returns>
    public static DalamudApplicationBuilder ConfigureQuesting(this DalamudApplicationBuilder builder)
    {
        return builder.ConfigureQuests()
            .ConfigureQuestingConfig();
    }

    private static DalamudApplicationBuilder ConfigureQuests(this DalamudApplicationBuilder builder)
    {
        builder.Services.AddHostedService<QuestService>();

        return builder;
    }

    private static DalamudApplicationBuilder ConfigureQuestingConfig(this DalamudApplicationBuilder builder)
    {
        builder.Services.Configure<QuestingConfig>(builder.Configuration.GetSection("Questing"));
        builder.Services.AddSingleton<IOptionsMonitor<QuestingConfig>, OptionsMonitor<QuestingConfig>>();

        return builder;
    }
}
