using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobGiver_FEAutoReplace : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        if (!pawn.IsColonistPlayerControlled)
        {
            return null;
        }

        if (pawn.InMentalState)
        {
            return null;
        }

        if (pawn.Map == null)
        {
            return null;
        }

        var jobdef = DefDatabase<JobDef>.GetNamed("FEReplace");
        if (pawn.CurJobDef == jobdef)
        {
            return null;
        }

        var FE = FWFoamUtility.GetFE(pawn);
        if (FE == null || !FWFoamUtility.ReplaceFEFoam(FE))
        {
            return null;
        }

        FindBestReplace(pawn, FE.def, out var targ);
        if (targ != null)
        {
            return new Job(jobdef, targ, FE);
        }

        return null;
    }

    private void FindBestReplace(Pawn FW, ThingDef FEItem, out Thing targ)
    {
        targ = null;
        if (FW?.Map == null)
        {
            return;
        }

        var listFE = FW.Map.listerThings.ThingsOfDef(FEItem);
        var needed = 1;
        if (listFE.Count <= 0)
        {
            return;
        }

        Thing besttarg = null;
        var bestpoints = 0f;
        foreach (var targchk in listFE)
        {
            if (targchk.IsForbidden(FW) || targchk?.Faction != null && !targchk.Faction.IsPlayer ||
                !FW.CanReserveAndReach(targchk, PathEndMode.ClosestTouch, Danger.None) ||
                !FWFoamUtility.IsFEFoamThing(targchk) || FWFoamUtility.ReplaceFEFoam(targchk))
            {
                continue;
            }

            float targpoints = 0;
            if (targchk != null && targchk.stackCount > needed)
            {
                targpoints = targchk.stackCount / FW.Position.DistanceTo(targchk.Position);
            }
            else
            {
                if (targchk != null)
                {
                    targpoints = targchk.stackCount / (FW.Position.DistanceTo(targchk.Position) * 2f);
                }
            }

            if (!(targpoints > bestpoints))
            {
                continue;
            }

            besttarg = targchk;
            bestpoints = targpoints;
        }

        if (besttarg != null)
        {
            targ = besttarg;
        }
    }
}