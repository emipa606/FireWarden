using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden
{
    // Token: 0x0200000E RID: 14
    public class Alert_FireWardenLacksFE : Alert
    {
        // Token: 0x0400000D RID: 13
        private static readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x06000033 RID: 51 RVA: 0x000032CC File Offset: 0x000014CC
        public Alert_FireWardenLacksFE()
        {
            defaultLabel = "PelFWLacksFE".Translate();
            defaultExplanation = "PelFWLacksFEDesc".Translate();
            defaultPriority = AlertPriority.High;
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000031 RID: 49 RVA: 0x00003137 File Offset: 0x00001337
        private IEnumerable<Pawn> FWNoFE
        {
            get
            {
                foreach (var p in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (Controller.Settings.ShowEqWarnMsgs && FWResearch.FireExt.IsFinished &&
                        !p.story.traits.HasTrait(TraitDefOf.Pyromaniac) &&
                        (!p.story.traits.HasTrait(TraitDefOf.Brawler) || p.story.traits.HasTrait(TraitDefOf.Brawler) &&
                            !Controller.Settings.BrawlerNotOK) &&
                        p.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) && !HasFE(p) && !p.Downed &&
                        !Controller.Settings.EquippingDone)
                    {
                        yield return p;
                    }
                }
            }
        }

        // Token: 0x06000032 RID: 50 RVA: 0x00003148 File Offset: 0x00001348
        private bool HasFE(Pawn FW)
        {
            if (FW.equipment.Primary == null)
            {
                foreach (var invThing in FW.inventory.innerContainer)
                {
                    if (invThing.def.defName == FEDefName && FWFoamUtility.HasFEFoam(invThing))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (FW.equipment.Primary.def.defName != FEDefName)
            {
                var HasFEcheck = false;
                using var enumerator = FW.inventory.innerContainer.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current?.def.defName != FEDefName)
                    {
                        continue;
                    }

                    HasFEcheck = true;
                    break;
                }

                return HasFEcheck;
            }

            if (FWFoamUtility.HasFEFoam(FW.equipment.Primary))
            {
                return true;
            }

            foreach (var invThing2 in FW.inventory.innerContainer)
            {
                if (invThing2.def.defName == FEDefName && FWFoamUtility.HasFEFoam(invThing2))
                {
                    return true;
                }
            }

            return false;
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00003305 File Offset: 0x00001505
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(FWNoFE.ToList());
        }

        // Token: 0x0200002B RID: 43
        [DefOf]
        public static class FWWorkTypeDef
        {
            // Token: 0x0400005D RID: 93
            public static WorkTypeDef PelFireWarden;
        }

        // Token: 0x0200002C RID: 44
        [DefOf]
        public static class FWResearch
        {
            // Token: 0x0400005E RID: 94
            public static ResearchProjectDef FireExt;
        }
    }
}