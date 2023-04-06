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
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GettingTooAttached.Modules;

public class IslandWorkshop
{

    private Configuration Configuration;
    internal static GameGui GameGui { get; private set; } = null!;

    class ItemValues
    {
        public uint id { get; set; }
        public int hours { get; set; }

        internal ItemValues(uint id, int hours)
        {
            this.hours = hours;
            this.id = id;
        }
    }
    private static Dictionary<string, IslandWorkshop.ItemValues> workshopItems = new Dictionary<string, ItemValues>();

    static IslandWorkshop()
    {
        workshopItems.Add("Isleworks Potion", new ItemValues(0, 4));
        workshopItems.Add("Isleworks Firesand", new ItemValues(1, 4));
        workshopItems.Add("Isleworks Grilled Clam", new ItemValues(3, 4));
        workshopItems.Add("Isleworks Necklace", new ItemValues(4, 4));
        workshopItems.Add("Isleworks Sauerkraut", new ItemValues(8, 4));
        workshopItems.Add("Isleworks Baked Pumpkin", new ItemValues(9, 4));
        workshopItems.Add("Isleworks Culinary Knife", new ItemValues(11, 4));
        workshopItems.Add("Isleworks Brush", new ItemValues(12, 4));
        workshopItems.Add("Isleworks Boiled Egg", new ItemValues(13, 4));
        workshopItems.Add("Isleworks Earrings", new ItemValues(15, 4));
        workshopItems.Add("Isleworks Butter", new ItemValues(16, 4));
        workshopItems.Add("Isleworks Parsnip Salad", new ItemValues(25, 4));
        workshopItems.Add("Isleworks Rope", new ItemValues(28, 4));
        workshopItems.Add("Isleworks Squid Ink", new ItemValues(32, 4));
        workshopItems.Add("Isleworks Tomato Relish", new ItemValues(35, 4));
        workshopItems.Add("Isleworks Corn Flakes", new ItemValues(38, 4));
        workshopItems.Add("Isleworks Coconut Juice", new ItemValues(51, 4));
        workshopItems.Add("Isleworks Honey", new ItemValues(52, 4));
        workshopItems.Add("Isleworks Powdered Paprika", new ItemValues(55, 4));
        workshopItems.Add("Isleworks Isleloaf", new ItemValues(57, 4));
        workshopItems.Add("Isleworks Popoto Salad", new ItemValues(58, 4));
        workshopItems.Add("Isleworks Dressing", new ItemValues(59, 4));
        workshopItems.Add("Isleworks Wooden Chair", new ItemValues(2, 6));
        workshopItems.Add("Isleworks Coral Ring", new ItemValues(5, 6));
        workshopItems.Add("Isleworks Barbut", new ItemValues(6, 6));
        workshopItems.Add("Isleworks Macuahuitl", new ItemValues(7, 6));
        workshopItems.Add("Isleworks tunic", new ItemValues(10, 6));
        workshopItems.Add("Isleworks Hora", new ItemValues(14, 6));
        workshopItems.Add("Isleworks Brick Counter", new ItemValues(17, 6));
        workshopItems.Add("Isleworks Sweet Popoto", new ItemValues(24, 6));
        workshopItems.Add("Isleworks Caramels", new ItemValues(26, 6));
        workshopItems.Add("Isleworks Ribbon", new ItemValues(27, 6));
        workshopItems.Add("Isleworks Cavalier's Hat", new ItemValues(29, 6));
        workshopItems.Add("Isleworks Horn", new ItemValues(30, 6));
        workshopItems.Add("Isleworks Salt Cod", new ItemValues(31, 6));
        workshopItems.Add("Isleworks Essential Draught", new ItemValues(33, 6));
        workshopItems.Add("Isleberry Jam", new ItemValues(34, 6));
        workshopItems.Add("Isleworks Onion Soup", new ItemValues(36, 6));
        workshopItems.Add("Islefish Pie", new ItemValues(37, 6));
        workshopItems.Add("Isleworks Vegetable Juice", new ItemValues(43, 6));
        workshopItems.Add("Isleworks Pumpkin Pudding", new ItemValues(44, 6));
        workshopItems.Add("Isleworks Sheepfluff Rug", new ItemValues(45, 6));
        workshopItems.Add("Isleworks Garden Scythe", new ItemValues(46, 6));
        workshopItems.Add("Isleworks Dried Flowers", new ItemValues(54, 6));
        workshopItems.Add("Isleworks Cawl Cennin", new ItemValues(56, 6));
        workshopItems.Add("Bronze Sheep", new ItemValues(18, 8));
        workshopItems.Add("Isleworks Growth Formula", new ItemValues(19, 8));
        workshopItems.Add("Isleworks Garnet Rapier", new ItemValues(20, 8));
        workshopItems.Add("Isleworks Spruce Round Shield", new ItemValues(21, 8));
        workshopItems.Add("Isleworks Shark Oil", new ItemValues(22, 8));
        workshopItems.Add("Isleworks Silver Ear Cuffs", new ItemValues(23, 8));
        workshopItems.Add("Isleworks Pickled Radish", new ItemValues(39, 8));
        workshopItems.Add("Isleworks Iron Axe", new ItemValues(40, 8));
        workshopItems.Add("Isleworks Quartz Ring", new ItemValues(41, 8));
        workshopItems.Add("Isleworks Porcelain Vase", new ItemValues(42, 8));
        workshopItems.Add("Isleworks Bed", new ItemValues(47, 8));
        workshopItems.Add("Isleworks Scale Fingers", new ItemValues(48, 8));
        workshopItems.Add("Isleworks Crook", new ItemValues(49, 8));
        workshopItems.Add("Isleworks Coral Sword", new ItemValues(50, 8));
        workshopItems.Add("Isleworks Seashine Opal", new ItemValues(53, 8));
    }


    public void Dispose() { }

    public static unsafe bool IsAddonReady(AtkUnitBase* addon)
    {
        return addon->IsVisible && addon->UldManager.LoadedState == AtkLoadState.Loaded;
    }

    public static unsafe bool OpenMenu()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MJICraftSchedule", 1);
        if (Svc.Condition[ConditionFlag.NormalConditions] && addon == null)
        {
            try
            {
                ActionManager.Instance()->UseAction(ActionType.General, 13);
                return true;
            }
            catch
            {
                PluginLog.Log("[GettingTooAttached] Failed to open workshop menu.");
            }
        }
        return false;
    }
    public static bool isWorkshopOpen() => Svc.GameGui.GetAddonByName("MJICraftSchedule") != IntPtr.Zero;

    public static unsafe bool OpenCycle(int cycle_day)
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MJICraftSchedule");
        if (isWorkshopOpen() && IsAddonReady(addon))
        {
            try
            {
                var workshopPTR = Svc.GameGui.GetAddonByName("MJICraftSchedule");
                if (workshopPTR == IntPtr.Zero)
                    return false;

                var workshopWindow = (AtkUnitBase*)workshopPTR;
                if (workshopWindow == null)
                    return false;


                var SelectCycle = stackalloc AtkValue[2];
                SelectCycle[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 19,
                };
                SelectCycle[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = (uint)(cycle_day - 1),
                };
                workshopWindow->FireCallback(1, SelectCycle);

                return true;
            }
            catch
            {
                PluginLog.Log("[GettingTooAttached] Failed to open cycle " + cycle_day);
            }
        }
        return false;
    }

    public static unsafe bool OpenAgenda(int workshop)
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MJICraftSchedule");
        if (isWorkshopOpen() && IsAddonReady(addon))
        {
            try
            {
                var workshopPTR = Svc.GameGui.GetAddonByName("MJICraftSchedule");
                if (workshopPTR == IntPtr.Zero)
                    return false;

                var workshopWindow = (AtkUnitBase*)workshopPTR;
                if (workshopWindow == null)
                    return false;


                var SelectAgenda = stackalloc AtkValue[3];
                SelectAgenda[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 16,
                };
                SelectAgenda[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = (uint)(workshop - 1),
                };
                SelectAgenda[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = (uint)(workshop - 1),
                };
                workshopWindow->FireCallback(1, SelectAgenda);

                return true;
            }
            catch
            {
                PluginLog.Log("[GettingTooAttached] Failed to open agenda for workshop " + workshop);
            }
        }
        return false;
    }

    public static unsafe bool Schedule(String key)
    {
        var item = workshopItems.TryGetValue(key, out var itemValues) ? itemValues : null;
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MJICraftScheduleSetting");
        if (IsAddonReady(addon))
        {
            try
            {
                var schedulerPTR = Svc.GameGui.GetAddonByName("MJICraftScheduleSetting");
                if (schedulerPTR == IntPtr.Zero)
                    return false;

                var schedulerWindow = (AtkUnitBase*)schedulerPTR;
                if (schedulerWindow == null)
                    return false;

                var SelectItem = stackalloc AtkValue[2];
                SelectItem[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 11,
                };
                SelectItem[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = item.id,
                };
                schedulerWindow->FireCallback(1, SelectItem);

                var Schedule = stackalloc AtkValue[1];
                Schedule[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 13,
                };
                schedulerWindow->FireCallback(1, Schedule);
                schedulerWindow->Close(true);

                return true;
            }
            catch
            {
                PluginLog.Log("[GettingTooAttached] Failed to schedule");
            }
        }
        return false;
    }
}
