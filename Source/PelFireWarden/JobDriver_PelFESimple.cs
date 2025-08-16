using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobDriver_PelFESimple : JobDriver
{
    private const string ExtinguisherDefName = "Gun_Fire_Ext";
    private const string FireBeaterDefName = "Firebeater";

    private const bool DebugFwData = false;
    private ThingWithComps extinguisherCached;
    private bool fwTriedCastOnce;

    private Fire TargetFire => (Fire)job.targetA.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    private static ThingWithComps findExtinguisher(Pawn p)
    {
        if (p.equipment.Primary != null &&
            p.equipment.Primary.def.defName == ExtinguisherDefName &&
            FWFoamUtility.HasFEFoam(p.equipment.Primary))
        {
            return p.equipment.Primary;
        }

        if (p.inventory.innerContainer.NullOrEmpty())
        {
            return null;
        }

        foreach (var item in p.inventory.innerContainer)
        {
            if (item.def.defName == ExtinguisherDefName && FWFoamUtility.HasFEFoam(item))
            {
                return (ThingWithComps)item;
            }
        }

        return null;
    }

    private static bool fwHasFe(Pawn p)
    {
        return findExtinguisher(p) != null;
    }

    private static bool fwHasFb(Pawn p)
    {
        if (p.equipment.Primary != null)
        {
            if (p.equipment.Primary.def.defName == FireBeaterDefName)
            {
                return true;
            }
        }
        else if (!p.inventory.innerContainer.NullOrEmpty())
        {
            foreach (var item in p.inventory.innerContainer)
            {
                if (item?.def.defName == FireBeaterDefName)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        fwTriedCastOnce = false;

        this.FailOnDespawnedOrNull(TargetIndex.A);

        var toil = new Toil();
        var toilEquipGoto = new Toil();
        var toilEquip = new Toil();
        var toilTouch = new Toil();
        var toilBeat = new Toil();
        var toilBash = new Toil();

        var hasExtinguisher = false;
        var hasFireBeater = false;
        if (pawn.equipment.Primary != null)
        {
            switch (pawn.equipment.Primary.def.defName)
            {
                case ExtinguisherDefName when FWFoamUtility.HasFEFoam(pawn.equipment.Primary):
                    hasExtinguisher = true;
                    break;
                case FireBeaterDefName:
                    hasFireBeater = true;
                    break;
            }
        }

        if (!hasExtinguisher)
        {
            var fireBeater = hasFireBeater;
            toil.initAction = delegate
            {
                var hasPrimaryWeapon = false;
                ThingWithComps inventoryItemWithComps = null;
                ThingWithComps primaryWeapon = null;
                Thing item = null;
                Thing fireBeaterItem = null;
                if (pawn.equipment.Primary != null)
                {
                    primaryWeapon = pawn.equipment.Primary;
                }

                foreach (var inventoryItem in pawn.inventory.innerContainer)
                {
                    if (inventoryItem.def.defName == ExtinguisherDefName && FWFoamUtility.HasFEFoam(inventoryItem))
                    {
                        item = inventoryItem;
                        inventoryItemWithComps = (ThingWithComps)inventoryItem;
                        if (primaryWeapon != null)
                        {
                            hasPrimaryWeapon = true;
                        }

                        break;
                    }

                    if (inventoryItem.def.defName == FireBeaterDefName)
                    {
                        fireBeaterItem = inventoryItem;
                    }
                }

                if (inventoryItemWithComps == null && !fireBeater && fireBeaterItem != null)
                {
                    item = fireBeaterItem;
                    inventoryItemWithComps = (ThingWithComps)fireBeaterItem;
                    if (primaryWeapon != null)
                    {
                        hasPrimaryWeapon = true;
                    }
                }

                if (inventoryItemWithComps == null)
                {
                    return;
                }

                var fWPrimDef = "";
                if (hasPrimaryWeapon)
                {
                    fWPrimDef = pawn.equipment.Primary.def.defName;
                    pawn.equipment.Remove(pawn.equipment.Primary);
                }

                pawn.inventory.innerContainer.Remove(item);
                pawn.equipment.MakeRoomFor(inventoryItemWithComps);
                pawn.equipment.AddEquipment(inventoryItemWithComps);
                switch (hasPrimaryWeapon)
                {
                    case true:
                        pawn.inventory.innerContainer.TryAdd(primaryWeapon);
                        break;
                    case false:
                        return;
                }

                const string fWSwapType = "SI";
                if (pawn.equipment.Primary.def.defName != ExtinguisherDefName &&
                    pawn.equipment.Primary.def.defName != FireBeaterDefName)
                {
                    return;
                }

                var primary = pawn.equipment.Primary;
                ((FireWardenData)primary).FWSwapType = fWSwapType;
                ((FireWardenData)primary).FWPawnID = pawn.thingIDNumber;
                ((FireWardenData)primary).FWPrimDef = fWPrimDef;
            };
            toil.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
            yield return toil;
        }

        var equippingDone = Controller.Settings.EquippingDone;
        var num = (float)Controller.Settings.SearchRange;
        if (num < 25f)
        {
            num = 25f;
        }

        if (num > 75f)
        {
            num = 75f;
        }

        hasExtinguisher = false;
        hasFireBeater = false;
        if (pawn.equipment.Primary != null)
        {
            switch (pawn.equipment.Primary.def.defName)
            {
                case ExtinguisherDefName when FWFoamUtility.HasFEFoam(pawn.equipment.Primary):
                    hasExtinguisher = true;
                    break;
                case FireBeaterDefName:
                    hasFireBeater = true;
                    break;
            }
        }

        if (!hasExtinguisher && !hasFireBeater && equippingDone)
        {
            ThingWithComps thingWithComps = null;
            ThingWithComps thingWithComps2 = null;
            Thing thing = null;
            if (pawn.equipment.Primary != null)
            {
                thingWithComps2 = pawn.equipment.Primary;
            }

            foreach (var item3 in pawn.inventory.innerContainer)
            {
                if (item3.def.defName == ExtinguisherDefName && FWFoamUtility.HasFEFoam(item3))
                {
                    thingWithComps = (ThingWithComps)item3;
                    if (thingWithComps2 == null)
                    {
                    }

                    break;
                }

                if (item3.def.defName == FireBeaterDefName)
                {
                    thing = item3;
                }
            }

            if (thingWithComps == null && thing != null)
            {
                thingWithComps = (ThingWithComps)thing;
            }

            if (thingWithComps == null)
            {
                Thing thingToGrab = null;
                var isBrawler = Controller.Settings.BrawlerNotOK && pawn.story.traits.HasTrait(TraitDefOf.Brawler);
                var traverseParams = TraverseParms.For(pawn);
                if (!isBrawler)
                {
                    var searchSet =
                        pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(ExtinguisherDefName));
                    var closestExtinguisher = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map,
                        searchSet,
                        PathEndMode.OnCell, traverseParams, num, validatorFireExtinguisher);
                    if (closestExtinguisher != null)
                    {
                        thingToGrab = closestExtinguisher;
                    }
                    else
                    {
                        var searchSet2 =
                            pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FireBeaterDefName));
                        var closestFireBeater = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map,
                            searchSet2,
                            PathEndMode.OnCell, traverseParams, num, validatorFireBeater);
                        if (closestFireBeater != null)
                        {
                            thingToGrab = closestFireBeater;
                        }
                    }
                }
                else
                {
                    var searchSet3 =
                        pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(FireBeaterDefName));
                    var closestFireBeaterThing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map,
                        searchSet3,
                        PathEndMode.OnCell, traverseParams, num, validatorFireBeater);
                    if (closestFireBeaterThing != null)
                    {
                        thingToGrab = closestFireBeaterThing;
                    }
                }

                if (thingToGrab != null)
                {
                    toilEquipGoto.initAction = delegate
                    {
                        if (Map.reservationManager.CanReserve(pawn, thingToGrab))
                        {
                            pawn.Reserve(thingToGrab, job);
                        }

                        pawn.pather.StartPath(thingToGrab, PathEndMode.OnCell);
                    };
                    toilEquipGoto.FailOn(thingToGrab.DestroyedOrNull);
                    toilEquipGoto.AddFailCondition(() =>
                        fwHasFe(pawn) && thingToGrab.def.defName == ExtinguisherDefName);
                    toilEquipGoto.AddFailCondition(() => fwHasFb(pawn) && thingToGrab.def.defName == FireBeaterDefName);
                    toilEquipGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
                    yield return toilEquipGoto;
                    toilEquip.initAction = delegate
                    {
                        var primary = pawn.equipment.Primary;
                        var fWPrimDef = "N";
                        if (primary != null)
                        {
                            fWPrimDef = pawn.equipment.Primary.def.defName;
                            pawn.equipment.Remove(pawn.equipment.Primary);
                            pawn.inventory.innerContainer.TryAdd(primary);
                        }

                        var thingWithComps3 = (ThingWithComps)thingToGrab;
                        ThingWithComps thingWithComps4;
                        if (thingWithComps3.def.stackLimit > 1 && thingWithComps3.stackCount > 1)
                        {
                            thingWithComps4 = (ThingWithComps)thingWithComps3.SplitOff(1);
                        }
                        else
                        {
                            thingWithComps4 = thingWithComps3;
                            thingWithComps4.DeSpawn();
                        }

                        pawn.equipment.MakeRoomFor(thingWithComps4);
                        pawn.equipment.AddEquipment(thingWithComps4);
                        const string fWSwapType = "EN";
                        if (pawn.equipment.Primary.def.defName != ExtinguisherDefName &&
                            pawn.equipment.Primary.def.defName != FireBeaterDefName)
                        {
                            return;
                        }

                        var primary2 = pawn.equipment.Primary;
                        ((FireWardenData)primary2).FWSwapType = fWSwapType;
                        ((FireWardenData)primary2).FWPawnID = pawn.thingIDNumber;
                        ((FireWardenData)primary2).FWPrimDef = fWPrimDef;
                    };
                    toilEquip.AddFailCondition(() => fwHasFe(pawn) && thingToGrab.def.defName == ExtinguisherDefName);
                    toilEquip.AddFailCondition(() => fwHasFb(pawn) && thingToGrab.def.defName == FireBeaterDefName);
                    toilEquip.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
                    yield return toilEquip;
                }
            }
        }

        extinguisherCached = findExtinguisher(pawn);

        if (extinguisherCached != null)
        {
            var extinguisherVerb = extinguisherCached.GetComp<CompEquippable>()?.PrimaryVerb;

            pawn.jobs.curJob.verbToUse = extinguisherVerb;


            this.FailOn(() => pawn.jobs.curJob.verbToUse == null);

            var verbRange = extinguisherVerb?.verbProps.range ?? 10f;
            var desiredRange = verbRange * (float)(Controller.Settings.HowClose / 100.0);

            if (desiredRange < 3f)
            {
                desiredRange = 3f;
            }

            if (desiredRange > verbRange)
            {
                desiredRange = verbRange;
            }


            var toilGotoFe = new Toil();
            var toilCastFe = new Toil();

            toilGotoFe.initAction = delegate
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
                        maxRangeFromTarget = desiredRange,
                        wantCoverFromTarget = false
                    }, out var dest))
                {
                    toilGotoFe.actor.pather.StartPath(TargetFire, PathEndMode.Touch);
                }
                else
                {
                    toilGotoFe.actor.pather.StartPath(dest, PathEndMode.OnCell);
                    pawn.Map.pawnDestinationReservationManager.Reserve(pawn, pawn.jobs.curJob, dest);
                }
            };
            toilGotoFe.FailOnDespawnedOrNull(TargetIndex.A);
            toilGotoFe.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toilGotoFe.FailOn(() => pawn.jobs.curJob.verbToUse == null);

            yield return toilGotoFe;

            toilCastFe.initAction = () =>
            {
                if (extinguisherCached != null && pawn.equipment.Primary != extinguisherCached)
                {
                    if (pawn.inventory?.innerContainer?.Contains(extinguisherCached) == true)
                    {
                        pawn.inventory.innerContainer.Remove(extinguisherCached);
                    }

                    pawn.equipment.MakeRoomFor(extinguisherCached);
                    pawn.equipment.AddEquipment(extinguisherCached);
                }

                var verbNow = pawn.equipment?.PrimaryEq?.VerbTracker?.PrimaryVerb;


                if (verbNow == null)
                {
                    return;
                }


                if (!verbNow.CanHitTarget(TargetFire))
                {
                    if (fwTriedCastOnce)
                    {
                        return;
                    }

                    fwTriedCastOnce = true;

                    pawn.pather.StartPath(TargetFire, PathEndMode.Touch);

                    return;
                }


                fwTriedCastOnce = false;

                verbNow.TryStartCastOn(TargetFire);
            };
            toilCastFe.FailOnDespawnedOrNull(TargetIndex.A);
            toilCastFe.FailOn(() => pawn.jobs.curJob.verbToUse == null);
            toilCastFe.FailOn(() => pawn.equipment?.PrimaryEq?.VerbTracker?.PrimaryVerb == null);
            toilCastFe.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
            yield return toilCastFe;
            var cleanupFe = new Toil
            {
                initAction = () =>
                {
                    if (extinguisherCached == null || pawn.equipment.Primary != extinguisherCached)
                    {
                        return;
                    }

                    if (pawn.inventory.innerContainer.Contains(extinguisherCached))
                    {
                        return;
                    }

                    pawn.equipment.Remove(extinguisherCached);
                    pawn.inventory.innerContainer.TryAdd(extinguisherCached);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return cleanupFe;

            yield break;
        }

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
                startTacklingFireIfAnyAt(pawn.pather.nextCell, toilBeat);
            }

            if (pawn.Position != TargetFire.Position)
            {
                startTacklingFireIfAnyAt(pawn.Position, toilBeat);
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
            }
            else if (!(pawn.Position != TargetFire.Position) || !startTacklingFireIfAnyAt(pawn.Position, toilBeat))
            {
                if (pawn.equipment.Primary != null)
                {
                    if (pawn.equipment.Primary.def.defName == FireBeaterDefName)
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
            }
        };
        toilBeat.FailOnDespawnedOrNull(TargetIndex.A);
        toilBeat.defaultCompleteMode = ToilCompleteMode.Never;
        yield return toilBeat;
        if (pawn.equipment.Primary == null || pawn.equipment.Primary.def.defName != FireBeaterDefName)
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
            var targetFire = TargetFire;
            if (targetFire is { Destroyed: false })
            {
                TargetFire.Destroy();
            }
        });
        toilBash.FailOnDespawnedOrNull(TargetIndex.A);
        toilBash.defaultCompleteMode = ToilCompleteMode.Delay;
        var num3 = 50;
        var num4 = pawn.GetStatValue(StatDefOf.WorkSpeedGlobal);
        if (num4 <= 0f)
        {
            num4 = 1f;
        }

        num3 = (int)(num3 * (1f / num4));
        if (num3 < 25)
        {
            num3 = 25;
        }

        if (num3 > 200)
        {
            num3 = 200;
        }

        toilBash.defaultDuration = num3;
        yield return toilBash;

        var cleanup = new Toil
        {
            initAction = () => { },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        yield return cleanup;
        yield break;

        bool validatorFireExtinguisher(Thing t)
        {
            if (!t.IsForbidden(pawn) && pawn.CanReserve(t) && FWFoamUtility.HasFEFoam(t))
            {
                return !FWFoamUtility.ReplaceFEFoam(t);
            }

            return false;
        }

        bool validatorFireBeater(Thing t)
        {
            return !t.IsForbidden(pawn) && pawn.CanReserve(t);
        }
    }

    private bool startTacklingFireIfAnyAt(IntVec3 cell, Toil nextToil)
    {
        foreach (var thing in cell.GetThingList(pawn.Map))
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