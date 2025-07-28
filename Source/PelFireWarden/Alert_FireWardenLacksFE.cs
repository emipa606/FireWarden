using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden;

public class Alert_FireWardenLacksFE : Alert
{
    private static readonly string FEDefName = "Gun_Fire_Ext";

    public Alert_FireWardenLacksFE()
    {
        defaultLabel = "PelFWLacksFE".Translate();
        defaultExplanation = "PelFWLacksFEDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

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

    public override AlertReport GetReport()
    {
        return AlertReport.CulpritsAre(FWNoFE.ToList());
    }

    [DefOf]
    private static class FWWorkTypeDef
    {
        public static WorkTypeDef PelFireWarden;
    }

    [DefOf]
    private static class FWResearch
    {
        public static ResearchProjectDef FireExt;
    }
}