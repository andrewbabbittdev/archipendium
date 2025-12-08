// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Archipendium.Questing.Config;

/// <summary>
/// Represents an item used in a quest, including its name and associated token multiplier.
/// </summary>
public class QuestItem
{
    /// <summary>
    /// The item name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The token multiplier.
    /// </summary>
    public required decimal Multiplier { get; set; }
}
