using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobGiver_SwapBackFW : ThinkNode
{
    private readonly string FBDefName = "Firebeater";

    private readonly string FEDefName = "Gun_Fire_Ext";

    private readonly string SwapReturnType = "ER";

    private int FWPawnId;

    private string FWPrimDef = "N";

    private string FWSwapinfo = "N";

    private ThingWithComps FWToCheck;

    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParm)
    {
        ThinkResult result;
        var HasPrimFWGear = false;
        var HasPrimFEEq = pawn.equipment.Primary != null && pawn.equipment.Primary.def.defName == FEDefName;
        var HasPrimFBEq = pawn.equipment.Primary != null && pawn.equipment.Primary.def.defName == FBDefName;
        if (HasPrimFEEq || HasPrimFBEq)
        {
            HasPrimFWGear = true;
        }

        if (!HasPrimFWGear)
        {
            result = ThinkResult.NoJob;
        }
        else
        {
            var IsFightingFires = pawn.CurJob != null && pawn.CurJob.targetA != null &&
                                  pawn.CurJob.targetA.HasThing && pawn.CurJob.targetA.Thing.def == ThingDefOf.Fire;
            var IsAlreadySwapping = pawn.CurJob != null &&
                                    (pawn.CurJobDef == FWSwapJobs.FWSwapping ||
                                     pawn.CurJobDef == FWSwapJobs.FWSwapReturn);
            if (IsFightingFires || IsAlreadySwapping)
            {
                result = ThinkResult.NoJob;
            }
            else
            {
                var IsHomeFire = false;
                var list = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Fire);
                foreach (var thing in list)
                {
                    if (!pawn.Map.areaManager.Home[thing.Position] || thing.Position.Fogged(thing.Map))
                    {
                        continue;
                    }

                    IsHomeFire = true;
                    break;
                }

                if (IsHomeFire)
                {
                    result = ThinkResult.NoJob;
                }
                else if (pawn.Drafted || HealthAIUtility.ShouldSeekMedicalRest(pawn) || pawn.IsBurning())
                {
                    result = ThinkResult.NoJob;
                }
                else
                {
                    FWToCheck = pawn.equipment.Primary;
                    FWSwapinfo = (FWToCheck as FireWardenData)?.FWSwapType;
                    FWPawnId = ((FireWardenData)FWToCheck).FWPawnID;
                    FWPrimDef = (FWToCheck as FireWardenData)?.FWPrimDef;
                    if (pawn.thingIDNumber == FWPawnId)
                    {
                        Thing RemoveThing = null;
                        var Swap = false;
                        foreach (var invThing in pawn.inventory.innerContainer)
                        {
                            if (invThing.def.defName != FWPrimDef)
                            {
                                continue;
                            }

                            RemoveThing = invThing;
                            Swap = true;
                            break;
                        }

                        var SwapJobDef = FWSwapJobs.FWSwapping;
                        if (FWSwapinfo == SwapReturnType)
                        {
                            SwapJobDef = FWSwapJobs.FWSwapReturn;
                        }

                        var swapJob = Swap
                            ? new Job(SwapJobDef, pawn.equipment.Primary, RemoveThing)
                            : new Job(SwapJobDef, pawn.equipment.Primary);
                        return new ThinkResult(swapJob, this);
                    }

                    if (FWSwapinfo != "N" || FWPawnId != 0 || FWPrimDef != "N")
                    {
                        FWResetVars(pawn.equipment.Primary);
                    }

                    result = ThinkResult.NoJob;
                }
            }
        }

        return result;
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