using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden
{
    // Token: 0x0200000D RID: 13
    public class Alert_FireWardenLacksFB : Alert
    {
        // Token: 0x0400000C RID: 12
        private static readonly string FBDefName = "Firebeater";

        // Token: 0x0600002E RID: 46 RVA: 0x000030E0 File Offset: 0x000012E0
        public Alert_FireWardenLacksFB()
        {
            defaultLabel = "PelFWLacksFB".Translate();
            defaultExplanation = "PelFWLacksFBDesc".Translate();
            defaultPriority = AlertPriority.High;
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600002C RID: 44 RVA: 0x00002FD2 File Offset: 0x000011D2
        private IEnumerable<Pawn> FWNoFB
        {
            get
            {
                foreach (var p in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (Controller.Settings.ShowEqWarnMsgs &&
                        (FWResearch.FireBeater.IsFinished && !FWResearch.FireExt.IsFinished ||
                         FWResearch.FireExt.IsFinished && Controller.Settings.BrawlerNotOK &&
                         p.story.traits.HasTrait(TraitDefOf.Brawler)) &&
                        !p.story.traits.HasTrait(TraitDefOf.Pyromaniac) &&
                        p.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) && !HasFB(p) && !p.Downed &&
                        !Controller.Settings.EquippingDone)
                    {
                        yield return p;
                    }
                }
            }
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00002FE4 File Offset: 0x000011E4
        private bool HasFB(Pawn FW)
        {
            if (FW.equipment.Primary == null)
            {
                var HasFBcheck = false;
                using var enumerator = FW.inventory.innerContainer.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current?.def.defName != FBDefName)
                    {
                        continue;
                    }

                    HasFBcheck = true;
                    break;
                }

                return HasFBcheck;
            }

            if (FW.equipment.Primary.def.defName == FBDefName)
            {
                return true;
            }

            {
                var HasFBcheck2 = false;
                using var enumerator = FW.inventory.innerContainer.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current?.def.defName != FBDefName)
                    {
                        continue;
                    }

                    HasFBcheck2 = true;
                    break;
                }

                return HasFBcheck2;
            }
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00003119 File Offset: 0x00001319
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(FWNoFB.ToList());
        }

        // Token: 0x02000028 RID: 40
        [DefOf]
        public static class FWWorkTypeDef
        {
            // Token: 0x04000055 RID: 85
            public static WorkTypeDef PelFireWarden;
        }

        // Token: 0x02000029 RID: 41
        [DefOf]
        public static class FWResearch
        {
            // Token: 0x04000056 RID: 86
            public static ResearchProjectDef FireBeater;

            // Token: 0x04000057 RID: 87
            public static ResearchProjectDef FireExt;
        }
    }
}