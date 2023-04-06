using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace GettingTooAttached
{
    internal class Service
    {
        internal static Plugin Plugin = null!;
        internal static Configuration Configuration { get; set; } = null!;
        [PluginService] internal static DalamudPluginInterface Interface { get; private set; } = null!;
        [PluginService] internal static ChatGui ChatGui { get; private set; } = null!;
        [PluginService] internal static CommandManager CommandManager { get; set; } = null!;
        [PluginService] internal static Framework Framework { get; private set; } = null!;
    }
}
