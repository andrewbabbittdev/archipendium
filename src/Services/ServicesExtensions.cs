// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Questing.Services;
using Dalamud.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Archipendium.Services;

/// <summary>
/// Provides extension methods for configuring Archipendium services in a Dalamud application builder.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Configures the application builder to support Archipendium services, including Archipelago and questing-specific services.
    /// </summary>
    /// <param name="builder">The application builder to configure. Must not be null.</param>
    /// <returns>The same <see cref="DalamudApplicationBuilder"/> instance so that additional configuration calls can be chained.</returns>
    public static DalamudApplicationBuilder ConfigureArchipendiumServices(this DalamudApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ArchipelagoService>();
        builder.Services.AddHostedService<QuestService>();

        return builder;
    }
}
