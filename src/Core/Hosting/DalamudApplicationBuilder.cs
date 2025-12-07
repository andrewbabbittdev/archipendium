// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archipendium.Core.Hosting;

/// <summary>
/// Provides a builder for configuring and constructing a Dalamud host application, including services, configuration, logging, and environment settings.
/// </summary>
public sealed class DalamudApplicationBuilder : IHostApplicationBuilder
{
    private readonly HostApplicationBuilder _hostApplicationBuilder;

    /// <inheritdoc/>
    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_hostApplicationBuilder).Properties;

    /// <inheritdoc/>
    public IConfigurationManager Configuration => _hostApplicationBuilder.Configuration;

    /// <inheritdoc/>
    public IHostEnvironment Environment => _hostApplicationBuilder.Environment;

    /// <inheritdoc/>
    public ILoggingBuilder Logging => _hostApplicationBuilder.Logging;

    /// <inheritdoc/>
    public IMetricsBuilder Metrics => _hostApplicationBuilder.Metrics;

    /// <inheritdoc/>
    public IServiceCollection Services => _hostApplicationBuilder.Services;

    internal DalamudApplicationBuilder(IDalamudPluginInterface pluginInterface)
    {
        _hostApplicationBuilder = Host.CreateEmptyApplicationBuilder(new()
        {
            ApplicationName = pluginInterface.InternalName,
            ContentRootPath = pluginInterface.AssemblyLocation.DirectoryName,
            EnvironmentName = pluginInterface.IsDev ? Environments.Development : Environments.Production
        });

        _hostApplicationBuilder.Services.AddSingleton(pluginInterface.GetRequiredService<IChatGui>());
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        _hostApplicationBuilder.ConfigureContainer(factory, configure);
    }

    /// <summary>
    /// Builds and returns a configured instance of the <see cref="DalamudApplication"/> based on the current application builder settings.
    /// </summary>
    /// <returns>A <see cref="DalamudApplication"/> instance initialized with the configured application services and settings.</returns>
    public DalamudApplication Build()
    {
        return new(_hostApplicationBuilder.Build());
    }
}
