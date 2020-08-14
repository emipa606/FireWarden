using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x0200000A RID: 10
	internal class WorkGiver_PelFWFightFires : WorkGiver_Scanner
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002B8C File Offset: 0x00000D8C
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ThingDefOf.Fire);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002B98 File Offset: 0x00000D98
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002B9B File Offset: 0x00000D9B
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002BA0 File Offset: 0x00000DA0
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Fire fire = t as Fire;
			if (fire == null)
			{
				return false;
			}
			Pawn pawn2 = fire.parent as Pawn;
			if (pawn2 != null)
			{
				if (pawn2 == pawn)
				{
					return false;
				}
				if ((pawn2.Faction == pawn.Faction || pawn2.HostFaction == pawn.Faction || pawn2.HostFaction == pawn.HostFaction) && !pawn.Map.areaManager.Home[fire.Position] && IntVec3Utility.ManhattanDistanceFlat(pawn.Position, pawn2.Position) > 15)
				{
					return false;
				}
				if (!pawn.CanReach(pawn2, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
				{
					return false;
				}
			}
			else
			{
				if (pawn.WorkTagIsDisabled(WorkTags.Firefighting))
				{
					return false;
				}
				if (pawn.WorkTagIsDisabled(WorkTags.Violent))
				{
					return false;
				}
				if (pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac))
				{
					return false;
				}
				if (!pawn.Map.areaManager.Home[fire.Position])
				{
					JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans, null);
					return false;
				}
			}
			if ((pawn.Position - fire.Position).LengthHorizontalSquared > 225)
			{
				LocalTargetInfo target = fire;
				if (!pawn.CanReserve(target, 1, -1, null, forced))
				{
					return false;
				}
			}
			return !WorkGiver_PelFWFightFires.FireIsBeingHandled(fire, pawn);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002CE3 File Offset: 0x00000EE3
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("PelFESimple", true), t);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002CFC File Offset: 0x00000EFC
		public static bool FireIsBeingHandled(Fire f, Pawn FW)
		{
			if (!f.Spawned)
			{
				return false;
			}
			Pawn pawn = f.Map.reservationManager.FirstRespectedReserver(f, FW);
			if (pawn == null || pawn.Drafted || pawn.IsBurning())
			{
				return false;
			}
			if (Controller.Settings.HandleAnyway)
			{
				if (FW.workSettings.WorkIsActive(WorkGiver_PelFWFightFires.FWWorkTypeDef.PelFireWarden))
				{
					if (WorkGiver_PelFWFightFires.FWHasFE(FW))
					{
						return false;
					}
					if (WorkGiver_PelFWFightFires.FWHasFE(pawn) && pawn.workSettings.WorkIsActive(WorkGiver_PelFWFightFires.FWWorkTypeDef.PelFireWarden))
					{
						return pawn.Position.InHorDistOf(f.Position, (float)Controller.Settings.HandledRange);
					}
					return pawn.Position.InHorDistOf(f.Position, 5f);
				}
				else
				{
					if (WorkGiver_PelFWFightFires.FWHasFE(pawn) && pawn.workSettings.WorkIsActive(WorkGiver_PelFWFightFires.FWWorkTypeDef.PelFireWarden))
					{
						return pawn.Position.InHorDistOf(f.Position, (float)Controller.Settings.HandledRange);
					}
					return pawn.Position.InHorDistOf(f.Position, 5f);
				}
			}
			else
			{
				if (WorkGiver_PelFWFightFires.FWHasFE(pawn) && pawn.workSettings.WorkIsActive(WorkGiver_PelFWFightFires.FWWorkTypeDef.PelFireWarden))
				{
					return pawn.Position.InHorDistOf(f.Position, (float)Controller.Settings.HandledRange);
				}
				return pawn.Position.InHorDistOf(f.Position, 5f);
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002E70 File Offset: 0x00001070
		public static bool FWHasFE(Pawn FW)
		{
			if (FW.equipment.Primary != null)
			{
				return FW.equipment.Primary.def.defName == "Gun_Fire_Ext" && FWFoamUtility.HasFEFoam(FW.equipment.Primary);
			}
			if (!FW.inventory.innerContainer.NullOrEmpty<Thing>())
			{
				foreach (Thing invFECheck in FW.inventory.innerContainer)
				{
					if (invFECheck.def.defName == "Gun_Fire_Ext" && FWFoamUtility.HasFEFoam(invFECheck))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x04000007 RID: 7
		private const int NearbyPawnRadius = 15;

		// Token: 0x04000008 RID: 8
		private const int MaxReservationCheckDistance = 15;

		// Token: 0x04000009 RID: 9
		private const float HandledDistance = 5f;

		// Token: 0x0400000A RID: 10
		private const string FEDefName = "Gun_Fire_Ext";

		// Token: 0x02000025 RID: 37
		[DefOf]
		public static class FWWorkTypeDef
		{
			// Token: 0x0400004F RID: 79
			public static WorkTypeDef PelFireWarden;
		}
	}
}
