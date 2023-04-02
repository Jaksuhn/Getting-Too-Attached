using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
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
        this.Configuration = plugin.Configuration;
    }

    public void Dispose()
    {
        this.GoatImage.Dispose();
    }
    public unsafe override void Draw()
    {
        bool enableLooping = Configuration.enableLooping;
        int delay = Configuration.attemptDelay;
        int loops = Configuration.loopAmt;
        if (ImGui.Checkbox("Start Looping", ref enableLooping))
        {
            this.Configuration.enableLooping = enableLooping;
            this.Configuration.Save();
        }

        if (ImGui.SliderInt("Loop Amount", ref loops, 0, 10000))
        {
            if (loops < 0) loops = 0;
            if (loops > 10000) loops = 10000;

            this.Configuration.loopAmt = loops;
            this.Configuration.Save();
        }

        if (ImGui.SliderInt("Set delay (ms)###ActionDelay", ref delay, 0, 3000))
        {
            if (delay < 0) delay = 0;
            if (delay > 3000) delay = 3000;

            this.Configuration.attemptDelay = delay;
            this.Configuration.Save();
        }

        ImGui.Spacing();

        ImGui.Text("Debug Buttons");

        // if (ImGui.Button("Open Materia"))
        // {
        //     ActionManager.Instance()->UseAction(ActionType.General, 13);
        // }

        // if (ImGui.Button("Target gear"))
        // {
        //     SelectItem();
        // }

        // if (ImGui.Button("Select Materia"))
        // {
        //     SelectMateria();
        // }

        // if (ImGui.Button("Meld Materia Dialog"))
        // {
        //     if (IsMateriaMenuDialogOpen())
        //     {
        //         ConfirmMateriaDialog();
        //     }
        // }

        // if (ImGui.Button("Extract Materia"))
        // {
        //     RetrieveMateria();
        // }

        // if (ImGui.Button("Retrieval Dialog"))
        // {
        //     if (IsMateriaRetrieveDialogOpen())
        //     {
        //         ConfirmRetrievalDialog();
        //     }
        // }

        if (ImGui.Button("Reset MeldState"))
        {
            Plugin.ResetMeldState();
        }
        if (ImGui.Button("Test"))
        {
            // Svc.Chat.Print("[GettingTooAttached] test");
            Svc.Toasts.ShowQuest("test", new QuestToastOptions() { PlaySound = true, DisplayCheckmark = false });
            Plugin.PrintPluginMessage("test");
        }

        if (ImGui.Button("achievement test"))
        {
            Modules.AchievementCheck.GoToAchievement();
            Plugin.PrintPluginMessage("test");
        }
    }
}
