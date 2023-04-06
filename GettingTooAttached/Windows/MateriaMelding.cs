using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using GettingTooAttached.Modules;
using ImGuiNET;

namespace GettingTooAttached.Windows;

internal unsafe class MateriaMelding : Window
{
    internal float height;
    private const string addon = "MateriaAttach";
    public MateriaMelding() : base("GettingTooAttached overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize, true)
    {
        RespectCloseHotkey = false;
    }

    public override void Draw()
    {
        var tryAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName(addon);
        if (tryAddon == null || !tryAddon->IsVisible)
        {
            return;
        }
        ImGui.Checkbox("Enable GettingTooAttached", ref Meld.Enabled);
        if (!Meld.Enabled)
        {
            ImGui.SameLine();
            ImGui.SetNextItemWidth(200);
        }
        height = ImGui.GetWindowSize().Y;
    }
}
