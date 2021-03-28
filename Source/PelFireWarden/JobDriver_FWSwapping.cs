using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x02000015 RID: 21
    public class JobDriver_FWSwapping : JobDriver
    {
        // Token: 0x04000026 RID: 38
        private static readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x04000027 RID: 39
        private static readonly string FBDefName = "Firebeater";

        // Token: 0x06000053 RID: 83 RVA: 0x00003DE8 File Offset: 0x00001FE8
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00003DEB File Offset: 0x00001FEB
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

                    var primToSwap = (ThingWithComps) job.GetTarget(TargetIndex.A).Thing;
                    Thing RemoveThing = null;
                    if (job.GetTarget(TargetIndex.B) != null)
                    {
                        RemoveThing = job.GetTarget(TargetIndex.B).Thing;
                    }

                    pawn.equipment.Remove(pawn.equipment.Primary);
                    if (RemoveThing != null)
                    {
                        var invGearToEquip = (ThingWithComps) RemoveThing;
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

        // Token: 0x06000055 RID: 85 RVA: 0x00003DFC File Offset: 0x00001FFC
        private void FWResetVars(ThingWithComps thingWC)
        {
            if (thingWC == null || thingWC.def.defName != FEDefName && thingWC.def.defName != FBDefName)
            {
                return;
            }

            ((FireWardenData) thingWC).FWSwapType = "N";
            ((FireWardenData) thingWC).FWPawnID = 0;
            ((FireWardenData) thingWC).FWPrimDef = "N";
        }
    }
}