using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using GettingTooAttached.Windows;
using ECommons;
using ECommons.DalamudServices;
using Dalamud.Game;
using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;

namespace GettingTooAttached
{

    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] public static Framework Framework { get; private set; } = null!;
        public string Name => "Getting Too Attached";
        private const string main_command = "/gta";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("GettingTooAttached");

        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }
        private Task task = Task.CompletedTask;

        private enum MeldState
        {
            OPEN_MENU = -1,
            SELECT_ITEM = 0,
            SELECT_MATERIA = 1,
            CONFIRM_DIALOG = 2,
            RETRIEVE_MATERIA = 3,
            RETRIEVE_DIALOG = 4
        }

        private long nextAttempt = 0;
        private const long attemptDelay = 1000;

        MeldState currentMeldStage = MeldState.OPEN_MENU;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            Framework.Update += LoopDaemon;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            ECommonsMain.Init(pluginInterface, this);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this, goatImage);

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            this.CommandManager.AddHandler(main_command, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open plugin interface"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public unsafe void ResetMeldState()
        {
            this.currentMeldStage = (MeldState)((int)-1);
            PluginLog.Log("Reset meld state to " + this.currentMeldStage);
        }

        private unsafe void LoopDaemon(Framework framework)
        {
            if (Environment.TickCount64 > nextAttempt)
            {
                if (this.Configuration.SomePropertyToBeSavedAndWithADefault)
                {
                    if (this.currentMeldStage switch
                    {
                        MeldState.OPEN_MENU => MainWindow.OpenMenu(),
                        MeldState.SELECT_ITEM => MainWindow.SelectItem(),
                        MeldState.SELECT_MATERIA => MainWindow.SelectMateria(),
                        MeldState.CONFIRM_DIALOG => MainWindow.ConfirmMateriaDialog(),
                        MeldState.RETRIEVE_MATERIA => MainWindow.RetrieveMateria(),
                        MeldState.RETRIEVE_DIALOG => MainWindow.ConfirmRetrievalDialog()
                    })
                    {
                        this.currentMeldStage = (MeldState)(((int)this.currentMeldStage + 1) % 5);
                    }
                }
                nextAttempt = Environment.TickCount64 + attemptDelay;
            }
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            ConfigWindow.Dispose();
            MainWindow.Dispose();
            Framework.Update -= LoopDaemon;

            this.CommandManager.RemoveHandler(main_command);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
