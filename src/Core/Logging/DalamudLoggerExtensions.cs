// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Archipendium.Core.Logging;

/// <summary>
/// Provides extension methods for configuring logging to use the Dalamud logging infrastructure within an plugin's logging pipeline.
/// </summary>
public static class DalamudLoggerExtensions
{
    /// <summary>
    /// Adds a logger provider that writes log messages using the Dalamud logging infrastructure.
    /// </summary>
    /// <param name="builder">The logging builder to configure with the Dalamud logger provider. Cannot be null.</param>
    /// <returns>The same instance of <see cref="ILoggingBuilder"/> for chaining.</returns>
    public static ILoggingBuilder AddDalamudLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DalamudLoggerProvider>());

        return builder;
    }
}
