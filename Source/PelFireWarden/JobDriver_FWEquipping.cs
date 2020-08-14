using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden
{
	// Token: 0x02000013 RID: 19
	public class JobDriver_FWEquipping : JobDriver
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003AA3 File Offset: 0x00001CA3
		public bool IsFW(Pawn p)
		{
			return p.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) && !p.story.traits.HasTrait(TraitDefOf.Pyromaniac);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003AD4 File Offset: 0x00001CD4
		public static bool FWHasFE(Pawn p)
		{
			if (p.equipment.Primary != null)
			{
				if (p.equipment.Primary.def.defName == JobDriver_FWEquipping.FEDefName && FWFoamUtility.HasFEFoam(p.equipment.Primary))
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
						if (enumerator.Current.def.defName == JobDriver_FWEquipping.FEDefName && FWFoamUtility.HasFEFoam(p.equipment.Primary))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003BAC File Offset: 0x00001DAC
		public static bool FWHasFB(Pawn p)
		{
			if (p.equipment.Primary != null)
			{
				if (p.equipment.Primary.def.defName == JobDriver_FWEquipping.FBDefName)
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
						if (enumerator.Current.def.defName == JobDriver_FWEquipping.FBDefName)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003C5C File Offset: 0x00001E5C
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toilEquipGoto = new Toil();
			Toil toilEquip = new Toil();
			if (Controller.Settings.EquippingDone)
			{
				Thing ThingToGrab = job.GetTarget(TargetIndex.A).Thing;
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
					toilEquipGoto.tickAction = delegate()
					{
					};
					toilEquipGoto.AddFailCondition(() => !this.IsFW(this.pawn));
					toilEquipGoto.AddFailCondition(() => JobDriver_FWEquipping.FWHasFE(pawn) && ThingToGrab.def.defName == JobDriver_FWEquipping.FEDefName);
					toilEquipGoto.AddFailCondition(() => JobDriver_FWEquipping.FWHasFB(pawn) && ThingToGrab.def.defName == JobDriver_FWEquipping.FBDefName);
					toilEquipGoto.FailOn(new Func<bool>(ThingToGrab.DestroyedOrNull));
					toilEquipGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
					yield return toilEquipGoto;
					if (ThingToGrab != null)
					{
						toilEquip.initAction = delegate()
						{
							ThingWithComps FEGrabWithComps = (ThingWithComps)ThingToGrab;
							ThingWithComps FEGrabbed;
							if (FEGrabWithComps.def.stackLimit > 1 && FEGrabWithComps.stackCount > 1)
							{
								FEGrabbed = (ThingWithComps)FEGrabWithComps.SplitOff(1);
							}
							else
							{
								FEGrabbed = FEGrabWithComps;
								FEGrabbed.DeSpawn(DestroyMode.Vanish);
							}
							string returnType = "N";
							int pawnIDNumber = 0;
							string primDef = "N";
							if (FEGrabbed.def.defName == JobDriver_FWEquipping.FEDefName || FEGrabbed.def.defName == JobDriver_FWEquipping.FBDefName)
							{
								(FEGrabbed as FireWardenData).FWSwapType = returnType;
								(FEGrabbed as FireWardenData).FWPawnID = pawnIDNumber;
								(FEGrabbed as FireWardenData).FWPrimDef = primDef;
							}
							pawn.inventory.innerContainer.TryAdd(FEGrabbed, true);
						};
						toilEquip.AddFailCondition(() => !this.IsFW(this.pawn));
						toilEquip.AddFailCondition(() => JobDriver_FWEquipping.FWHasFE(pawn) && ThingToGrab.def.defName == JobDriver_FWEquipping.FEDefName);
						toilEquip.AddFailCondition(() => JobDriver_FWEquipping.FWHasFB(pawn) && ThingToGrab.def.defName == JobDriver_FWEquipping.FBDefName);
						toilEquip.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
						yield return toilEquip;
					}
				}
			}
			yield break;
		}

		// Token: 0x04000023 RID: 35
		private static readonly string FEDefName = "Gun_Fire_Ext";

		// Token: 0x04000024 RID: 36
		private static readonly string FBDefName = "Firebeater";
	}
}
