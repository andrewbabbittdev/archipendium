// Licensed to the Archipendium Contributors under one or more agreements.
// The Archipendium Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Archipendium.Core;
using Archipendium.Core.Services;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;
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
    private bool _isDisconnecting;

    /// <summary>
    /// Initializes a new instance of the MainWindow.
    /// </summary>
    /// <param name="config">The configuration options.</param>
    /// <param name="archipelago">The Archipelago service.</param>
    public MainWindow(IOptions<Configuration> config, ArchipelagoService archipelago) : base("Archipendium")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        Size = new Vector2(272, 250);
        SizeCondition = ImGuiCond.Always;

        _config = config;
        _archipelago = archipelago;
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        ImGui.NewLine();
        ImGui.Indent(40);

        if (!_archipelago.IsConnected)
        {
            Size = new Vector2(272, 250);
            RenderConnectPage();
        }
        else
        {
            Size = new Vector2(272, 202);
            RenderSessionPage();
        }

        ImGui.Unindent(40);
    }

    private void RenderConnectPage()
    {
        var host = _config.Value.Host;
        ImGui.Text("Archipelago Host:");
        if (ImGui.InputText("##Archipelago Host:", ref host))
        {
            _config.Value.Host = host;
        }

        var slot = _config.Value.Slot;
        ImGui.Text("Slot Name:");
        if (ImGui.InputText("##Slot Name:", ref slot))
        {
            _config.Value.Slot = slot;
        }

        ImGui.Text("Password:");
        ImGui.InputText("##Password:", ref _password, flags: ImGuiInputTextFlags.Password);

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
    }

    private void RenderSessionPage()
    {
        var credits = _archipelago.Tokens;
        ImGui.Text("Archipelago Hint Tokens:");
        ImGui.BeginDisabled();
        ImGui.InputInt("##Archipelago Credits:", ref credits);
        ImGui.EndDisabled();

        var hintCost = ArchipelagoService.TokensPerHint;
        ImGui.Text("Archipelago Hint Price:");
        ImGui.BeginDisabled();
        ImGui.InputInt("##Archipelago Hint Price:", ref hintCost);
        ImGui.EndDisabled();

        if (credits < hintCost)
        {
            ImGui.BeginDisabled();
        }

        if (ImGui.Button("Purchase Hint"))
        {
            Task.Run(_archipelago.PurchaseHint);
        }

        if (credits < hintCost)
        {
            ImGui.EndDisabled();
        }

        ImGui.SameLine();
        ImGui.Indent(148);

        var disconnectButtonColor = new Vector4(0xD9, 0x53, 0x4F, 0xFF) / 0xFF;

        if (_isDisconnecting)
        {
            ImGui.BeginDisabled();
        }

        if (ImGuiComponents.IconButton(FontAwesomeIcon.PowerOff, defaultColor: disconnectButtonColor, hoveredColor: disconnectButtonColor))
        {
            _isDisconnecting = true;

            Task.Run(() =>
            {
                _archipelago.Disconnect();
                _isDisconnecting = false;
            });
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Disconnect");
        }

        if (_isDisconnecting)
        {
            ImGui.EndDisabled();
        }
    }
}
