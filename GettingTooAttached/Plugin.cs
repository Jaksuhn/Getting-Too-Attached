using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using ECommons;
using GettingTooAttached.Windows;
using GettingTooAttached.Modules;
using System;
using System.IO;
using System.Threading.Tasks;
using ECommons.DalamudServices;

namespace GettingTooAttached
{

    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] public static Framework Framework { get; private set; } = null!;
        [PluginService] public static ChatGui Chat { get; private set; } = null!;
        public string Name => "Getting Too Attached";
        private const string main_command = "/gta";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("GettingTooAttached");

        public XivChatType MessageChannel { get; set; }

        private MainWindow MainWindow { get; init; }
        private Task task = Task.CompletedTask;

        private enum MeldState
        {
            OPEN_MENU = -1,
            SELECT_ITEM = 0,
            SELECT_MATERIA = 1,
            CONFIRM_DIALOG = 2,
            RETRIEVE_MATERIA = 3,
            RETRIEVE_DIALOG = 4,
            END
        }

        private long nextAttempt = 0;

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

            MainWindow = new MainWindow(this, goatImage);

            WindowSystem.AddWindow(MainWindow);

            this.CommandManager.AddHandler(main_command, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open plugin interface"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
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
                if (this.Configuration.enableLooping && (this.Configuration.loopAmt > 0 || this.Configuration.loopAmt == -1))
                {
                    if (this.currentMeldStage switch
                    {
                        MeldState.OPEN_MENU => Meld.OpenMenu(),
                        MeldState.SELECT_ITEM => Meld.SelectItem(),
                        MeldState.SELECT_MATERIA => Meld.SelectMateria(),
                        MeldState.CONFIRM_DIALOG => Meld.ConfirmMateriaDialog(),
                        MeldState.RETRIEVE_MATERIA => Meld.RetrieveMateria(),
                        MeldState.RETRIEVE_DIALOG => Meld.ConfirmRetrievalDialog()
                    })
                    {
                        this.currentMeldStage = (MeldState)(((int)this.currentMeldStage + 1) % 6);
                    }
                    if (Configuration.loopAmt != -1 && currentMeldStage == MeldState.END)
                    {
                        this.Configuration.loopAmt -= 1;
                        if (this.Configuration.loopAmt == 0)
                        {
                            this.Configuration.enableLooping = false;
                            ResetMeldState();
                        }
                        currentMeldStage = MeldState.SELECT_ITEM;
                    }
                }
                nextAttempt = Environment.TickCount64 + Configuration.attemptDelay;
            }
        }

        public void PrintPluginMessage(String msg)
        {
            var message = new XivChatEntry
            {
                Message = new SeStringBuilder()
                .AddUiForeground($"[GettingTooAttached] ", 45)
                .AddUiForeground(msg, 576)
                .Build()
            };

            Svc.Chat.PrintChat(message);
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

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
    }
}
