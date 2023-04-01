using ClickLib.Clicks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Numerics;

namespace GettingTooAttached.Windows;

public class MainWindow : Window, IDisposable
{
    private TextureWrap GoatImage;
    private Plugin Plugin;

    private Configuration Configuration;
    internal static GameGui GameGui { get; private set; } = null!;

    public MainWindow(Plugin plugin, TextureWrap goatImage) : base(
        "Getting Too Attached", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.GoatImage = goatImage;
        this.Plugin = plugin;
    }

    public void Dispose()
    {
        this.GoatImage.Dispose();
    }

    public static unsafe bool IsAddonReady(AtkUnitBase* addon)
    {
        return addon->IsVisible && addon->UldManager.LoadedState == AtkLoadState.Loaded;
    }

    public static unsafe bool OpenMenu()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MateriaAttach", 1);
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

    public static unsafe bool SelectItem()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MateriaAttach", 1);
        if (!Svc.Condition[ConditionFlag.Occupied39] && Svc.Condition[ConditionFlag.NormalConditions] && addon != null && IsAddonReady(addon))
        {
            try
            {
                var materializePTR = Svc.GameGui.GetAddonByName("MateriaAttach", 1);
                if (materializePTR == IntPtr.Zero)
                    return false;

                var materalizeWindow = (AtkUnitBase*)materializePTR;
                if (materalizeWindow == null)
                    return false;


                var SelectItemvalues = stackalloc AtkValue[4];
                SelectItemvalues[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 1,
                };
                SelectItemvalues[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                SelectItemvalues[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 1,
                };

                SelectItemvalues[3] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };

                materalizeWindow->FireCallback(1, SelectItemvalues);
                PluginLog.Log("selectitem");
                return true;
            }
            catch
            {
                PluginLog.Log("select item error");
            }
        }
        return false;
    }

    public static unsafe bool SelectMateria()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MateriaAttach", 1);
        if (!Svc.Condition[ConditionFlag.Occupied39] && Svc.Condition[ConditionFlag.NormalConditions] && !IsMateriaMenuDialogOpen() && IsAddonReady(addon))
        {
            try
            {
                var materializePTR = Svc.GameGui.GetAddonByName("MateriaAttach", 1);
                if (materializePTR == IntPtr.Zero)
                    return false;

                var materalizeWindow = (AtkUnitBase*)materializePTR;
                if (materalizeWindow == null)
                    return false;

                var SelectMateriavalues = stackalloc AtkValue[4];
                SelectMateriavalues[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 2,
                };
                SelectMateriavalues[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                SelectMateriavalues[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 1,
                };

                SelectMateriavalues[3] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };

                materalizeWindow->FireCallback(1, SelectMateriavalues);
                PluginLog.Log("selectmateria");
                return true;
            }
            catch
            {
                PluginLog.Log("select materia error");
            }
        }
        return false;
    }

    public static bool IsMateriaMenuDialogOpen() => Svc.GameGui.GetAddonByName("MateriaAttachDialog", 1) != IntPtr.Zero;

    public unsafe static bool ConfirmMateriaDialog()
    {
        if (MainWindow.IsMateriaMenuDialogOpen())
        {
            try
            {
                var addon = Svc.GameGui.GetAddonByName("MateriaAttachDialog", 1);
                if (addon == IntPtr.Zero)
                    return false;

                var meldDialogWindow = (AtkUnitBase*)addon;
                if (meldDialogWindow == null)
                    return false;

                // Confirming the meld
                var MeldDialogvalues = stackalloc AtkValue[4];
                MeldDialogvalues[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                MeldDialogvalues[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                MeldDialogvalues[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };

                meldDialogWindow->FireCallback(1, MeldDialogvalues);
                meldDialogWindow->Close(true);
                PluginLog.Log("materiaattachdialog");
                return true;
            }
            catch (Exception e)
            {
                PluginLog.Log("confirm error " + e.ToString());
            }
        }
        return false;
    }

    public static bool IsMateriaRetrieveDialogOpen() => Svc.GameGui.GetAddonByName("MateriaRetrieveDialog", 1) != IntPtr.Zero;

    public static unsafe bool RetrieveMateria()
    {
        var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MateriaAttach", 1);
        if (!Svc.Condition[ConditionFlag.MeldingMateria] && Svc.Condition[ConditionFlag.NormalConditions] && IsAddonReady(addon))
        {
            try
            {
                var materializePTR = Svc.GameGui.GetAddonByName("MateriaAttach", 1);
                if (materializePTR == IntPtr.Zero)
                    return false;

                var materalizeWindow = (AtkUnitBase*)materializePTR;
                if (materalizeWindow == null)
                    return false;


                var SelectItemvalues = stackalloc AtkValue[4];
                SelectItemvalues[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 1,
                };
                SelectItemvalues[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                SelectItemvalues[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 1,
                };

                SelectItemvalues[3] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };

                materalizeWindow->FireCallback(1, SelectItemvalues);

                var RightClickCallback = stackalloc AtkValue[4];
                RightClickCallback[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 4,
                };
                RightClickCallback[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                RightClickCallback[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = 1,
                };

                RightClickCallback[3] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };

                materalizeWindow->FireCallback(1, RightClickCallback);

                var contextPTR = Svc.GameGui.GetAddonByName("ContextMenu", 1);
                if (contextPTR == IntPtr.Zero)
                    return false;

                var contextWindow = (AtkUnitBase*)contextPTR;
                if (contextWindow == null)
                    return false;

                var RetrieveCallback = stackalloc AtkValue[5];
                RetrieveCallback[0] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                RetrieveCallback[1] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 1,
                };
                RetrieveCallback[2] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt,
                    UInt = 0,
                };
                RetrieveCallback[3] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };
                RetrieveCallback[4] = new()
                {
                    Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                    Int = 0,
                };

                contextWindow->FireCallback(1, RetrieveCallback);
                PluginLog.Log("retrievemateria");
                return true;
            }
            catch
            {
                PluginLog.Log("retrieve error");
            }
        }
        return false;
    }

    public unsafe static bool ConfirmRetrievalDialog()
    {
        if (MainWindow.IsMateriaRetrieveDialogOpen())
        {
            try
            {
                var retrievePTR = Svc.GameGui.GetAddonByName("MateriaRetrieveDialog", 1);
                if (retrievePTR == IntPtr.Zero)
                    return false;

                var retrievalWindow = (AtkUnitBase*)retrievePTR;
                if (retrievalWindow == null)
                    return false;
                PluginLog.Log(retrievePTR.ToString());

                ClickMateriaRetrieveDialog.Using(retrievePTR).Begin();
                PluginLog.Log("retrievaldialog");
                return true;
            }
            catch (Exception e)
            {
                PluginLog.Log("retrieve error " + e.ToString());
            }
        }
        return false;
    }

    public unsafe override void Draw()
    {
        ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }

        ImGui.Spacing();

        ImGui.Text("Debug Buttons");

        if (ImGui.Button("Open Materia"))
        {
            ActionManager.Instance()->UseAction(ActionType.General, 13);
        }

        if (ImGui.Button("Target gear"))
        {
            SelectItem();
        }

        if (ImGui.Button("Select Materia"))
        {
            SelectMateria();
        }

        if (ImGui.Button("Meld Materia Dialog"))
        {
            if (IsMateriaMenuDialogOpen())
            {
                ConfirmMateriaDialog();
            }
        }

        if (ImGui.Button("Extract Materia"))
        {
            RetrieveMateria();
        }

        if (ImGui.Button("Retrieval Dialog"))
        {
            if (IsMateriaRetrieveDialogOpen())
            {
                ConfirmRetrievalDialog();
            }
        }

        if (ImGui.Button("Reset MeldState"))
        {
            Plugin.ResetMeldState();
        }

    }
}
