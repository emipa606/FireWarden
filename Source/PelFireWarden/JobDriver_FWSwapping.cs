using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobDriver_FWSwapping : JobDriver
{
    private static readonly string FEDefName = "Gun_Fire_Ext";

    private static readonly string FBDefName = "Firebeater";

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
                if (pawn.equipment.Primary.def.defName != FEDefName &&
                    pawn.equipment.Primary.def.defName != FBDefName)
                {
                    return;
                }

                var primToSwap = (ThingWithComps)job.GetTarget(TargetIndex.A).Thing;
                Thing RemoveThing = null;
                if (job.GetTarget(TargetIndex.B) != null)
                {
                    RemoveThing = job.GetTarget(TargetIndex.B).Thing;
                }

                pawn.equipment.Remove(pawn.equipment.Primary);
                if (RemoveThing != null)
                {
                    var invGearToEquip = (ThingWithComps)RemoveThing;
                    pawn.inventory.innerContainer.Remove(RemoveThing);
                    pawn.equipment.MakeRoomFor(invGearToEquip);
                    pawn.equipment.AddEquipment(invGearToEquip);
                }

                pawn.inventory.innerContainer.TryAdd(primToSwap);
                FWResetVars(primToSwap);
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            },
            defaultCompleteMode = ToilCompleteMode.FinishedBusy
        };
    }

    private void FWResetVars(ThingWithComps thingWC)
    {
        if (thingWC == null || thingWC.def.defName != FEDefName && thingWC.def.defName != FBDefName)
        {
            return;
        }

        ((FireWardenData)thingWC).FWSwapType = "N";
        ((FireWardenData)thingWC).FWPawnID = 0;
        ((FireWardenData)thingWC).FWPrimDef = "N";
    }
}