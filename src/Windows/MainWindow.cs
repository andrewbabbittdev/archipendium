// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core;
using Archipendium.Core.Services;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace Archipendium.Windows;

/// <summary>
/// Represents a window that displays connection-related information and controls within the plugin.
/// </summary>
public class MainWindow : Window
{
    private readonly IOptions<Configuration> _config;
    private readonly ArchipelagoService _archipelago;

    private string _password = string.Empty;
    private bool _isConnecting;

    /// <summary>
    /// Initializes a new instance of the MainWindow.
    /// </summary>
    /// <param name="config">The configuration options.</param>
    /// <param name="archipelago">The Archipelago service.</param>
    public MainWindow(IOptions<Configuration> config, ArchipelagoService archipelago) : base("Archipendium Connect")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        Size = new Vector2(272, 250);
        SizeCondition = ImGuiCond.Always;

        _config = config;
        _archipelago = archipelago;
    }

    /// <inheritdoc/>
    public override bool DrawConditions()
    {
        return !_archipelago.IsConnected;
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        ImGui.NewLine();
        ImGui.Indent(40);

        var host = _config.Value.Host;
        ImGui.Text("Archipelago host:");
        if (ImGui.InputText("##Archipelago host:", ref host))
        {
            _config.Value.Host = host;
        }

        var slot = _config.Value.Slot;
        ImGui.Text("Slot name:");
        if (ImGui.InputText("##Slot name:", ref slot))
        {
            _config.Value.Slot = slot;
        }

        ImGui.Text("Password (optional):");
        ImGui.InputText("##Password (optional):", ref _password, flags: ImGuiInputTextFlags.Password);

        if (_isConnecting)
        {
            ImGui.BeginDisabled();
        }

        if (ImGui.Button(_isConnecting ? "Connecting" : "Connect"))
        {
            _config.Value.Save();
            _isConnecting = true;

            Task.Run(() =>
            {
                _archipelago.Connect(host, slot, _password);
                _isConnecting = false;
            });
        }

        if (_isConnecting)
        {
            ImGui.EndDisabled();
        }

        ImGui.Unindent(40);
    }
}
