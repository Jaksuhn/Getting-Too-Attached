using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;

namespace GettingTooAttached.Modules.Daemons;

public class MeldingDaemon
{
    public Configuration Configuration { get; init; }
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

    public MeldingDaemon() : this(new Configuration()) { }
    public MeldingDaemon(Configuration configuration) { Configuration = configuration; }

    public unsafe void LoopDaemon(Dalamud.Game.Framework framework)
    {
        if (Environment.TickCount64 > nextAttempt)
        {
            if (Configuration.enableLooping && (Configuration.loopAmt > 0 || Configuration.loopAmt == -1))
            {
                if (currentMeldStage switch
                {
                    MeldState.OPEN_MENU => Meld.OpenMenu(),
                    MeldState.SELECT_ITEM => Meld.SelectItem(),
                    MeldState.SELECT_MATERIA => Meld.SelectMateria(),
                    MeldState.CONFIRM_DIALOG => Meld.ConfirmMateriaDialog(),
                    MeldState.RETRIEVE_MATERIA => Meld.RetrieveMateria(),
                    MeldState.RETRIEVE_DIALOG => Meld.ConfirmRetrievalDialog()
                })
                {
                    currentMeldStage = (MeldState)(((int)currentMeldStage + 1) % 6);
                }
                if (Configuration.loopAmt != -1 && currentMeldStage == MeldState.END)
                {
                    Configuration.loopAmt -= 1;
                    if (Configuration.loopAmt == 0)
                    {
                        Configuration.enableLooping = false;
                        ResetMeldState();
                    }
                    currentMeldStage = MeldState.SELECT_ITEM;
                }
            }
            nextAttempt = Environment.TickCount64 + Configuration.attemptDelay;
        }
    }
    public unsafe void ResetMeldState()
    {
        currentMeldStage = (MeldState)((int)-1);
    }
}
