using System;
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
            FECompCorrection_Setup_FE();
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
        private static void FECompCorrection_Setup_FE()
        {
            ProxSetup_Comp(typeof(CompProperties_FWData), def => def.defName == FireWardenData.FEDefName);
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002088 File Offset: 0x00000288
        private static void ProxSetup_Comp(Type compType, Func<ThingDef, bool> qualifier)
        {
            var list = DefDatabase<ThingDef>.AllDefsListForReading.Where(qualifier).ToList();
            list.RemoveDuplicates();
            var count = 0;
            foreach (var def in list)
            {
                if (def.comps == null ||
                    def.comps.Any(c => c.GetType() == compType))
                {
                    continue;
                }

                def.comps.Add((CompProperties) Activator.CreateInstance(compType));
                count++;
            }

            Log.Message("Fire warden: Corrected " + count + " Fire extinguisher definitions.");
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002164 File Offset: 0x00000364
        private static void FECorrection_FE()
        {
            var count = 0;
            var changed = 0;
            var def = DefDatabase<ThingDef>.GetNamed(FireWardenData.FEDefName, false);
            if (def != null)
            {
                var maps = Find.Maps;
                if (maps.Count > 0)
                {
                    foreach (var map in maps)
                    {
                        var things = map.listerThings.ThingsOfDef(def);
                        if (things.Count > 0)
                        {
                            foreach (var thing in things)
                            {
                                if (thing.TryGetComp<CompFWData>() != null)
                                {
                                    continue;
                                }

                                AddCompToThing(thing, ref changed);
                                count++;
                            }
                        }

                        var pawns = map.mapPawns.AllPawns;
                        if (pawns.Count <= 0)
                        {
                            continue;
                        }

                        foreach (var pawn in pawns)
                        {
                            CheckPawnsGear(pawn, def, ref count, ref changed);
                        }
                    }
                }

                var travellers = Find.WorldPawns.AllPawnsAlive;
                if (travellers.Count > 0)
                {
                    foreach (var pawn2 in travellers)
                    {
                        CheckPawnsGear(pawn2, def, ref count, ref changed);
                    }
                }
            }

            Log.Message("Firewarden checked: " + count + " FEs, changing: " + changed);
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002320 File Offset: 0x00000520
        private static void CheckPawnsGear(Pawn pawn, ThingDef def, ref int count, ref int changed)
        {
            if (pawn?.equipment != null)
            {
                var equips = pawn.equipment.AllEquipmentListForReading;
                if (equips.Count > 0)
                {
                    foreach (var equip in equips)
                    {
                        if (equip.def != def || equip.TryGetComp<CompFWData>() != null)
                        {
                            continue;
                        }

                        AddCompToThing(equip, ref changed);
                        count++;
                    }
                }
            }

            if (pawn?.inventory != null)
            {
                var invs = pawn.inventory.innerContainer.InnerListForReading;
                if (invs.Count > 0)
                {
                    foreach (var inv in invs)
                    {
                        if (inv.def != def || inv.TryGetComp<CompFWData>() != null)
                        {
                            continue;
                        }

                        AddCompToThing(inv, ref changed);
                        count++;
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
                var carryTracker = pawn.carryTracker;
                thing = carryTracker?.CarriedThing;
            }

            var carried = thing;
            if (carried == null || carried.def != def || carried.TryGetComp<CompFWData>() != null)
            {
                return;
            }

            AddCompToThing(carried, ref changed);
            count++;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002468 File Offset: 0x00000668
        private static void AddCompToThing(Thing thing, ref int changed)
        {
            if (thing is not ThingWithComps TWC)
            {
                return;
            }

            try
            {
                var thingComp = (ThingComp) Activator.CreateInstance(typeof(CompFWData));
                thingComp.parent = TWC;
                var comp = thingComp.props;
                if (comp != null)
                {
                    thingComp.Initialize(comp);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Could not instantiate or initialize a ThingComp: " + arg);
            }

            if (TWC.TryGetComp<CompFWData>() != null)
            {
                changed++;
            }
        }
    }
}