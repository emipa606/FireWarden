using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x0200001B RID: 27
	public class JobGiver_equipnewFW : ThinkNode
	{
		// Token: 0x06000067 RID: 103 RVA: 0x000043C0 File Offset: 0x000025C0
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParm)
		{
			ThinkResult result = ThinkResult.NoJob;
			if (!Controller.Settings.EquippingDone)
			{
				result = ThinkResult.NoJob;
			}
			else
			{
				bool IsFW = false;
				if (pawn.IsColonistPlayerControlled && pawn.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) && !pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac))
				{
					IsFW = true;
				}
				if (!IsFW)
				{
					result = ThinkResult.NoJob;
				}
				else
				{
					bool FWHasGear = false;
					string TargetGearDef = "N";
					if (FWResearch.FireExt.IsFinished)
					{
						if (!JobGiver_equipnewFW.FWGotFE(pawn))
						{
							if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
							{
								if (!Controller.Settings.BrawlerNotOK)
								{
									TargetGearDef = JobGiver_equipnewFW.FEDefName;
								}
								else if (!JobGiver_equipnewFW.FWGotFB(pawn))
								{
									TargetGearDef = JobGiver_equipnewFW.FBDefName;
								}
								else
								{
									FWHasGear = true;
								}
							}
							else
							{
								TargetGearDef = JobGiver_equipnewFW.FEDefName;
							}
						}
						else
						{
							FWHasGear = true;
						}
					}
					else if (FWResearch.FireBeater.IsFinished)
					{
						if (!JobGiver_equipnewFW.FWGotFB(pawn))
						{
							TargetGearDef = JobGiver_equipnewFW.FBDefName;
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
						bool IsFightingFires = false;
						if (pawn.CurJob != null && pawn.CurJob.targetA != null && pawn.CurJob.targetA.HasThing && pawn.CurJob.targetA.Thing.def == ThingDefOf.Fire)
						{
							IsFightingFires = true;
						}
						bool IsAlreadyEquipping = false;
						if (pawn.CurJob != null && pawn.CurJobDef == FWEquipJobs.FWEquipping)
						{
							IsAlreadyEquipping = true;
						}
						if (IsFightingFires || IsAlreadyEquipping)
						{
							result = ThinkResult.NoJob;
						}
						else
						{
							bool IsHomeFire = false;
							List<Thing> list = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Fire);
							for (int i = 0; i < list.Count; i++)
							{
								Thing thing = list[i];
								if (pawn.Map.areaManager.Home[thing.Position] && !thing.Position.Fogged(thing.Map))
								{
									IsHomeFire = true;
									break;
								}
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
								float FWSearchRange = (float)Controller.Settings.SearchRange;
								if (FWSearchRange < 25f)
								{
									FWSearchRange = 25f;
								}
								if (FWSearchRange > 75f)
								{
									FWSearchRange = 75f;
								}
								List<Thing> FElist = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(JobGiver_equipnewFW.FEDefName, true));
								List<Thing> FBlist = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(JobGiver_equipnewFW.FBDefName, true));
								TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
								Predicate<Thing> validatorFE = (Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, false) && !FWFoamUtility.ReplaceFEFoam(t) && FWFoamUtility.HasFEFoam(t);
								Predicate<Thing> validatorFB = (Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, false);
								Thing ThingToGrab = null;
								if (TargetGearDef != "N")
								{
									if (TargetGearDef == JobGiver_equipnewFW.FEDefName)
									{
										if (!JobGiver_equipnewFW.FWGotFE(pawn))
										{
											ThingToGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, FElist, PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFE, null);
										}
										if (ThingToGrab == null && !JobGiver_equipnewFW.FWGotFB(pawn))
										{
											ThingToGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, FBlist, PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFB, null);
										}
									}
									else if (!JobGiver_equipnewFW.FWGotFB(pawn))
									{
										ThingToGrab = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, FBlist, PathEndMode.OnCell, traverseParams, FWSearchRange, validatorFB, null);
									}
								}
								if (ThingToGrab != null && pawn.inventory.innerContainer.CanAcceptAnyOf(ThingToGrab, false))
								{
									return new ThinkResult(new Job(FWEquipJobs.FWEquipping, ThingToGrab), this, null, false);
								}
								result = ThinkResult.NoJob;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004814 File Offset: 0x00002A14
		public static bool FWGotFE(Pawn p)
		{
			if (p.equipment.Primary != null && p.equipment.Primary.def.defName == JobGiver_equipnewFW.FEDefName && FWFoamUtility.HasFEFoam(p.equipment.Primary))
			{
				return true;
			}
			if (!p.inventory.innerContainer.NullOrEmpty<Thing>())
			{
				foreach (Thing invFECheck in p.inventory.innerContainer)
				{
					if (invFECheck.def.defName == JobGiver_equipnewFW.FEDefName)
					{
						if ((invFECheck as FireWardenData).FWSwapType != "N")
						{
							JobGiver_equipnewFW.FWResetVars((ThingWithComps)invFECheck);
						}
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000048FC File Offset: 0x00002AFC
		public static bool FWGotFB(Pawn p)
		{
			if (p.equipment.Primary != null && p.equipment.Primary.def.defName == JobGiver_equipnewFW.FBDefName)
			{
				return true;
			}
			if (!p.inventory.innerContainer.NullOrEmpty<Thing>())
			{
				foreach (Thing invFECheck in p.inventory.innerContainer)
				{
					if (invFECheck.def.defName == JobGiver_equipnewFW.FBDefName)
					{
						if ((invFECheck as FireWardenData).FWSwapType != "N")
						{
							JobGiver_equipnewFW.FWResetVars((ThingWithComps)invFECheck);
						}
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000049D4 File Offset: 0x00002BD4
		public static void FWResetVars(ThingWithComps thingWC)
		{
			if (thingWC != null && (thingWC.def.defName == JobGiver_equipnewFW.FEDefName || thingWC.def.defName == JobGiver_equipnewFW.FBDefName))
			{
				(thingWC as FireWardenData).FWSwapType = "N";
				(thingWC as FireWardenData).FWPawnID = 0;
				(thingWC as FireWardenData).FWPrimDef = "N";
			}
		}

		// Token: 0x04000031 RID: 49
		public static string FEDefName = "Gun_Fire_Ext";

		// Token: 0x04000032 RID: 50
		public static string FBDefName = "Firebeater";

		// Token: 0x04000033 RID: 51
		public ThingWithComps FEToCheck;

		// Token: 0x04000034 RID: 52
		public string FESwapinfo = "N";

		// Token: 0x04000035 RID: 53
		public int FEPawnId;

		// Token: 0x04000036 RID: 54
		public string FEPrimDef = "N";
	}
}
