using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden;

public class Alert_FireWardenLacksFB : Alert
{
    private static readonly string FBDefName = "Firebeater";

    public Alert_FireWardenLacksFB()
    {
        defaultLabel = "PelFWLacksFB".Translate();
        defaultExplanation = "PelFWLacksFBDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

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

    public override AlertReport GetReport()
    {
        return AlertReport.CulpritsAre(FWNoFB.ToList());
    }

    [DefOf]
    public static class FWWorkTypeDef
    {
        public static WorkTypeDef PelFireWarden;
    }

    [DefOf]
    public static class FWResearch
    {
        public static ResearchProjectDef FireBeater;

        public static ResearchProjectDef FireExt;
    }
}