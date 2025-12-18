// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core.Services;
using Archipendium.Windows;
using Dalamud.Hosting.Commands;

namespace Archipendium.Commands;

/// <summary>
/// Represents the root command that opens the user interface and optionally sends a message if connected.
/// </summary>
/// <param name="mainWindow">The main window of the application.</param>
/// <param name="archipelagoService">The Archipelago service for handling connections.</param>
public class RootCommand(MainWindow mainWindow, ArchipelagoService archipelagoService) : Command("/ap", "Opens the UI. Additional text will send a message if connected.")
{
    /// <inheritdoc/>
    public override void OnExecute(string command, string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            mainWindow.Toggle();
        }
        else
        {
            archipelagoService.SendChatMessage(args);
        }
    }
}
