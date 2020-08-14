using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000017 RID: 23
	public class JobDriver_PelFESimple : JobDriver
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00004181 File Offset: 0x00002381
		protected Fire TargetFire
		{
			get
			{
				return (Fire)this.job.targetA.Thing;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004198 File Offset: 0x00002398
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000419C File Offset: 0x0000239C
		public static bool FWHasFE(Pawn p)
		{
			if (p.equipment.Primary != null)
			{
				if (p.equipment.Primary.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(p.equipment.Primary))
				{
					return true;
				}
			}
			else if (!p.inventory.innerContainer.NullOrEmpty<Thing>())
			{
				foreach (Thing invFECheck in p.inventory.innerContainer)
				{
					if (invFECheck.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(invFECheck))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004268 File Offset: 0x00002468
		public static bool FWHasFB(Pawn p)
		{
			if (p.equipment.Primary != null)
			{
				if (p.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
				{
					return true;
				}
			}
			else if (!p.inventory.innerContainer.NullOrEmpty<Thing>())
			{
				using (List<Thing>.Enumerator enumerator = p.inventory.innerContainer.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.def.defName == JobDriver_PelFESimple.FBDefName)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004318 File Offset: 0x00002518
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			Toil toilInv = new Toil();
			Toil toilEquipGoto = new Toil();
			Toil toilEquip = new Toil();
			Toil toilGoto = new Toil();
			Toil toilCast = new Toil();
			Toil toilTouch = new Toil();
			Toil toilBeat = new Toil();
			Toil toilBash = new Toil();
			bool HasPrimFE = false;
			bool HasPrimFB = false;
			if (this.pawn.equipment.Primary != null)
			{
				if (this.pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(this.pawn.equipment.Primary))
				{
					HasPrimFE = true;
				}
				else if (this.pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
				{
					HasPrimFB = true;
				}
			}
			if (!HasPrimFE)
			{
				toilInv.initAction = delegate()
				{
					bool Swap = false;
					ThingWithComps invGearToEquip2 = null;
					ThingWithComps primToSwap2 = null;
					Thing RemoveThing = null;
					Thing BackupThing2 = null;
					if (pawn.equipment.Primary != null)
					{
						primToSwap2 = pawn.equipment.Primary;
					}
					foreach (Thing invThing2 in pawn.inventory.innerContainer)
					{
						if (invThing2.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(invThing2))
						{
							RemoveThing = invThing2;
							invGearToEquip2 = (ThingWithComps)invThing2;
							if (primToSwap2 != null)
							{
								Swap = true;
								break;
							}
							break;
						}
						else if (invThing2.def.defName == JobDriver_PelFESimple.FBDefName)
						{
							BackupThing2 = invThing2;
						}
					}
					if (invGearToEquip2 == null && !HasPrimFB && BackupThing2 != null)
					{
						RemoveThing = BackupThing2;
						invGearToEquip2 = (ThingWithComps)BackupThing2;
						if (primToSwap2 != null)
						{
							Swap = true;
						}
					}
					if (invGearToEquip2 != null)
					{
						string primDef = "";
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
							pawn.inventory.innerContainer.TryAdd(primToSwap2, true);
						}
						if (Swap)
						{
							string returnType = "SI";
							if (pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FEDefName || pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
							{
								ThingWithComps primary = pawn.equipment.Primary;
								(primary as FireWardenData).FWSwapType = returnType;
								(primary as FireWardenData).FWPawnID = pawn.thingIDNumber;
								(primary as FireWardenData).FWPrimDef = primDef;
								if (JobDriver_PelFESimple.DebugFWData)
								{
									ThingWithComps Test = pawn.equipment.Primary;
									string debugTest = pawn.Label + " : ";
									debugTest = debugTest + Test.Label + " : ";
									debugTest = debugTest + pawn.equipment.Primary.GetType().ToString() + " : ";
									if ((Test as FireWardenData).FWSwapType != null)
									{
										debugTest = debugTest + (Test as FireWardenData).FWSwapType + " : ";
									}
									else
									{
										debugTest += "null : ";
									}
									if ((Test as FireWardenData).FWPawnID.ToString() != null)
									{
										debugTest = debugTest + (Test as FireWardenData).FWPawnID.ToString() + " : ";
									}
									else
									{
										debugTest += "null : ";
									}
									if ((Test as FireWardenData).FWPrimDef != null)
									{
										debugTest += (Test as FireWardenData).FWPrimDef;
									}
									else
									{
										debugTest += "null";
									}
									Messages.Message(debugTest, pawn, MessageTypeDefOf.NeutralEvent, false);
								}
							}
						}
					}
				};
				toilInv.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
				yield return toilInv;
			}
			bool FWEquipping = Controller.Settings.EquippingDone;
			float FWSearchRange = 50f;
			FWSearchRange = (float)Controller.Settings.SearchRange;
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
			if (this.pawn.equipment.Primary != null)
			{
				if (this.pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(this.pawn.equipment.Primary))
				{
					HasPrimFE = true;
				}
				else if (this.pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
				{
					HasPrimFB = true;
				}
			}
			if (!HasPrimFE && !HasPrimFB && FWEquipping)
			{
				ThingWithComps invGearToEquip = null;
				ThingWithComps primToSwap = null;
				Thing BackupThing = null;
				if (this.pawn.equipment.Primary != null)
				{
					primToSwap = this.pawn.equipment.Primary;
				}
				foreach (Thing invThing in this.pawn.inventory.innerContainer)
				{
					if (invThing.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(invThing))
					{
						invGearToEquip = (ThingWithComps)invThing;
						if (primToSwap != null)
						{
							break;
						}
						break;
					}
					else if (invThing.def.defName == JobDriver_PelFESimple.FBDefName)
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
					bool skip = false;
					if (Controller.Settings.BrawlerNotOK && this.pawn.story.traits.HasTrait(TraitDefOf.Brawler))
					{
						skip = true;
					}
					TraverseParms traverseParams = TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.ByPawn, false);
                    bool validatorFE(Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, false) && FWFoamUtility.HasFEFoam(t) && !FWFoamUtility.ReplaceFEFoam(t);
                    bool validatorFB(Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, false);
                    if (!skip)
					{
						List<Thing> FElist = this.pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(JobDriver_PelFESimple.FEDefName, true));
						Thing FEGrab = GenClosest.ClosestThing_Global_Reachable(this.pawn.Position, this.pawn.Map, FElist, PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFE, null);
						if (FEGrab != null)
						{
							ThingToGrab = FEGrab;
						}
						else
						{
							List<Thing> FBlist = this.pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(JobDriver_PelFESimple.FBDefName, true));
							Thing FBGrab = GenClosest.ClosestThing_Global_Reachable(this.pawn.Position, this.pawn.Map, FBlist, PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFB, null);
							if (FBGrab != null)
							{
								ThingToGrab = FBGrab;
							}
						}
					}
					else
					{
						List<Thing> FBlist2 = this.pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(JobDriver_PelFESimple.FBDefName, true));
						Thing FBGrab2 = GenClosest.ClosestThing_Global_Reachable(this.pawn.Position, this.pawn.Map, FBlist2, PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFB, null);
						if (FBGrab2 != null)
						{
							ThingToGrab = FBGrab2;
						}
					}
					if (ThingToGrab != null)
					{
						toilEquipGoto.initAction = delegate()
						{
							if (Map.reservationManager.CanReserve(pawn, ThingToGrab, 1, -1, null, false))
							{
								pawn.Reserve(ThingToGrab, job, 1, -1, null, true);
							}
							pawn.pather.StartPath(ThingToGrab, PathEndMode.OnCell);
						};
						toilEquipGoto.FailOn(new Func<bool>(ThingToGrab.DestroyedOrNull));
						toilEquipGoto.AddFailCondition(() => JobDriver_PelFESimple.FWHasFE(pawn) && ThingToGrab.def.defName == JobDriver_PelFESimple.FEDefName);
						toilEquipGoto.AddFailCondition(() => JobDriver_PelFESimple.FWHasFB(pawn) && ThingToGrab.def.defName == JobDriver_PelFESimple.FBDefName);
						toilEquipGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
						yield return toilEquipGoto;
						if (ThingToGrab != null)
						{
							toilEquip.initAction = delegate()
							{
								ThingWithComps primDeEquip = pawn.equipment.Primary;
								string primDef = "N";
								if (primDeEquip != null)
								{
									primDef = pawn.equipment.Primary.def.defName;
									pawn.equipment.Remove(pawn.equipment.Primary);
									pawn.inventory.innerContainer.TryAdd(primDeEquip, true);
								}
								ThingWithComps FWGrabWithComps = (ThingWithComps)ThingToGrab;
								ThingWithComps FWGrabbed;
								if (FWGrabWithComps.def.stackLimit > 1 && FWGrabWithComps.stackCount > 1)
								{
									FWGrabbed = (ThingWithComps)FWGrabWithComps.SplitOff(1);
								}
								else
								{
									FWGrabbed = FWGrabWithComps;
									FWGrabbed.DeSpawn(DestroyMode.Vanish);
								}
								pawn.equipment.MakeRoomFor(FWGrabbed);
								pawn.equipment.AddEquipment(FWGrabbed);
								bool flag = false;
								string returnType = "EN";
								if (flag)
								{
									returnType = "ER";
								}
								if (pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FEDefName || pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
								{
									ThingWithComps primary = pawn.equipment.Primary;
									(primary as FireWardenData).FWSwapType = returnType;
									(primary as FireWardenData).FWPawnID = pawn.thingIDNumber;
									(primary as FireWardenData).FWPrimDef = primDef;
									if (JobDriver_PelFESimple.DebugFWData)
									{
										ThingWithComps Test = pawn.equipment.Primary;
										string debugTest = pawn.Label + " : ";
										debugTest = debugTest + Test.Label + " : ";
										debugTest = debugTest + pawn.equipment.Primary.GetType().ToString() + " : ";
										if ((Test as FireWardenData).FWSwapType != null)
										{
											debugTest = debugTest + (Test as FireWardenData).FWSwapType + " : ";
										}
										else
										{
											debugTest += "null : ";
										}
										if ((Test as FireWardenData).FWPawnID.ToString() != null)
										{
											debugTest = debugTest + (Test as FireWardenData).FWPawnID.ToString() + " : ";
										}
										else
										{
											debugTest += "null : ";
										}
										if ((Test as FireWardenData).FWPrimDef != null)
										{
											debugTest += (Test as FireWardenData).FWPrimDef;
										}
										else
										{
											debugTest += "null";
										}
										Messages.Message(debugTest, pawn, MessageTypeDefOf.NeutralEvent, false);
									}
								}
							};
							toilEquip.AddFailCondition(() => JobDriver_PelFESimple.FWHasFE(pawn) && ThingToGrab.def.defName == JobDriver_PelFESimple.FEDefName);
							toilEquip.AddFailCondition(() => JobDriver_PelFESimple.FWHasFB(pawn) && ThingToGrab.def.defName == JobDriver_PelFESimple.FBDefName);
							toilEquip.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
							yield return toilEquip;
						}
					}
				}
			}
			bool HasPrimFEEq = false;
			if (this.pawn.equipment.Primary != null && this.pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FEDefName && FWFoamUtility.HasFEFoam(this.pawn.equipment.Primary))
			{
				HasPrimFEEq = true;
			}
			if (HasPrimFEEq)
			{
				Verb FEVerbToUse = this.pawn.TryGetAttackVerb(this.TargetFire, false);
				float RangeFireExt = 10f;
				if (FEVerbToUse != null)
				{
					this.pawn.jobs.curJob.verbToUse = FEVerbToUse;
					RangeFireExt = this.pawn.jobs.curJob.verbToUse.verbProps.range;
					RangeFireExt *= (float)(Controller.Settings.HowClose / 100.0);
					if (RangeFireExt < 3f)
					{
						RangeFireExt = 3f;
					}
					if (RangeFireExt > this.pawn.jobs.curJob.verbToUse.verbProps.range)
					{
						RangeFireExt = this.pawn.jobs.curJob.verbToUse.verbProps.range;
					}
				}
				toilGoto.initAction = delegate()
				{
					if (Map.reservationManager.CanReserve(pawn, TargetFire, 1, -1, null, false))
					{
						pawn.Reserve(TargetFire, job, 1, -1, null, true);
					}
                    if (!CastPositionFinder.TryFindCastPosition(new CastPositionRequest
                    {
                        caster = pawn,
                        target = TargetFire,
                        verb = pawn.jobs.curJob.verbToUse,
                        maxRangeFromTarget = RangeFireExt,
                        wantCoverFromTarget = false
                    }, out IntVec3 dest))
                    {
                        toilGoto.actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                        return;
                    }
                    toilGoto.actor.pather.StartPath(dest, PathEndMode.OnCell);
					pawn.Map.pawnDestinationReservationManager.Reserve(pawn, pawn.jobs.curJob, dest);
				};
				toilGoto.tickAction = delegate()
				{
					if (!Controller.Settings.TooBrave)
					{
						if (pawn.pather.Moving && pawn.pather.nextCell != TargetFire.Position)
						{
							StartTacklingFireIfAnyAt(pawn.pather.nextCell, toilCast);
						}
						if (pawn.Position != TargetFire.Position)
						{
							StartTacklingFireIfAnyAt(pawn.Position, toilCast);
						}
					}
				};
				toilGoto.FailOnDespawnedOrNull(TargetIndex.A);
				toilGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
				toilGoto.atomicWithPrevious = true;
				yield return toilGoto;
				toilCast.initAction = delegate()
				{
					pawn.jobs.curJob.verbToUse.TryStartCastOn(TargetFire, false, true);
					if (TargetFire.Destroyed)
					{
						pawn.records.Increment(RecordDefOf.FiresExtinguished);
						pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
					}
				};
				toilCast.FailOnDespawnedOrNull(TargetIndex.A);
				toilCast.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
				yield return toilCast;
			}
			else
			{
				toilTouch.initAction = delegate()
				{
					if (Map.reservationManager.CanReserve(pawn, TargetFire, 1, -1, null, false))
					{
						pawn.Reserve(TargetFire, job, 1, -1, null, true);
					}
					pawn.pather.StartPath(TargetFire, PathEndMode.Touch);
				};
				toilTouch.tickAction = delegate()
				{
					if (!Controller.Settings.TooBrave)
					{
						if (pawn.pather.Moving && pawn.pather.nextCell != TargetFire.Position)
						{
							StartTacklingFireIfAnyAt(pawn.pather.nextCell, toilBeat);
						}
						if (pawn.Position != TargetFire.Position)
						{
							StartTacklingFireIfAnyAt(pawn.Position, toilBeat);
						}
					}
				};
				toilTouch.FailOnDespawnedOrNull(TargetIndex.A);
				toilTouch.defaultCompleteMode = ToilCompleteMode.PatherArrival;
				toilTouch.atomicWithPrevious = true;
				yield return toilTouch;
				toilBeat.tickAction = delegate()
				{
					if (!pawn.CanReachImmediate(TargetFire, PathEndMode.Touch))
					{
						JumpToToil(toilTouch);
						return;
					}
					if (!(pawn.Position != TargetFire.Position) || !StartTacklingFireIfAnyAt(pawn.Position, toilBeat))
					{
						if (pawn.equipment.Primary != null)
						{
							if (pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
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
						if (TargetFire.Destroyed)
						{
							pawn.records.Increment(RecordDefOf.FiresExtinguished);
							pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
						}
					}
				};
				toilBeat.FailOnDespawnedOrNull(TargetIndex.A);
				toilBeat.defaultCompleteMode = ToilCompleteMode.Never;
				yield return toilBeat;
				if (this.pawn.equipment.Primary != null && this.pawn.equipment.Primary.def.defName == JobDriver_PelFESimple.FBDefName)
				{
					toilBash.initAction = delegate()
					{
						if (TargetFire != null && Map.reservationManager.CanReserve(pawn, TargetFire, 1, -1, null, false))
						{
							pawn.Reserve(TargetFire, job, 1, -1, null, true);
						}
						pawn.pather.StopDead();
					};
					toilBash.handlingFacing = true;
					toilBash.tickAction = delegate()
					{
						pawn.rotationTracker.FaceTarget(pawn.CurJob.GetTarget(TargetIndex.A));
						if (TargetFire != null)
						{
							pawn.Drawer.Notify_MeleeAttackOn(TargetFire);
						}
					};
					toilBash.PlaySoundAtStart(SoundDefOf.Interact_BeatFire);
					toilBash.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
					toilBash.AddFinishAction(delegate
					{
						if (TargetFire != null && !TargetFire.Destroyed)
						{
							TargetFire.Destroy(DestroyMode.Vanish);
						}
					});
					toilBash.FailOnDespawnedOrNull(TargetIndex.A);
					toilBash.defaultCompleteMode = ToilCompleteMode.Delay;
					int ticks = 50;
					float WorkSpeed = this.pawn.GetStatValue(StatDefOf.WorkSpeedGlobal, true);
					if (WorkSpeed <= 0f)
					{
						WorkSpeed = 1f;
					}
					ticks = (int)((float)ticks * (1f / WorkSpeed));
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
			yield break;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004328 File Offset: 0x00002528
		private bool StartTacklingFireIfAnyAt(IntVec3 cell, Toil nextToil)
		{
			List<Thing> thingList = cell.GetThingList(this.pawn.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
                if (thingList[i] is Fire fire && fire.parent == null)
                {
                    this.job.targetA = fire;
                    this.pawn.pather.StopDead();
                    base.JumpToToil(nextToil);
                    return true;
                }
            }
			return false;
		}

		// Token: 0x0400002A RID: 42
		private static readonly string FEDefName = "Gun_Fire_Ext";

		// Token: 0x0400002B RID: 43
		private static readonly string FBDefName = "Firebeater";

		// Token: 0x0400002C RID: 44
		private static readonly bool DebugFWData = false;
	}
}
