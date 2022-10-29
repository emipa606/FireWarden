using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobGiver_FWSwapback : ThinkNode_JobGiver
{
    private static readonly JobGiver_SwapBackFW swapBack = new JobGiver_SwapBackFW();

    private static readonly string FEDefName = "Gun_Fire_Ext";

    private static readonly string FBDefName = "Firebeater";

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
                        FEResetVars((ThingWithComps)invFECheck);
                    }
                }
            }

            result = null;
        }

        return result;
    }

    private void FEResetVars(ThingWithComps thingWC)
    {
        if (thingWC == null || thingWC.def.defName != FEDefName && thingWC.def.defName != FBDefName)
        {
            return;
        }

        ((FireWardenData)thingWC).FWSwapType = "N";
        ((FireWardenData)thingWC).FWPawnID = 0;
        ((FireWardenData)thingWC).FWPrimDef = "N";
    }

    [DefOf]
    public static class FWSwapJobs
    {
        public static JobDef FWNoSwap;
    }
}