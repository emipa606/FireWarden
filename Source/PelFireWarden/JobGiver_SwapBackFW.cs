using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x0200001F RID: 31
	public class JobGiver_SwapBackFW : ThinkNode
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00004C40 File Offset: 0x00002E40
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParm)
		{
			ThinkResult result = ThinkResult.NoJob;
			bool HasPrimFWGear = false;
			bool HasPrimFEEq = false;
			if (pawn.equipment.Primary != null && pawn.equipment.Primary.def.defName == this.FEDefName)
			{
				HasPrimFEEq = true;
			}
			bool HasPrimFBEq = false;
			if (pawn.equipment.Primary != null && pawn.equipment.Primary.def.defName == this.FBDefName)
			{
				HasPrimFBEq = true;
			}
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
				bool IsFightingFires = false;
				if (pawn.CurJob != null && pawn.CurJob.targetA != null && pawn.CurJob.targetA.HasThing && pawn.CurJob.targetA.Thing.def == ThingDefOf.Fire)
				{
					IsFightingFires = true;
				}
				bool IsAlreadySwapping = false;
				if (pawn.CurJob != null && (pawn.CurJobDef == FWSwapJobs.FWSwapping || pawn.CurJobDef == FWSwapJobs.FWSwapReturn))
				{
					IsAlreadySwapping = true;
				}
				if (IsFightingFires || IsAlreadySwapping)
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
						this.FWToCheck = pawn.equipment.Primary;
						this.FWSwapinfo = (this.FWToCheck as FireWardenData).FWSwapType;
						this.FWPawnId = (this.FWToCheck as FireWardenData).FWPawnID;
						this.FWPrimDef = (this.FWToCheck as FireWardenData).FWPrimDef;
						if (pawn.thingIDNumber == this.FWPawnId)
						{
							Thing RemoveThing = null;
							bool Swap = false;
							foreach (Thing invThing in pawn.inventory.innerContainer)
							{
								if (invThing.def.defName == this.FWPrimDef)
								{
									RemoveThing = invThing;
									ThingWithComps thingWithComps = (ThingWithComps)invThing;
									Swap = true;
									break;
								}
							}
							JobDef SwapJobDef = FWSwapJobs.FWSwapping;
							if (this.FWSwapinfo == this.SwapReturnType)
							{
								SwapJobDef = FWSwapJobs.FWSwapReturn;
							}
							Job swapJob;
							if (Swap)
							{
								swapJob = new Job(SwapJobDef, pawn.equipment.Primary, RemoveThing);
							}
							else
							{
								swapJob = new Job(SwapJobDef, pawn.equipment.Primary);
							}
							return new ThinkResult(swapJob, this, null, false);
						}
						if (this.FWSwapinfo != "N" || this.FWPawnId != 0 || this.FWPrimDef != "N")
						{
							this.FWResetVars(pawn.equipment.Primary);
						}
						result = ThinkResult.NoJob;
					}
				}
			}
			return result;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004FA4 File Offset: 0x000031A4
		public void FWResetVars(ThingWithComps thingWC)
		{
			if (thingWC != null && (thingWC.def.defName == this.FEDefName || thingWC.def.defName == this.FBDefName))
			{
				(thingWC as FireWardenData).FWSwapType = "N";
				(thingWC as FireWardenData).FWPawnID = 0;
				(thingWC as FireWardenData).FWPrimDef = "N";
			}
		}

		// Token: 0x0400003D RID: 61
		public string FEDefName = "Gun_Fire_Ext";

		// Token: 0x0400003E RID: 62
		public string FBDefName = "Firebeater";

		// Token: 0x0400003F RID: 63
		public ThingWithComps FWToCheck;

		// Token: 0x04000040 RID: 64
		public string FWSwapinfo = "N";

		// Token: 0x04000041 RID: 65
		public int FWPawnId;

		// Token: 0x04000042 RID: 66
		public string FWPrimDef = "N";

		// Token: 0x04000043 RID: 67
		public string SwapReturnType = "ER";
	}
}
