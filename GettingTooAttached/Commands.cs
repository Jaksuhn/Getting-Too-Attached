using System;
using Dalamud.Game.Command;
using GettingTooAttached.Windows;

namespace GettingTooAttached
{
    internal class Commands : IDisposable
    {
        private Plugin Plugin { get; }

        internal Commands(Plugin plugin)
        {
            Plugin = plugin;

            Service.CommandManager.AddHandler("/lt", new CommandInfo(OnCommand)
            {
                HelpMessage = $"Toggle visibility of the {Plugin.Name} window",
            });
        }

        public void Dispose()
        {
            Service.CommandManager.RemoveHandler("/lt");
        }

        private void OnCommand(string command, string arguments) => Plugin.ToggleMainWindow();
    }
}
