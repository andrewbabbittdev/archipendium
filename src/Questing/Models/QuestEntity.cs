// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Archipendium.Questing.Models;

/// <summary>
/// Represents a quest entity.
/// </summary>
public class QuestEntity
{
    /// <summary>
    /// The entity name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The number of instances of this entity.
    /// </summary>
    public required int Count { get; set; }
}
