// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core.Hosting.Logging;
using Archipendium.Core.Hosting.Windowing;
using Dalamud.Interface.Windowing;
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
        var settings = new HostApplicationBuilderSettings
        {
            ApplicationName = pluginInterface.InternalName,
            ContentRootPath = pluginInterface.AssemblyLocation.DirectoryName,
            EnvironmentName = pluginInterface.IsDev ? Environments.Development : Environments.Production
        };

        _hostApplicationBuilder = Host.CreateEmptyApplicationBuilder(settings);

        ApplyDefaultAppConfiguration(_hostApplicationBuilder, _hostApplicationBuilder.Configuration, pluginInterface);
        AddDefaultServices(_hostApplicationBuilder, _hostApplicationBuilder.Services);

        _hostApplicationBuilder.Services.AddHostedService<WindowManager>();

        _hostApplicationBuilder.Services.AddSingleton(pluginInterface);
        _hostApplicationBuilder.Services.AddSingleton(pluginInterface.GetRequiredService<IPluginLog>());
        _hostApplicationBuilder.Services.AddSingleton(pluginInterface.GetRequiredService<ISeStringEvaluator>());
        _hostApplicationBuilder.Services.AddSingleton(pluginInterface.GetRequiredService<IChatGui>());
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        _hostApplicationBuilder.ConfigureContainer(factory, configure);
    }

    /// <summary>
    /// Registers all window in the assembly containing the specified type.
    /// </summary>
    /// <typeparam name="TAssembly">A type from the assembly whose Window-derived classes will be registered.</typeparam>
    public void AddWindows<TAssembly>()
    {
        var windows = typeof(TAssembly).Assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Window)));

        foreach (var window in windows)
        {
            _hostApplicationBuilder.Services.AddSingleton(window);
            _hostApplicationBuilder.Services.AddSingleton(typeof(Window), services => services.GetRequiredService(window));
        }
    }

    /// <summary>
    /// Builds and returns a configured instance of the <see cref="DalamudApplication"/> based on the current application builder settings.
    /// </summary>
    /// <returns>A <see cref="DalamudApplication"/> instance initialized with the configured application services and settings.</returns>
    public DalamudApplication Build()
    {
        return new(_hostApplicationBuilder.Build());
    }

    private static void ApplyDefaultAppConfiguration(HostApplicationBuilder builder, IConfigurationBuilder appConfigBuilder, IDalamudPluginInterface pluginInterface)
    {
        appConfigBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile(pluginInterface.ConfigFile.FullName, optional: true, reloadOnChange: true);
    }

    private static void AddDefaultServices(IHostApplicationBuilder builder, IServiceCollection services)
    {
        services.AddLogging(logging =>
        {
            logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            logging.AddDalamudLogger();

            logging.Configure(options =>
            {
                options.ActivityTrackingOptions =
                    ActivityTrackingOptions.SpanId |
                    ActivityTrackingOptions.TraceId |
                    ActivityTrackingOptions.ParentId;
            });
        });

        services.AddMetrics(metrics =>
        {
            metrics.AddConfiguration(builder.Configuration.GetSection("Metrics"));
        });
    }
}
