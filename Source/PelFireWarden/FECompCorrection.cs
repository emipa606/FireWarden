using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PelFireWarden
{
	// Token: 0x02000002 RID: 2
	[StaticConstructorOnStartup]
	public class FECompCorrection
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		static FECompCorrection()
		{
			FECompCorrection.FECompCorrection_Setup_FE();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		private static void FECompCorrection_Setup_FE()
		{
			FECompCorrection.ProxSetup_Comp(typeof(CompProperties_FWData), (ThingDef def) => def.defName == FireWardenData.FEDefName);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002088 File Offset: 0x00000288
		private static void ProxSetup_Comp(Type compType, Func<ThingDef, bool> qualifier)
		{
			List<ThingDef> list = DefDatabase<ThingDef>.AllDefsListForReading.Where(qualifier).ToList();
			GenList.RemoveDuplicates<ThingDef>(list);
			int count = 0;
			foreach (ThingDef def in list)
			{
				if (def.comps != null && !GenCollection.Any<CompProperties>(def.comps, (Predicate<CompProperties>)((CompProperties c) => ((object)c).GetType() == compType)))
				{
					def.comps.Add((CompProperties)(object)(CompProperties)Activator.CreateInstance(compType));
					count++;
				}
			}
			Log.Message("Fire warden: Corrected " + count + " Fire extinguisher definitions.", false);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002164 File Offset: 0x00000364
		private static void FECorrection_FE()
		{
			int count = 0;
			int changed = 0;
			ThingDef def = DefDatabase<ThingDef>.GetNamed(FireWardenData.FEDefName, false);
			if (def != null)
			{
				List<Map> maps = Find.Maps;
				if (maps.Count > 0)
				{
					foreach (Map map in maps)
					{
						List<Thing> things = map.listerThings.ThingsOfDef(def);
						if (things.Count > 0)
						{
							foreach (Thing thing in things)
							{
								if (thing.TryGetComp<CompFWData>() == null)
								{
									FECompCorrection.AddCompToThing(thing, ref changed);
									count++;
								}
							}
						}
						List<Pawn> pawns = map?.mapPawns.AllPawns;
						if (pawns.Count > 0)
						{
							foreach (Pawn pawn in pawns)
							{
								FECompCorrection.CheckPawnsGear(pawn, def, ref count, ref changed);
							}
						}
					}
				}
				List<Pawn> travellers = Find.WorldPawns.AllPawnsAlive;
				if (travellers.Count > 0)
				{
					foreach (Pawn pawn2 in travellers)
					{
						FECompCorrection.CheckPawnsGear(pawn2, def, ref count, ref changed);
					}
				}
			}
			Log.Message("Firewarden checked: " + count.ToString() + " FEs, changing: " + changed.ToString(), false);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002320 File Offset: 0x00000520
		private static void CheckPawnsGear(Pawn pawn, ThingDef def, ref int count, ref int changed)
		{
			if ((pawn?.equipment) != null)
			{
				List<ThingWithComps> equips = pawn.equipment.AllEquipmentListForReading;
				if (equips.Count > 0)
				{
					foreach (ThingWithComps equip in equips)
					{
						if (equip.def == def && equip.TryGetComp<CompFWData>() == null)
						{
							FECompCorrection.AddCompToThing(equip, ref changed);
							count++;
						}
					}
				}
			}
			if ((pawn?.inventory) != null)
			{
				List<Thing> invs = pawn.inventory.innerContainer.InnerListForReading;
				if (invs.Count > 0)
				{
					foreach (Thing inv in invs)
					{
						if (inv.def == def && inv.TryGetComp<CompFWData>() == null)
						{
							FECompCorrection.AddCompToThing(inv, ref changed);
							count++;
						}
					}
				}
			}
			Thing thing;
			if (pawn == null)
			{
				thing = null;
			}
			else
			{
				Pawn_CarryTracker carryTracker = pawn.carryTracker;
				thing = (carryTracker?.CarriedThing);
			}
			Thing carried = thing;
			if (carried != null && carried.def == def && carried.TryGetComp<CompFWData>() == null)
			{
				FECompCorrection.AddCompToThing(carried, ref changed);
				count++;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002468 File Offset: 0x00000668
		private static void AddCompToThing(Thing thing, ref int changed)
		{
            if (thing is ThingWithComps TWC)
            {
                try
                {
                    ThingComp thingComp = (ThingComp)Activator.CreateInstance(typeof(CompFWData));
                    thingComp.parent = TWC;
                    CompProperties comp = thingComp?.props;
                    if (comp != null)
                    {
                        thingComp.Initialize(comp);
                    }
                }
                catch (Exception arg)
                {
                    Log.Error("Could not instantiate or initialize a ThingComp: " + arg, false);
                }
                if (TWC.TryGetComp<CompFWData>() != null)
                {
                    changed++;
                }
            }
        }
	}
}
