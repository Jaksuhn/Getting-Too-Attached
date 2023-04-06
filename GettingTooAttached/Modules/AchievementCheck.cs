using ClickLib.Clicks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ImGuiScene;
using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GettingTooAttached.Modules;

public class AchievementCheck
{

    private Configuration Configuration;
    internal static GameGui GameGui { get; private set; } = null!;

    public AchievementCheck() { }

    public void Dispose() { }

    public static unsafe bool IsAddonReady(AtkUnitBase* addon)
    {
        return addon->IsVisible && addon->UldManager.LoadedState == AtkLoadState.Loaded;
    }

    public static unsafe bool OpenMenu()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("Achievement", 1);
        if (Svc.Condition[ConditionFlag.NormalConditions] && addon == null)
        {
            try
            {
                ActionManager.Instance()->UseAction(ActionType.General, 13);
                PluginLog.Log("open menu");
                return true;
            }
            catch
            {
                PluginLog.Log("open menu error");
            }
        }
        return false;
    }
    public static bool isAchievementMenuOpen() => Svc.GameGui.GetAddonByName("Achievement", 1) != IntPtr.Zero;

    public static unsafe bool GoToAchievement()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("Achievement", 1);
        if (isAchievementMenuOpen() && IsAddonReady(addon))
        {
            try
            {
                var achievementPTR = Svc.GameGui.GetAddonByName("Achievement", 1);
                if (achievementPTR == IntPtr.Zero)
                    return false;

                var achievementWindow = (AtkUnitBase*)achievementPTR;
                if (achievementWindow == null)
                    return false;


                // var SelectItemsvalues = stackalloc AtkValue[4];
                // SelectItemsvalues[0] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                //     UInt = 0,
                // };
                // SelectItemsvalues[1] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                //     Int = 0,
                // };
                // SelectItemsvalues[2] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                //     UInt = 27,
                // };
                // SelectItemsvalues[3] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                //     UInt = 4,
                // };
                // achievementWindow->FireCallback(1, SelectItemsvalues);

                var SelectMateriavalues = stackalloc AtkValue[4];
                SelectMateriavalues[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = 0,
                };
                SelectMateriavalues[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                SelectMateriavalues[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = 31,
                };
                SelectMateriavalues[3] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = 4,
                };
                achievementWindow->FireCallback(1, SelectMateriavalues);

                // var SelectGTAvalues = stackalloc AtkValue[4];
                // SelectGTAvalues[0] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                //     UInt = 1,
                // };
                // SelectGTAvalues[1] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                //     Int = 0,
                // };
                // SelectGTAvalues[2] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                //     UInt = 7,
                // };
                // SelectGTAvalues[3] = new()
                // {
                //     Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                //     Int = 0,
                // };
                // achievementWindow->FireCallback(1, SelectGTAvalues);

                PluginLog.Log("Finding Getting Too Attached VII Progress");
                return true;
            }
            catch
            {
                PluginLog.Log("FAILED Finding Getting Too Attached VII Progress");
            }
        }
        return false;
    }

    public static unsafe string GetGTAProgress()
    {
        try
        {
            var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("Achievement");
            var progress = addon->AtkValues[3347];
            return Marshal.PtrToStringUTF8(new nint(progress.String)) ?? "Unable to Get Achievement Progress";
        }
        catch (Exception e)
        {
            PluginLog.Log("[GettingTooAttached] Unable to Get Achievement Progress. " + e.ToString());
            return "Unable to Get Achievement Progress";
        }
    }
}
