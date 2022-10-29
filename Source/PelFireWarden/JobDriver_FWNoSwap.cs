using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobDriver_FWNoSwap : JobDriver
{
    private static readonly string FEDefName = "Gun_Fire_Ext";

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return new Toil
        {
            initAction = delegate
            {
                if (!pawn.inventory.innerContainer.NullOrEmpty())
                {
                    foreach (var invFECheck in pawn.inventory.innerContainer)
                    {
                        if (invFECheck.def.defName == FEDefName &&
                            (invFECheck as FireWardenData)?.FWSwapType != "N")
                        {
                            FEResetVars((ThingWithComps)invFECheck);
                        }
                    }
                }

                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    private void FEResetVars(ThingWithComps thingWC)
    {
        if (thingWC == null || thingWC.def.defName != FEDefName)
        {
            return;
        }

        ((FireWardenData)thingWC).FWSwapType = "N";
        ((FireWardenData)thingWC).FWPawnID = 0;
        ((FireWardenData)thingWC).FWPrimDef = "N";
    }
}