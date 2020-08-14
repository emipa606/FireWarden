using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000006 RID: 6
	public class JobDriver_FEReplace : JobDriver
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002839 File Offset: 0x00000A39
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, true);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000285C File Offset: 0x00000A5C
		internal void FEReplaceThing(Pawn pawn, Thing togo, Thing tofill, bool Deltogo)
		{
			if (Deltogo)
			{
				togo.Destroy(DestroyMode.Vanish);
			}
			FWFoamUtility.FEFillUp(tofill);
			if (Controller.Settings.SendReplaceMsgs)
			{
				Messages.Message("FWrd.FEReplaced".Translate(pawn?.LabelShort, tofill?.Label.CapitalizeFirst()), pawn, MessageTypeDefOf.NeutralEvent, false);
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000028CD File Offset: 0x00000ACD
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Pawn actor = base.GetActor();
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			Toil replace = Toils_General.Wait(180, TargetIndex.None);
			replace.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			replace.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			replace.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			yield return replace;
			yield return new Toil
			{
				initAction = delegate()
				{
					if (this.TargetThingA.stackCount > 1)
					{
						this.TargetThingA.stackCount--;
						this.FEReplaceThing(actor, this.TargetThingA, this.TargetThingB, false);
					}
					else
					{
						this.FEReplaceThing(actor, this.TargetThingA, this.TargetThingB, true);
					}
					this.EndJobWith(JobCondition.Succeeded);
				}
			};
			yield break;
		}
	}
}
