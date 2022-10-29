using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobDriver_PelFESimple : JobDriver
{
    private static readonly string FEDefName = "Gun_Fire_Ext";

    private static readonly string FBDefName = "Firebeater";

    private static readonly bool DebugFWData = false;

    private Fire TargetFire => (Fire)job.targetA.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    private static bool FWHasFE(Pawn p)
    {
        if (p.equipment.Primary != null)
        {
            if (p.equipment.Primary.def.defName == FEDefName && FWFoamUtility.HasFEFoam(p.equipment.Primary))
            {
                return true;
            }
        }
        else if (!p.inventory.innerContainer.NullOrEmpty())
        {
            foreach (var invFECheck in p.inventory.innerContainer)
            {
                if (invFECheck.def.defName == FEDefName && FWFoamUtility.HasFEFoam(invFECheck))
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    private static bool FWHasFB(Pawn p)
    {
        if (p.equipment.Primary != null)
        {
            if (p.equipment.Primary.def.defName == FBDefName)
            {
                return true;
            }
        }
        else if (!p.inventory.innerContainer.NullOrEmpty())
        {
            using var enumerator = p.inventory.innerContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current?.def.defName == FBDefName)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedOrNull(TargetIndex.A);
        var toilInv = new Toil();
        var toilEquipGoto = new Toil();
        var toilEquip = new Toil();
        var toilGoto = new Toil();
        var toilCast = new Toil();
        var toilTouch = new Toil();
        var toilBeat = new Toil();
        var toilBash = new Toil();
        var HasPrimFE = false;
        var HasPrimFB = false;
        if (pawn.equipment.Primary != null)
        {
            if (pawn.equipment.Primary.def.defName == FEDefName && FWFoamUtility.HasFEFoam(pawn.equipment.Primary))
            {
                HasPrimFE = true;
            }
            else if (pawn.equipment.Primary.def.defName == FBDefName)
            {
                HasPrimFB = true;
            }
        }

        if (!HasPrimFE)
        {
            var fb = HasPrimFB;
            toilInv.initAction = delegate
            {
                var Swap = false;
                ThingWithComps invGearToEquip2 = null;
                ThingWithComps primToSwap2 = null;
                Thing RemoveThing = null;
                Thing BackupThing2 = null;
                if (pawn.equipment.Primary != null)
                {
                    primToSwap2 = pawn.equipment.Primary;
                }

                foreach (var invThing2 in pawn.inventory.innerContainer)
                {
                    if (invThing2.def.defName != FEDefName || !FWFoamUtility.HasFEFoam(invThing2))
                    {
                        if (invThing2.def.defName == FBDefName)
                        {
                            BackupThing2 = invThing2;
                        }
                    }
                    else
                    {
                        RemoveThing = invThing2;
                        invGearToEquip2 = (ThingWithComps)invThing2;
                        if (primToSwap2 != null)
                        {
                            Swap = true;
                        }

                        break;
                    }
                }

                if (invGearToEquip2 == null && !fb && BackupThing2 != null)
                {
                    RemoveThing = BackupThing2;
                    invGearToEquip2 = (ThingWithComps)BackupThing2;
                    if (primToSwap2 != null)
                    {
                        Swap = true;
                    }
                }

                if (invGearToEquip2 == null)
                {
                    return;
                }

                var primDef = "";
                if (Swap)
                {
                    primDef = pawn.equipment.Primary.def.defName;
                    pawn.equipment.Remove(pawn.equipment.Primary);
                }

                pawn.inventory.innerContainer.Remove(RemoveThing);
                pawn.equipment.MakeRoomFor(invGearToEquip2);
                pawn.equipment.AddEquipment(invGearToEquip2);
                if (Swap)
                {
                    pawn.inventory.innerContainer.TryAdd(primToSwap2);
                }

                if (!Swap)
                {
                    return;
                }

                var returnType = "SI";
                if (pawn.equipment.Primary.def.defName != FEDefName &&
                    pawn.equipment.Primary.def.defName != FBDefName)
                {
                    return;
                }

                var primary = pawn.equipment.Primary;
                ((FireWardenData)primary).FWSwapType = returnType;
                ((FireWardenData)primary).FWPawnID = pawn.thingIDNumber;
                ((FireWardenData)primary).FWPrimDef = primDef;
                if (!DebugFWData)
                {
                    return;
                }

                var Test = pawn.equipment.Primary;
                var debugTest = $"{pawn.Label} : ";
                debugTest = $"{debugTest}{Test.Label} : ";
                debugTest = $"{debugTest}{pawn.equipment.Primary.GetType()} : ";
                if (((FireWardenData)Test).FWSwapType != null)
                {
                    debugTest = $"{debugTest}{((FireWardenData)Test).FWSwapType} : ";
                }
                else
                {
                    debugTest += "null : ";
                }

                debugTest = $"{debugTest}{((FireWardenData)Test).FWPawnID} : ";
                if (((FireWardenData)Test).FWPrimDef != null)
                {
                    debugTest += ((FireWardenData)Test).FWPrimDef;
                }
                else
                {
                    debugTest += "null";
                }

                Messages.Message(debugTest, pawn, MessageTypeDefOf.NeutralEvent, false);
            };
            toilInv.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
            yield return toilInv;
        }

        var FWEquipping = Controller.Settings.EquippingDone;
        var FWSearchRange = (float)Controller.Settings.SearchRange;
        if (FWSearchRange < 25f)
        {
            FWSearchRange = 25f;
        }

        if (FWSearchRange > 75f)
        {
            FWSearchRange = 75f;
        }

        HasPrimFE = false;
        HasPrimFB = false;
        if (pawn.equipment.Primary != null)
        {
            if (pawn.equipment.Primary.def.defName == FEDefName && FWFoamUtility.HasFEFoam(pawn.equipment.Primary))
            {
                HasPrimFE = true;
            }
            else if (pawn.equipment.Primary.def.defName == FBDefName)
            {
                HasPrimFB = true;
            }
        }

        if (!HasPrimFE && !HasPrimFB && FWEquipping)
        {
            ThingWithComps invGearToEquip = null;
            ThingWithComps primToSwap = null;
            Thing BackupThing = null;
            if (pawn.equipment.Primary != null)
            {
                primToSwap = pawn.equipment.Primary;
            }

            foreach (var invThing in pawn.inventory.innerContainer)
            {
                if (invThing.def.defName == FEDefName && FWFoamUtility.HasFEFoam(invThing))
                {
                    invGearToEquip = (ThingWithComps)invThing;
                    if (primToSwap != null)
                    {
                    }

                    break;
                }

                if (invThing.def.defName == FBDefName)
                {
                    BackupThing = invThing;
                }
            }

            if (invGearToEquip == null && BackupThing != null)
            {
                invGearToEquip = (ThingWithComps)BackupThing;
            }

            if (invGearToEquip == null)
            {
                Thing ThingToGrab = null;
                var skip = Controller.Settings.BrawlerNotOK && pawn.story.traits.HasTrait(TraitDefOf.Brawler);
                var traverseParams = TraverseParms.For(pawn);

                bool validatorFE(Thing t)
                {
                    return !t.IsForbidden(pawn) && pawn.CanReserve(t) && FWFoamUtility.HasFEFoam(t) &&
                           !FWFoamUtility.ReplaceFEFoam(t);
                }

                bool validatorFB(Thing t)
                {
                    return !t.IsForbidden(pawn) && pawn.CanReserve(t);
                }

                if (!skip)
                {
                    var FElist = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FEDefName));
                    var FEGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, FElist,
                        PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFE);
                    if (FEGrab != null)
                    {
                        ThingToGrab = FEGrab;
                    }
                    else
                    {
                        var FBlist = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FBDefName));
                        var FBGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, FBlist,
                            PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFB);
                        if (FBGrab != null)
                        {
                            ThingToGrab = FBGrab;
                        }
                    }
                }
                else
                {
                    var FBlist2 = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FBDefName));
                    var FBGrab2 = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, FBlist2,
                        PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFB);
                    if (FBGrab2 != null)
                    {
                        ThingToGrab = FBGrab2;
                    }
                }

                if (ThingToGrab != null)
                {
                    toilEquipGoto.initAction = delegate
                    {
                        if (Map.reservationManager.CanReserve(pawn, ThingToGrab))
                        {
                            pawn.Reserve(ThingToGrab, job);
                        }

                        pawn.pather.StartPath(ThingToGrab, PathEndMode.OnCell);
                    };
                    toilEquipGoto.FailOn(ThingToGrab.DestroyedOrNull);
                    toilEquipGoto.AddFailCondition(() => FWHasFE(pawn) && ThingToGrab.def.defName == FEDefName);
                    toilEquipGoto.AddFailCondition(() => FWHasFB(pawn) && ThingToGrab.def.defName == FBDefName);
                    toilEquipGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
                    yield return toilEquipGoto;
                    toilEquip.initAction = delegate
                    {
                        var primDeEquip = pawn.equipment.Primary;
                        var primDef = "N";
                        if (primDeEquip != null)
                        {
                            primDef = pawn.equipment.Primary.def.defName;
                            pawn.equipment.Remove(pawn.equipment.Primary);
                            pawn.inventory.innerContainer.TryAdd(primDeEquip);
                        }

                        var FWGrabWithComps = (ThingWithComps)ThingToGrab;
                        ThingWithComps FWGrabbed;
                        if (FWGrabWithComps.def.stackLimit > 1 && FWGrabWithComps.stackCount > 1)
                        {
                            FWGrabbed = (ThingWithComps)FWGrabWithComps.SplitOff(1);
                        }
                        else
                        {
                            FWGrabbed = FWGrabWithComps;
                            FWGrabbed.DeSpawn();
                        }

                        pawn.equipment.MakeRoomFor(FWGrabbed);
                        pawn.equipment.AddEquipment(FWGrabbed);
                        var returnType = "EN";
                        if (pawn.equipment.Primary.def.defName != FEDefName &&
                            pawn.equipment.Primary.def.defName != FBDefName)
                        {
                            return;
                        }

                        var primary = pawn.equipment.Primary;
                        ((FireWardenData)primary).FWSwapType = returnType;
                        ((FireWardenData)primary).FWPawnID = pawn.thingIDNumber;
                        ((FireWardenData)primary).FWPrimDef = primDef;
                        if (!DebugFWData)
                        {
                            return;
                        }

                        var Test = pawn.equipment.Primary;
                        var debugTest = $"{pawn.Label} : ";
                        debugTest = $"{debugTest}{Test.Label} : ";
                        debugTest = $"{debugTest}{pawn.equipment.Primary.GetType()} : ";
                        if ((Test as FireWardenData)?.FWSwapType != null)
                        {
                            debugTest = $"{debugTest}{((FireWardenData)Test).FWSwapType} : ";
                        }
                        else
                        {
                            debugTest += "null : ";
                        }

                        debugTest = $"{debugTest}{((FireWardenData)Test).FWPawnID} : ";
                        if (((FireWardenData)Test).FWPrimDef != null)
                        {
                            debugTest += ((FireWardenData)Test).FWPrimDef;
                        }
                        else
                        {
                            debugTest += "null";
                        }

                        Messages.Message(debugTest, pawn, MessageTypeDefOf.NeutralEvent, false);
                    };
                    toilEquip.AddFailCondition(() => FWHasFE(pawn) && ThingToGrab.def.defName == FEDefName);
                    toilEquip.AddFailCondition(() => FWHasFB(pawn) && ThingToGrab.def.defName == FBDefName);
                    toilEquip.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
                    yield return toilEquip;
                }
            }
        }

        var HasPrimFEEq = pawn.equipment.Primary != null && pawn.equipment.Primary.def.defName == FEDefName &&
                          FWFoamUtility.HasFEFoam(pawn.equipment.Primary);
        if (HasPrimFEEq)
        {
            var FEVerbToUse = pawn.TryGetAttackVerb(TargetFire);
            var RangeFireExt = 10f;
            if (FEVerbToUse != null)
            {
                pawn.jobs.curJob.verbToUse = FEVerbToUse;
                RangeFireExt = pawn.jobs.curJob.verbToUse.verbProps.range;
                RangeFireExt *= (float)(Controller.Settings.HowClose / 100.0);
                if (RangeFireExt < 3f)
                {
                    RangeFireExt = 3f;
                }

                if (RangeFireExt > pawn.jobs.curJob.verbToUse.verbProps.range)
                {
                    RangeFireExt = pawn.jobs.curJob.verbToUse.verbProps.range;
                }
            }

            toilGoto.initAction = delegate
            {
                if (Map.reservationManager.CanReserve(pawn, TargetFire))
                {
                    pawn.Reserve(TargetFire, job);
                }

                if (!CastPositionFinder.TryFindCastPosition(new CastPositionRequest
                    {
                        caster = pawn,
                        target = TargetFire,
                        verb = pawn.jobs.curJob.verbToUse,
                        maxRangeFromTarget = RangeFireExt,
                        wantCoverFromTarget = false
                    }, out var dest))
                {
                    toilGoto.actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                    return;
                }

                toilGoto.actor.pather.StartPath(dest, PathEndMode.OnCell);
                pawn.Map.pawnDestinationReservationManager.Reserve(pawn, pawn.jobs.curJob, dest);
            };
            toilGoto.tickAction = delegate
            {
                if (Controller.Settings.TooBrave)
                {
                    return;
                }

                if (pawn.pather.Moving && pawn.pather.nextCell != TargetFire.Position)
                {
                    StartTacklingFireIfAnyAt(pawn.pather.nextCell, toilCast);
                }

                if (pawn.Position != TargetFire.Position)
                {
                    StartTacklingFireIfAnyAt(pawn.Position, toilCast);
                }
            };
            toilGoto.FailOnDespawnedOrNull(TargetIndex.A);
            toilGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toilGoto.atomicWithPrevious = true;
            yield return toilGoto;
            toilCast.initAction = delegate
            {
                pawn.jobs.curJob.verbToUse.TryStartCastOn(TargetFire);
                if (!TargetFire.Destroyed)
                {
                    return;
                }

                pawn.records.Increment(RecordDefOf.FiresExtinguished);
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            };
            toilCast.FailOnDespawnedOrNull(TargetIndex.A);
            toilCast.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
            yield return toilCast;
        }
        else
        {
            toilTouch.initAction = delegate
            {
                if (Map.reservationManager.CanReserve(pawn, TargetFire))
                {
                    pawn.Reserve(TargetFire, job);
                }

                pawn.pather.StartPath(TargetFire, PathEndMode.Touch);
            };
            toilTouch.tickAction = delegate
            {
                if (Controller.Settings.TooBrave)
                {
                    return;
                }

                if (pawn.pather.Moving && pawn.pather.nextCell != TargetFire.Position)
                {
                    StartTacklingFireIfAnyAt(pawn.pather.nextCell, toilBeat);
                }

                if (pawn.Position != TargetFire.Position)
                {
                    StartTacklingFireIfAnyAt(pawn.Position, toilBeat);
                }
            };
            toilTouch.FailOnDespawnedOrNull(TargetIndex.A);
            toilTouch.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toilTouch.atomicWithPrevious = true;
            yield return toilTouch;
            toilBeat.tickAction = delegate
            {
                if (!pawn.CanReachImmediate(TargetFire, PathEndMode.Touch))
                {
                    JumpToToil(toilTouch);
                    return;
                }

                if (pawn.Position != TargetFire.Position && StartTacklingFireIfAnyAt(pawn.Position, toilBeat))
                {
                    return;
                }

                if (pawn.equipment.Primary != null)
                {
                    if (pawn.equipment.Primary.def.defName == FBDefName)
                    {
                        JumpToToil(toilBash);
                    }
                    else
                    {
                        pawn.natives.TryBeatFire(TargetFire);
                    }
                }
                else
                {
                    pawn.natives.TryBeatFire(TargetFire);
                }

                if (!TargetFire.Destroyed)
                {
                    return;
                }

                pawn.records.Increment(RecordDefOf.FiresExtinguished);
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            };
            toilBeat.FailOnDespawnedOrNull(TargetIndex.A);
            toilBeat.defaultCompleteMode = ToilCompleteMode.Never;
            yield return toilBeat;
            if (pawn.equipment.Primary == null || pawn.equipment.Primary.def.defName != FBDefName)
            {
                yield break;
            }

            toilBash.initAction = delegate
            {
                if (TargetFire != null && Map.reservationManager.CanReserve(pawn, TargetFire))
                {
                    pawn.Reserve(TargetFire, job);
                }

                pawn.pather.StopDead();
            };
            toilBash.handlingFacing = true;
            toilBash.tickAction = delegate
            {
                pawn.rotationTracker.FaceTarget(pawn.CurJob.GetTarget(TargetIndex.A));
                if (TargetFire != null)
                {
                    pawn.Drawer.Notify_MeleeAttackOn(TargetFire);
                }
            };
            toilBash.PlaySoundAtStart(SoundDefOf.Interact_BeatFire);
            toilBash.WithProgressBarToilDelay(TargetIndex.A);
            toilBash.AddFinishAction(delegate
            {
                if (TargetFire is { Destroyed: false })
                {
                    TargetFire.Destroy();
                }
            });
            toilBash.FailOnDespawnedOrNull(TargetIndex.A);
            toilBash.defaultCompleteMode = ToilCompleteMode.Delay;
            var ticks = 50;
            var WorkSpeed = pawn.GetStatValue(StatDefOf.WorkSpeedGlobal);
            if (WorkSpeed <= 0f)
            {
                WorkSpeed = 1f;
            }

            ticks = (int)(ticks * (1f / WorkSpeed));
            if (ticks < 25)
            {
                ticks = 25;
            }

            if (ticks > 200)
            {
                ticks = 200;
            }

            toilBash.defaultDuration = ticks;
            yield return toilBash;
        }
    }

    private bool StartTacklingFireIfAnyAt(IntVec3 cell, Toil nextToil)
    {
        var thingList = cell.GetThingList(pawn.Map);
        foreach (var thing in thingList)
        {
            if (thing is not Fire { parent: null } fire)
            {
                continue;
            }

            job.targetA = fire;
            pawn.pather.StopDead();
            JumpToToil(nextToil);
            return true;
        }

        return false;
    }
}