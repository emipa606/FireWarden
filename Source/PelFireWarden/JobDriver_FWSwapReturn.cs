using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000016 RID: 22
	public class JobDriver_FWSwapReturn : JobDriver
	{
		// Token: 0x06000059 RID: 89 RVA: 0x00003FBF File Offset: 0x000021BF
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003FC4 File Offset: 0x000021C4
		public bool FWHasFE(Pawn p)
		{
			if (p.equipment.Primary != null && p.equipment.Primary.def.defName == JobDriver_FWSwapReturn.FEDefName && (p.equipment.Primary as FireWardenData).FWSwapType == "ER" && (p.equipment.Primary as FireWardenData).FWPawnID == p.thingIDNumber)
			{
				return true;
			}
			if (this.pawn.carryTracker.CarriedThing == this.job.GetTarget(TargetIndex.A).Thing)
			{
				return true;
			}
			foreach (Thing invThing in this.pawn.inventory.innerContainer)
			{
				if (invThing.def.defName == JobDriver_FWSwapReturn.FEDefName)
				{
					ThingWithComps invGearFE = (ThingWithComps)invThing;
					if ((invGearFE as FireWardenData).FWSwapType == "ER" && (invGearFE as FireWardenData).FWPawnID == p.thingIDNumber)
					{
						return true;
					}
				}
			}
			return false;
		}


		// Token: 0x0600005C RID: 92 RVA: 0x00004114 File Offset: 0x00002314
		public void FEResetVars(ThingWithComps thingWC)
		{
			if (thingWC != null && thingWC.def.defName == JobDriver_FWSwapReturn.FEDefName)
			{
				(thingWC as FireWardenData).FWSwapType = "N";
				(thingWC as FireWardenData).FWPawnID = 0;
				(thingWC as FireWardenData).FWPrimDef = "N";
			}
		}

		// Token: 0x04000028 RID: 40
		public static string FEDefName = "Gun_Fire_Ext";

		// Token: 0x04000029 RID: 41
		public static bool FWDebug = true;

		// Token: 0x0600005B RID: 91 RVA: 0x00004104 File Offset: 0x00002304
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toilSwapback = new Toil();
			Toil toilReturnStock = new Toil();
			Toil toilDrop = new Toil();
			Toil toilReturnHome = new Toil();
			Toil toilDropHome = new Toil();
			ThingWithComps FEDroppedtemp = null;
			Thing resultingThing = null;
			ThingWithComps FETakeBack = (ThingWithComps)this.job.GetTarget(TargetIndex.A).Thing;
			string FETBSwapType = (FETakeBack as FireWardenData).FWSwapType;
			string DebugMsg = "";
			toilSwapback.initAction = delegate ()
			{
				if (this.FWHasFE(pawn) && resultingThing != null)
				{
					if (JobDriver_FWSwapReturn.FWDebug)
					{
						DebugMsg = pawn.Label + " in toil Swapback";
						Messages.Message(DebugMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
					}
					if (job.GetTarget(TargetIndex.B) != null)
					{
						Thing RemoveThing = job.GetTarget(TargetIndex.B).Thing;
						ThingWithComps invGearToEquip = (ThingWithComps)RemoveThing;
						if (pawn.inventory.innerContainer.Contains(RemoveThing) && pawn.equipment.Primary != invGearToEquip)
						{
							pawn.inventory.innerContainer.Remove(RemoveThing);
							pawn.equipment.MakeRoomFor(invGearToEquip);
							pawn.equipment.AddEquipment(invGearToEquip);
						}
					}
					FETakeBack = (ThingWithComps)job.GetTarget(TargetIndex.A).Thing;
					FEResetVars(FETakeBack);
					FETBSwapType = (FETakeBack as FireWardenData).FWSwapType;
					pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				}
			};
			toilSwapback.AddFailCondition(() => FETBSwapType != "ER");
			toilSwapback.FailOnDespawnedOrNull(TargetIndex.A);
			toilSwapback.FailOnDespawnedOrNull(TargetIndex.B);
			toilSwapback.defaultCompleteMode = ToilCompleteMode.Delay;
			yield return toilSwapback;
			if (this.pawn.equipment.Primary != null && this.pawn.equipment.Primary.def.defName == JobDriver_FWSwapReturn.FEDefName && (this.pawn.equipment.Primary as FireWardenData).FWSwapType == "ER" && (this.pawn.equipment.Primary as FireWardenData).FWPawnID == this.pawn.thingIDNumber)
			{
				StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(this.job.GetTarget(TargetIndex.A).Thing);
				bool dumpAtHome = false;
				if (Controller.Settings.ReturnToSpot)
				{
					dumpAtHome = true;
				}
				else
				{
					if (this.pawn.equipment.Primary == (ThingWithComps)this.job.GetTarget(TargetIndex.A).Thing)
					{
						this.pawn.equipment.TryDropEquipment(this.pawn.equipment.Primary, out FEDroppedtemp, this.pawn.Position, false);
						this.pawn.carryTracker.TryStartCarry(this.job.GetTarget(TargetIndex.A).Thing, 1, true);
					}
                    if (!StoreUtility.TryFindBestBetterStorageFor(this.job.GetTarget(TargetIndex.A).Thing, this.pawn, this.pawn.Map, currentPriority, this.pawn.Faction, out IntVec3 foundCell, out IHaulDestination haulDestination, true))
                    {
                        dumpAtHome = true;
                    }
                    else
                    {
                        toilReturnStock.initAction = delegate ()
                        {
                            if (JobDriver_FWSwapReturn.FWDebug)
                            {
                                DebugMsg = pawn.Label + " in toil ReturnStock";
                                Messages.Message(DebugMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
                            }
                            if (pawn.carryTracker.CarriedThing == job.GetTarget(TargetIndex.A).Thing)
                            {
                                IntVec3 destCell = haulDestination.Position;
                                pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, destCell);
                                pawn.pather.StartPath(destCell, PathEndMode.ClosestTouch);
                            }
                        };
                        toilReturnStock.AddFailCondition(() => FETBSwapType != "ER");
                        toilReturnStock.FailOnDespawnedOrNull(TargetIndex.A);
                        toilReturnStock.defaultCompleteMode = ToilCompleteMode.PatherArrival;
                        yield return toilReturnStock;
                        toilDrop.initAction = delegate ()
                        {
                            if ((haulDestination.Position - pawn.Position).LengthHorizontal <= 2f)
                            {
                                if (pawn.carryTracker.CarriedThing == job.GetTarget(TargetIndex.A).Thing)
                                {
                                    if (pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out resultingThing, null))
                                    {
                                        dumpAtHome = false;
                                        if (job.GetTarget(TargetIndex.B) == null)
                                        {
                                            FETakeBack = (ThingWithComps)job.GetTarget(TargetIndex.A).Thing;
                                            FEResetVars(FETakeBack);
                                            FETBSwapType = (FETakeBack as FireWardenData).FWSwapType;
                                            pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    dumpAtHome = true;
                                }
                            }
                        };
                        toilSwapback.AddFailCondition(() => FETBSwapType != "ER");
                        toilDrop.FailOnDespawnedOrNull(TargetIndex.A);
                        toilDrop.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
                        yield return toilDrop;
                    }
                }
				if (dumpAtHome)
				{
					IntVec3 ReturnCell = this.pawn.Position;
					if (false)
					{
						toilReturnHome.initAction = delegate ()
						{
							if (JobDriver_FWSwapReturn.FWDebug)
							{
								DebugMsg = pawn.Label + " in toil ReturnHome";
								Messages.Message(DebugMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
							}
							if (pawn.equipment.Primary == (ThingWithComps)job.GetTarget(TargetIndex.A).Thing)
							{
								pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out FEDroppedtemp, pawn.Position, false);
								pawn.carryTracker.TryStartCarry(job.GetTarget(TargetIndex.A).Thing, 1, true);
							}
							if (pawn.carryTracker.CarriedThing == job.GetTarget(TargetIndex.A).Thing)
							{
								pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, ReturnCell);
								pawn.pather.StartPath(ReturnCell, PathEndMode.ClosestTouch);
							}
						};
						toilReturnHome.AddFailCondition(() => FETBSwapType != "ER");
						toilReturnHome.FailOnDespawnedOrNull(TargetIndex.A);
						toilReturnHome.defaultCompleteMode = ToilCompleteMode.PatherArrival;
						yield return toilReturnHome;
					}
					else if (this.pawn.equipment.Primary == (ThingWithComps)this.job.GetTarget(TargetIndex.A).Thing)
					{
						this.pawn.equipment.TryDropEquipment(this.pawn.equipment.Primary, out FEDroppedtemp, this.pawn.Position, false);
						this.pawn.carryTracker.TryStartCarry(this.job.GetTarget(TargetIndex.A).Thing, 1, true);
					}
					toilDropHome.initAction = delegate ()
					{
					if ((ReturnCell - pawn.Position).LengthHorizontal <= 2f && pawn.carryTracker.CarriedThing == job.GetTarget(TargetIndex.A).Thing && pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out resultingThing, null) && job.GetTarget(TargetIndex.B) == null)
						{
						FETakeBack = (ThingWithComps)job.GetTarget(TargetIndex.A).Thing;
						FEResetVars(FETakeBack);
						FETBSwapType = (FETakeBack as FireWardenData).FWSwapType;
						pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
						}
					};
					toilDropHome.AddFailCondition(() => FETBSwapType != "ER");
					toilDropHome.FailOnDespawnedOrNull(TargetIndex.A);
					toilDropHome.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
					yield return toilDropHome;
				}
			}
			yield break;
		}
	}
}
