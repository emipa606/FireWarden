using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden;

public class Alert_FireWardenIsPyro : Alert
{
    public Alert_FireWardenIsPyro()
    {
        defaultLabel = "PelFWIsPyro".Translate();
        defaultExplanation = "PelFWIsPyroDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

    private IEnumerable<Pawn> FWIsPyro
    {
        get
        {
            foreach (var p in PawnsFinder.AllMaps_FreeColonistsSpawned)
            {
                if (p.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) &&
                    p.story.traits.HasTrait(TraitDefOf.Pyromaniac))
                {
                    yield return p;
                }
            }
        }
    }

    public override AlertReport GetReport()
    {
        return AlertReport.CulpritsAre(FWIsPyro.ToList());
    }

    [DefOf]
    private static class FWWorkTypeDef
    {
        public static WorkTypeDef PelFireWarden;
    }
}