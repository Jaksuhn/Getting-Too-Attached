// using Dalamud.Interface;
// using ImGuiNET;
// using System.Numerics;
// using ECommons.DalamudServices;

// namespace GettingTooAttached.Windows;

// public partial class IslandScheduler
// {
//     public Configuration Configuration;
//     private unsafe void DrawWorkshopOverlay(WorkshopWindow* addon, Vector2 pos, Vector2 size)
//     {
//         ImGui.SetNextWindowSize(size);
//         ImGui.SetNextWindowPos(pos);
//         ImGuiHelpers.ForceNextWindowMainViewport();
//     }

//     public unsafe void Draw()
//     {
//         if (!Configuration.EnableWorkshopModule)
//             return;

//         var addon = (WorkshopWindow*)Svc.GameGui.GetAddonByName("MJICraftSchedule", 1);
//         var pos = new Vector2(addon->Base.X, addon->Base.Y);
//         var size = new Vector2(addon->Base.WindowNode->AtkResNode.Width * addon->Base.WindowNode->AtkResNode.ScaleX,
//             addon->Base.WindowNode->AtkResNode.Height * addon->Base.WindowNode->AtkResNode.ScaleY);

//         DrawWorkshopOverlay(addon, pos, size);
//     }
// }
