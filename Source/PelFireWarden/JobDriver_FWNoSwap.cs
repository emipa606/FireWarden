using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000014 RID: 20
	public class JobDriver_FWNoSwap : JobDriver
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00003CB0 File Offset: 0x00001EB0
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003CB3 File Offset: 0x00001EB3
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return new Toil
			{
				initAction = delegate()
				{
					if (!this.pawn.inventory.innerContainer.NullOrEmpty<Thing>())
					{
						foreach (Thing invFECheck in this.pawn.inventory.innerContainer)
						{
							if (invFECheck.def.defName == JobDriver_FWNoSwap.FEDefName && (invFECheck as FireWardenData).FWSwapType != "N")
							{
								this.FEResetVars((ThingWithComps)invFECheck);
							}
						}
					}
					this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003CC4 File Offset: 0x00001EC4
		public void FEResetVars(ThingWithComps thingWC)
		{
			if (thingWC != null && thingWC.def.defName == JobDriver_FWNoSwap.FEDefName)
			{
				(thingWC as FireWardenData).FWSwapType = "N";
				(thingWC as FireWardenData).FWPawnID = 0;
				(thingWC as FireWardenData).FWPrimDef = "N";
			}
		}

		// Token: 0x04000025 RID: 37
		public static string FEDefName = "Gun_Fire_Ext";
	}
}
