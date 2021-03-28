using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x0200001D RID: 29
    public class JobGiver_FWSwapback : ThinkNode_JobGiver
    {
        // Token: 0x04000038 RID: 56
        private static readonly JobGiver_SwapBackFW swapBack = new();

        // Token: 0x04000039 RID: 57
        private static readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x0400003A RID: 58
        private static readonly string FBDefName = "Firebeater";

        // Token: 0x06000070 RID: 112 RVA: 0x00004AC0 File Offset: 0x00002CC0
        protected override Job TryGiveJob(Pawn pawn)
        {
            var thinkResult = swapBack.TryIssueJobPackage(pawn, default);
            Job result;
            if (thinkResult.IsValid)
            {
                result = thinkResult.Job;
            }
            else
            {
                if (!pawn.inventory.innerContainer.NullOrEmpty())
                {
                    foreach (var invFECheck in pawn.inventory.innerContainer)
                    {
                        if ((invFECheck.def.defName == FEDefName || invFECheck.def.defName == FBDefName) &&
                            (invFECheck as FireWardenData)?.FWSwapType != "N")
                        {
                            FEResetVars((ThingWithComps) invFECheck);
                        }
                    }
                }

                result = null;
            }

            return result;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00004BAC File Offset: 0x00002DAC
        private void FEResetVars(ThingWithComps thingWC)
        {
            if (thingWC == null || thingWC.def.defName != FEDefName && thingWC.def.defName != FBDefName)
            {
                return;
            }

            ((FireWardenData) thingWC).FWSwapType = "N";
            ((FireWardenData) thingWC).FWPawnID = 0;
            ((FireWardenData) thingWC).FWPrimDef = "N";
        }

        // Token: 0x0200003D RID: 61
        [DefOf]
        public static class FWSwapJobs
        {
            // Token: 0x040000A7 RID: 167
            public static JobDef FWNoSwap;
        }
    }
}