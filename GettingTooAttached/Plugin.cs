using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using ECommons;
using GettingTooAttached.Windows;
using GettingTooAttached.Modules.Daemons;
using System;
using ECommons.DalamudServices;

namespace GettingTooAttached
{

    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "LazyToolBox";
        public Configuration Configuration { get; init; }
        private Commands Commands { get; }
        public WindowSystem WindowSystem = new("GettingTooAttached");

        private MainWindow MainWindow { get; init; }
        private MateriaMelding MateriaMelding { get; }
        private MeldingDaemon MeldingDaemon { get; }
        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            pluginInterface.Create<Service>();
            Service.Plugin = this;
            Service.CommandManager = commandManager;
            Commands = new Commands(this);

            Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.Interface);

            ECommonsMain.Init(pluginInterface, this);

            this.MainWindow = new MainWindow(this);

            WindowSystem.AddWindow(MainWindow);

            Service.Interface.UiBuilder.Draw += this.Draw;
            Service.Interface.UiBuilder.OpenConfigUi += this.ToggleMainWindow;

            MeldingDaemon = new MeldingDaemon();
            Service.Framework.Update += MeldingDaemon.LoopDaemon;
        }

        internal void ToggleMainWindow() { MainWindow.Toggle(); }

        private void Draw()
        {
            this.MainWindow.Draw();
            // this.MateriaMelding.Draw();
            // this.IslandScheduler.Draw();
        }

        public void PrintPluginMessage(String msg)
        {
            var message = new XivChatEntry
            {
                Message = new SeStringBuilder()
                .AddUiForeground($"[{Name}] ", 45)
                .AddUiForeground(msg, 576)
                .Build()
            };

            Svc.Chat.PrintChat(message);
        }

        public void Dispose()
        {
            Service.Interface.UiBuilder.Draw -= this.Draw;
            this.Commands.Dispose();
            this.WindowSystem.RemoveAllWindows();
        }
    }
}
