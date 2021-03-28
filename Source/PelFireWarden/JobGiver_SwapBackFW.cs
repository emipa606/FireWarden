using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x0200001F RID: 31
    public class JobGiver_SwapBackFW : ThinkNode
    {
        // Token: 0x0400003E RID: 62
        private readonly string FBDefName = "Firebeater";

        // Token: 0x0400003D RID: 61
        private readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x04000043 RID: 67
        private readonly string SwapReturnType = "ER";

        // Token: 0x04000041 RID: 65
        private int FWPawnId;

        // Token: 0x04000042 RID: 66
        private string FWPrimDef = "N";

        // Token: 0x04000040 RID: 64
        private string FWSwapinfo = "N";

        // Token: 0x0400003F RID: 63
        private ThingWithComps FWToCheck;

        // Token: 0x06000074 RID: 116 RVA: 0x00004C40 File Offset: 0x00002E40
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
                        FWPawnId = ((FireWardenData) FWToCheck).FWPawnID;
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

        // Token: 0x06000075 RID: 117 RVA: 0x00004FA4 File Offset: 0x000031A4
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