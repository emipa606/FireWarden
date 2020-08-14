using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000015 RID: 21
	public class JobDriver_FWSwapping : JobDriver
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00003DE8 File Offset: 0x00001FE8
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003DEB File Offset: 0x00001FEB
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return new Toil
			{
				initAction = delegate()
				{
					if (this.pawn.equipment.Primary.def.defName == JobDriver_FWSwapping.FEDefName || this.pawn.equipment.Primary.def.defName == JobDriver_FWSwapping.FBDefName)
					{
						ThingWithComps primToSwap = (ThingWithComps)this.job.GetTarget(TargetIndex.A).Thing;
						Thing RemoveThing = null;
						if (this.job.GetTarget(TargetIndex.B) != null)
						{
							RemoveThing = this.job.GetTarget(TargetIndex.B).Thing;
						}
						this.pawn.equipment.Remove(this.pawn.equipment.Primary);
						if (RemoveThing != null)
						{
							ThingWithComps invGearToEquip = (ThingWithComps)RemoveThing;
							this.pawn.inventory.innerContainer.Remove(RemoveThing);
							this.pawn.equipment.MakeRoomFor(invGearToEquip);
							this.pawn.equipment.AddEquipment(invGearToEquip);
						}
						this.pawn.inventory.innerContainer.TryAdd(primToSwap, true);
						this.FWResetVars(primToSwap);
						this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
					}
				},
				defaultCompleteMode = ToilCompleteMode.FinishedBusy
			};
			yield break;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003DFC File Offset: 0x00001FFC
		public void FWResetVars(ThingWithComps thingWC)
		{
			if (thingWC != null && (thingWC.def.defName == JobDriver_FWSwapping.FEDefName || thingWC.def.defName == JobDriver_FWSwapping.FBDefName))
			{
				(thingWC as FireWardenData).FWSwapType = "N";
				(thingWC as FireWardenData).FWPawnID = 0;
				(thingWC as FireWardenData).FWPrimDef = "N";
			}
		}

		// Token: 0x04000026 RID: 38
		public static string FEDefName = "Gun_Fire_Ext";

		// Token: 0x04000027 RID: 39
		public static string FBDefName = "Firebeater";
	}
}
