using System;
using System.Collections.Generic;
using Verse;

namespace PelFireWarden
{
	// Token: 0x02000003 RID: 3
	public class FWFoamUtility
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000024F0 File Offset: 0x000006F0
		public static void FEFillUp(Thing thing)
		{
			if (FWFoamUtility.IsFEFoamThing(thing))
			{
				(thing as FireWardenData).FEFoamUses = (thing as FireWardenData).FWComp.Props.FEFoamUses;
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000251C File Offset: 0x0000071C
		public static Thing GetFE(Pawn pawn)
		{
			Thing FE = null;
			bool flag;
			if (pawn == null)
			{
				flag = (null != null);
			}
			else
			{
				Pawn_EquipmentTracker equipment = pawn.equipment;
				flag = ((equipment?.Primary) != null);
			}
			if (flag)
			{
				Thing chkPrim = pawn.equipment.Primary;
				if (FWFoamUtility.IsFEFoamThing(chkPrim))
				{
					return chkPrim;
				}
			}
			if (pawn.inventory.innerContainer.Any)
			{
				List<Thing> invList = pawn.inventory.innerContainer.InnerListForReading;
				if (invList.Count > 0)
				{
					foreach (Thing chkInv in invList)
					{
						if (FWFoamUtility.IsFEFoamThing(chkInv))
						{
							return chkInv;
						}
					}
					return FE;
				}
			}
			return FE;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000025D8 File Offset: 0x000007D8
		public static bool IsFEFoamThing(Thing thing)
		{
			return (thing as ThingWithComps).TryGetComp<CompFWData>() != null && thing.def.defName == "Gun_Fire_Ext";
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002601 File Offset: 0x00000801
		public static bool HasFEFoam(Thing thing)
		{
			return !FWFoamUtility.IsFEFoamThing(thing) || (thing as FireWardenData).FEFoamUses > 0;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000261C File Offset: 0x0000081C
		public static bool ReplaceFEFoam(Thing thing)
		{
			bool replace = false;
			if (FWFoamUtility.IsFEFoamThing(thing))
			{
				float foamLevel = (float)(thing as FireWardenData).FEFoamUses;
				float maxLevel = (float)(thing as FireWardenData).FWComp.Props.FEFoamUses;
				if (maxLevel > 0f && foamLevel * 100f / maxLevel <= Controller.Settings.ReplacePct)
				{
					replace = true;
				}
			}
			return replace;
		}

		// Token: 0x04000001 RID: 1
		public const string FEdefName = "Gun_Fire_Ext";
	}
}
