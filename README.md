# GettingTooAttached

This plugin is design to get you the Getting Too Attached achievements.

Right now it's not user friendly/super intuitive, but I'll work on that and update this ReadMe accordingly.

To get it to work:

1. Once installed, have once piece of gear in your inventory and a materia that will not overcap a stat. The plugin does not behave correctly if you overcap.

2. Open the plugin with the command `/gta`.

3. Tick the checkbox to start the plugin.

4. Change the delay as needs be.

### NOTES

-   You must start the plugin without the meld menu up to begin with.
-   This will target the first item that appears in the melding menu, so if you have extra gear that would cap stats, take it out of your inventory if it's first.
-   If you wish to resume, it'll start on the last step you were on when you stopped. Hit the "reset MeldState" button to set it back to the start and make sure all menus are closed again.
-   This will loop endlessly. It does not check for when you've gotten the achievement(s).

### REPO

```
https://raw.githubusercontent.com/Jaksuhn/DalamudPlugins/main/pluginmaster.json
```

### Special Thanks

I barely knew C# before starting this and knew nothing about the inner workings of dalamud. Wouldn't've been possible without the teachings/guidance of Taurenkey and Ravicale.
