using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000007 RID: 7
	public class JobGiver_FEAutoReplace : ThinkNode_JobGiver
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000028E8 File Offset: 0x00000AE8
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!pawn.IsColonistPlayerControlled)
			{
				return null;
			}
			if (pawn.InMentalState)
			{
				return null;
			}
			if ((pawn?.Map) == null)
			{
				return null;
			}
			JobDef jobdef = DefDatabase<JobDef>.GetNamed("FEReplace", true);
			if ((pawn?.CurJobDef) == jobdef)
			{
				return null;
			}
			Thing FE = FWFoamUtility.GetFE(pawn);
			if (FE != null && FWFoamUtility.ReplaceFEFoam(FE))
			{
				Thing targ;
				this.FindBestReplace(pawn, FE.def, out targ);
				if (targ != null)
				{
					return new Job(jobdef, targ, FE);
				}
			}
			return null;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002970 File Offset: 0x00000B70
		internal void FindBestReplace(Pawn FW, ThingDef FEItem, out Thing targ)
		{
			targ = null;
			if ((FW?.Map) != null)
			{
				List<Thing> listFE = FW?.Map.listerThings.ThingsOfDef(FEItem);
				int needed = 1;
				if (listFE.Count > 0)
				{
					Thing besttarg = null;
					float bestpoints = 0f;
					for (int i = 0; i < listFE.Count; i++)
					{
						Thing targchk = listFE[i];
						if (!targchk.IsForbidden(FW) && ((targchk?.Faction) == null || targchk.Faction.IsPlayer) && FW.CanReserveAndReach(targchk, PathEndMode.ClosestTouch, Danger.None, 1, -1, null, false) && FWFoamUtility.IsFEFoamThing(targchk) && !FWFoamUtility.ReplaceFEFoam(targchk))
						{
							float targpoints;
							if (targchk.stackCount > needed)
							{
								targpoints = (float)targchk.stackCount / FW.Position.DistanceTo(targchk.Position);
							}
							else
							{
								targpoints = (float)targchk.stackCount / (FW.Position.DistanceTo(targchk.Position) * 2f);
							}
							if (targpoints > bestpoints)
							{
								besttarg = targchk;
								bestpoints = targpoints;
							}
						}
					}
					if (besttarg != null)
					{
						targ = besttarg;
					}
				}
			}
		}
	}
}
