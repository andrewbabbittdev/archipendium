// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Archipendium.Configuration;

/// <summary>
/// The questing configuration.
/// </summary>
public class QuestingConfig
{
    /// <summary>
    /// The list of chat types that are supported.
    /// </summary>
    public List<int> ChatTypes { get; set; } = [];

    /// <summary>
    /// The list of quest items being tracked.
    /// </summary>
    public List<QuestItem> Items { get; set; } = [];
}
