using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x0200001D RID: 29
	public class JobGiver_FWSwapback : ThinkNode_JobGiver
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00004AC0 File Offset: 0x00002CC0
		protected override Job TryGiveJob(Pawn pawn)
		{
			ThinkResult thinkResult = JobGiver_FWSwapback.swapBack.TryIssueJobPackage(pawn, default(JobIssueParams));
			Job result;
			if (thinkResult.IsValid)
			{
				result = thinkResult.Job;
			}
			else
			{
				if (!pawn.inventory.innerContainer.NullOrEmpty<Thing>())
				{
					foreach (Thing invFECheck in pawn.inventory.innerContainer)
					{
						if ((invFECheck.def.defName == JobGiver_FWSwapback.FEDefName || invFECheck.def.defName == JobGiver_FWSwapback.FBDefName) && (invFECheck as FireWardenData).FWSwapType != "N")
						{
							this.FEResetVars((ThingWithComps)invFECheck);
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004BAC File Offset: 0x00002DAC
		public void FEResetVars(ThingWithComps thingWC)
		{
			if (thingWC != null && (thingWC.def.defName == JobGiver_FWSwapback.FEDefName || thingWC.def.defName == JobGiver_FWSwapback.FBDefName))
			{
				(thingWC as FireWardenData).FWSwapType = "N";
				(thingWC as FireWardenData).FWPawnID = 0;
				(thingWC as FireWardenData).FWPrimDef = "N";
			}
		}

		// Token: 0x04000038 RID: 56
		private static readonly JobGiver_SwapBackFW swapBack = new JobGiver_SwapBackFW();

		// Token: 0x04000039 RID: 57
		public static string FEDefName = "Gun_Fire_Ext";

		// Token: 0x0400003A RID: 58
		public static string FBDefName = "Firebeater";

		// Token: 0x0200003D RID: 61
		[DefOf]
		public static class FWSwapJobs
		{
			// Token: 0x040000A7 RID: 167
			public static JobDef FWNoSwap;
		}
	}
}
