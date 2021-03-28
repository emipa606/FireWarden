using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x02000014 RID: 20
    public class JobDriver_FWNoSwap : JobDriver
    {
        // Token: 0x04000025 RID: 37
        private static readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x0600004D RID: 77 RVA: 0x00003CB0 File Offset: 0x00001EB0
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00003CB3 File Offset: 0x00001EB3
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
                                FEResetVars((ThingWithComps) invFECheck);
                            }
                        }
                    }

                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00003CC4 File Offset: 0x00001EC4
        private void FEResetVars(ThingWithComps thingWC)
        {
            if (thingWC == null || thingWC.def.defName != FEDefName)
            {
                return;
            }

            ((FireWardenData) thingWC).FWSwapType = "N";
            ((FireWardenData) thingWC).FWPawnID = 0;
            ((FireWardenData) thingWC).FWPrimDef = "N";
        }
    }
}