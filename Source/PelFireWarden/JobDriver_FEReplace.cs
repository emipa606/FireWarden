using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobDriver_FEReplace : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job);
    }

    private void FEReplaceThing(Pawn replacePawn, Thing togo, Thing tofill, bool Deltogo)
    {
        if (Deltogo)
        {
            togo.Destroy();
        }

        FWFoamUtility.FEFillUp(tofill);
        if (Controller.Settings.SendReplaceMsgs)
        {
            Messages.Message("FWrd.FEReplaced".Translate(replacePawn?.LabelShort, tofill?.Label.CapitalizeFirst()),
                replacePawn, MessageTypeDefOf.NeutralEvent, false);
        }
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var actor = GetActor();
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A);
        var replace = Toils_General.Wait(180);
        replace.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        replace.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        replace.WithProgressBarToilDelay(TargetIndex.A);
        yield return replace;
        yield return new Toil
        {
            initAction = delegate
            {
                if (TargetThingA.stackCount > 1)
                {
                    TargetThingA.stackCount--;
                    FEReplaceThing(actor, TargetThingA, TargetThingB, false);
                }
                else
                {
                    FEReplaceThing(actor, TargetThingA, TargetThingB, true);
                }

                EndJobWith(JobCondition.Succeeded);
            }
        };
    }
}