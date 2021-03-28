using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
    // Token: 0x02000016 RID: 22
    public class JobDriver_FWSwapReturn : JobDriver
    {
        // Token: 0x04000028 RID: 40
        private static readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x04000029 RID: 41
        private static readonly bool FWDebug = true;

        // Token: 0x06000059 RID: 89 RVA: 0x00003FBF File Offset: 0x000021BF
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x0600005A RID: 90 RVA: 0x00003FC4 File Offset: 0x000021C4
        private bool FWHasFE(Pawn p)
        {
            if (p.equipment.Primary != null && p.equipment.Primary.def.defName == FEDefName &&
                (p.equipment.Primary as FireWardenData)?.FWSwapType == "ER" &&
                ((FireWardenData) p.equipment.Primary).FWPawnID == p.thingIDNumber)
            {
                return true;
            }

            if (pawn.carryTracker.CarriedThing == job.GetTarget(TargetIndex.A).Thing)
            {
                return true;
            }

            foreach (var invThing in pawn.inventory.innerContainer)
            {
                if (invThing.def.defName != FEDefName)
                {
                    continue;
                }

                var invGearFE = (ThingWithComps) invThing;
                if ((invGearFE as FireWardenData)?.FWSwapType == "ER" &&
                    (invGearFE as FireWardenData).FWPawnID == p.thingIDNumber)
                {
                    return true;
                }
            }

            return false;
        }


        // Token: 0x0600005C RID: 92 RVA: 0x00004114 File Offset: 0x00002314
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

        // Token: 0x0600005B RID: 91 RVA: 0x00004104 File Offset: 0x00002304
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var toilSwapback = new Toil();
            var toilReturnStock = new Toil();
            var toilDrop = new Toil();
            var toilDropHome = new Toil();
            Thing resultingThing = null;
            var FETakeBack = (ThingWithComps) job.GetTarget(TargetIndex.A).Thing;
            var FETBSwapType = (FETakeBack as FireWardenData)?.FWSwapType;
            var DebugMsg = "";
            toilSwapback.initAction = delegate
            {
                if (!FWHasFE(pawn) || resultingThing == null)
                {
                    return;
                }

                if (FWDebug)
                {
                    DebugMsg = pawn.Label + " in toil Swapback";
                    Messages.Message(DebugMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
                }

                if (job.GetTarget(TargetIndex.B) != null)
                {
                    var RemoveThing = job.GetTarget(TargetIndex.B).Thing;
                    var invGearToEquip = (ThingWithComps) RemoveThing;
                    if (pawn.inventory.innerContainer.Contains(RemoveThing) && pawn.equipment.Primary != invGearToEquip)
                    {
                        pawn.inventory.innerContainer.Remove(RemoveThing);
                        pawn.equipment.MakeRoomFor(invGearToEquip);
                        pawn.equipment.AddEquipment(invGearToEquip);
                    }
                }

                FETakeBack = (ThingWithComps) job.GetTarget(TargetIndex.A).Thing;
                FEResetVars(FETakeBack);
                FETBSwapType = (FETakeBack as FireWardenData)?.FWSwapType;
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            };
            toilSwapback.AddFailCondition(() => FETBSwapType != "ER");
            toilSwapback.FailOnDespawnedOrNull(TargetIndex.A);
            toilSwapback.FailOnDespawnedOrNull(TargetIndex.B);
            toilSwapback.defaultCompleteMode = ToilCompleteMode.Delay;
            yield return toilSwapback;
            if (pawn.equipment.Primary == null || pawn.equipment.Primary.def.defName != FEDefName ||
                (pawn.equipment.Primary as FireWardenData)?.FWSwapType != "ER" ||
                ((FireWardenData) pawn.equipment.Primary).FWPawnID != pawn.thingIDNumber)
            {
                yield break;
            }

            var currentPriority = StoreUtility.CurrentStoragePriorityOf(job.GetTarget(TargetIndex.A).Thing);
            var dumpAtHome = false;
            if (Controller.Settings.ReturnToSpot)
            {
                dumpAtHome = true;
            }
            else
            {
                if (pawn.equipment.Primary == (ThingWithComps) job.GetTarget(TargetIndex.A).Thing)
                {
                    pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out _, pawn.Position, false);
                    pawn.carryTracker.TryStartCarry(job.GetTarget(TargetIndex.A).Thing, 1);
                }

                if (!StoreUtility.TryFindBestBetterStorageFor(job.GetTarget(TargetIndex.A).Thing, pawn, pawn.Map,
                    currentPriority, pawn.Faction, out _, out var haulDestination))
                {
                    dumpAtHome = true;
                }
                else
                {
                    toilReturnStock.initAction = delegate
                    {
                        if (FWDebug)
                        {
                            DebugMsg = pawn.Label + " in toil ReturnStock";
                            Messages.Message(DebugMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
                        }

                        if (pawn.carryTracker.CarriedThing != job.GetTarget(TargetIndex.A).Thing)
                        {
                            return;
                        }

                        var destCell = haulDestination.Position;
                        pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, destCell);
                        pawn.pather.StartPath(destCell, PathEndMode.ClosestTouch);
                    };
                    toilReturnStock.AddFailCondition(() => FETBSwapType != "ER");
                    toilReturnStock.FailOnDespawnedOrNull(TargetIndex.A);
                    toilReturnStock.defaultCompleteMode = ToilCompleteMode.PatherArrival;
                    yield return toilReturnStock;
                    toilDrop.initAction = delegate
                    {
                        if (!((haulDestination.Position - pawn.Position).LengthHorizontal <= 2f))
                        {
                            return;
                        }

                        if (pawn.carryTracker.CarriedThing == job.GetTarget(TargetIndex.A).Thing)
                        {
                            if (!pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near,
                                out resultingThing))
                            {
                                return;
                            }

                            dumpAtHome = false;
                            if (job.GetTarget(TargetIndex.B) != null)
                            {
                                return;
                            }

                            FETakeBack = (ThingWithComps) job.GetTarget(TargetIndex.A).Thing;
                            FEResetVars(FETakeBack);
                            FETBSwapType = (FETakeBack as FireWardenData)?.FWSwapType;
                            pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                        }
                        else
                        {
                            dumpAtHome = true;
                        }
                    };
                    toilSwapback.AddFailCondition(() => FETBSwapType != "ER");
                    toilDrop.FailOnDespawnedOrNull(TargetIndex.A);
                    toilDrop.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
                    yield return toilDrop;
                }
            }

            if (!dumpAtHome)
            {
                yield break;
            }

            var ReturnCell = pawn.Position;
//            if (false)
//            {
//                /*
//                            {
//                                toilReturnHome.initAction = delegate
//                                {
//                                    if (FWDebug)
//                                    {
//                                        DebugMsg = pawn.Label + " in toil ReturnHome";
//                                        Messages.Message(DebugMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
//                                    }
//                                    if (pawn.equipment.Primary == (ThingWithComps)job.GetTarget(TargetIndex.A).Thing)
//                                    {
//                                        pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out _, pawn.Position, false);
//                                        pawn.carryTracker.TryStartCarry(job.GetTarget(TargetIndex.A).Thing, 1);
//                                    }
//
//                                    if (pawn.carryTracker.CarriedThing != job.GetTarget(TargetIndex.A).Thing)
//                                    {
//                                        return;
//                                    }
//
//                                    pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, ReturnCell);
//                                    pawn.pather.StartPath(ReturnCell, PathEndMode.ClosestTouch);
//                                };
//                                toilReturnHome.AddFailCondition(() => FETBSwapType != "ER");
//                                toilReturnHome.FailOnDespawnedOrNull(TargetIndex.A);
//                                toilReturnHome.defaultCompleteMode = ToilCompleteMode.PatherArrival;
//                                yield return toilReturnHome;
//                            }
//                */
//            }
            if (pawn.equipment.Primary == (ThingWithComps) job.GetTarget(TargetIndex.A).Thing)
            {
                pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out _, pawn.Position, false);
                pawn.carryTracker.TryStartCarry(job.GetTarget(TargetIndex.A).Thing, 1);
            }

            toilDropHome.initAction = delegate
            {
                if (!((ReturnCell - pawn.Position).LengthHorizontal <= 2f) ||
                    pawn.carryTracker.CarriedThing != job.GetTarget(TargetIndex.A).Thing ||
                    !pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out resultingThing) ||
                    job.GetTarget(TargetIndex.B) != null)
                {
                    return;
                }

                FETakeBack = (ThingWithComps) job.GetTarget(TargetIndex.A).Thing;
                FEResetVars(FETakeBack);
                FETBSwapType = (FETakeBack as FireWardenData)?.FWSwapType;
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            };
            toilDropHome.AddFailCondition(() => FETBSwapType != "ER");
            toilDropHome.FailOnDespawnedOrNull(TargetIndex.A);
            toilDropHome.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
            yield return toilDropHome;
        }
    }
}