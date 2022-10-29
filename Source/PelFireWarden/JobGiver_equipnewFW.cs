using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobGiver_equipnewFW : ThinkNode
{
    private static readonly string FEDefName = "Gun_Fire_Ext";

    private static readonly string FBDefName = "Firebeater";

    public int FEPawnId;

    public string FEPrimDef = "N";

    public string FESwapinfo = "N";

    public ThingWithComps FEToCheck;

    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParm)
    {
        ThinkResult result;
        if (!Controller.Settings.EquippingDone)
        {
            result = ThinkResult.NoJob;
        }
        else
        {
            var IsFW = pawn.IsColonistPlayerControlled &&
                       pawn.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) &&
                       !pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);
            if (!IsFW)
            {
                result = ThinkResult.NoJob;
            }
            else
            {
                var FWHasGear = false;
                var TargetGearDef = "N";
                if (FWResearch.FireExt.IsFinished)
                {
                    if (!FWGotFE(pawn))
                    {
                        if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
                        {
                            if (!Controller.Settings.BrawlerNotOK)
                            {
                                TargetGearDef = FEDefName;
                            }
                            else if (!FWGotFB(pawn))
                            {
                                TargetGearDef = FBDefName;
                            }
                            else
                            {
                                FWHasGear = true;
                            }
                        }
                        else
                        {
                            TargetGearDef = FEDefName;
                        }
                    }
                    else
                    {
                        FWHasGear = true;
                    }
                }
                else if (FWResearch.FireBeater.IsFinished)
                {
                    if (!FWGotFB(pawn))
                    {
                        TargetGearDef = FBDefName;
                    }
                    else
                    {
                        FWHasGear = true;
                    }
                }

                if (FWHasGear)
                {
                    result = ThinkResult.NoJob;
                }
                else
                {
                    var IsFightingFires = pawn.CurJob != null && pawn.CurJob.targetA != null &&
                                          pawn.CurJob.targetA.HasThing &&
                                          pawn.CurJob.targetA.Thing.def == ThingDefOf.Fire;
                    var IsAlreadyEquipping = pawn.CurJob != null && pawn.CurJobDef == FWEquipJobs.FWEquipping;
                    if (IsFightingFires || IsAlreadyEquipping)
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
                            var FWSearchRange = (float)Controller.Settings.SearchRange;
                            if (FWSearchRange < 25f)
                            {
                                FWSearchRange = 25f;
                            }

                            if (FWSearchRange > 75f)
                            {
                                FWSearchRange = 75f;
                            }

                            var FElist =
                                pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FEDefName));
                            var FBlist =
                                pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FBDefName));
                            var traverseParams = TraverseParms.For(pawn);

                            bool ValidatorFe(Thing t)
                            {
                                return !t.IsForbidden(pawn) && pawn.CanReserve(t) &&
                                       !FWFoamUtility.ReplaceFEFoam(t) && FWFoamUtility.HasFEFoam(t);
                            }

                            bool ValidatorFb(Thing t)
                            {
                                return !t.IsForbidden(pawn) && pawn.CanReserve(t);
                            }

                            Thing ThingToGrab = null;
                            if (TargetGearDef != "N")
                            {
                                if (TargetGearDef == FEDefName)
                                {
                                    if (!FWGotFE(pawn))
                                    {
                                        ThingToGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position,
                                            pawn.Map, FElist, PathEndMode.OnCell, traverseParams, FWSearchRange,
                                            ValidatorFe);
                                    }

                                    if (ThingToGrab == null && !FWGotFB(pawn))
                                    {
                                        ThingToGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position,
                                            pawn.Map, FBlist, PathEndMode.OnCell, traverseParams, FWSearchRange,
                                            ValidatorFb);
                                    }
                                }
                                else if (!FWGotFB(pawn))
                                {
                                    ThingToGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map,
                                        FBlist, PathEndMode.OnCell, traverseParams, FWSearchRange, ValidatorFb);
                                }
                            }

                            if (ThingToGrab != null &&
                                pawn.inventory.innerContainer.CanAcceptAnyOf(ThingToGrab, false))
                            {
                                return new ThinkResult(new Job(FWEquipJobs.FWEquipping, ThingToGrab), this);
                            }

                            result = ThinkResult.NoJob;
                        }
                    }
                }
            }
        }

        return result;
    }

    private static bool FWGotFE(Pawn p)
    {
        if (p.equipment.Primary != null && p.equipment.Primary.def.defName == FEDefName &&
            FWFoamUtility.HasFEFoam(p.equipment.Primary))
        {
            return true;
        }

        if (p.inventory.innerContainer.NullOrEmpty())
        {
            return false;
        }

        foreach (var invFECheck in p.inventory.innerContainer)
        {
            if (invFECheck.def.defName != FEDefName)
            {
                continue;
            }

            if ((invFECheck as FireWardenData)?.FWSwapType != "N")
            {
                FWResetVars((ThingWithComps)invFECheck);
            }

            return true;
        }

        return false;
    }

    private static bool FWGotFB(Pawn p)
    {
        if (p.equipment.Primary != null && p.equipment.Primary.def.defName == FBDefName)
        {
            return true;
        }

        if (p.inventory.innerContainer.NullOrEmpty())
        {
            return false;
        }

        foreach (var invFECheck in p.inventory.innerContainer)
        {
            if (invFECheck.def.defName != FBDefName)
            {
                continue;
            }

            if ((invFECheck as FireWardenData)?.FWSwapType != "N")
            {
                FWResetVars((ThingWithComps)invFECheck);
            }

            return true;
        }

        return false;
    }

    private static void FWResetVars(ThingWithComps thingWC)
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