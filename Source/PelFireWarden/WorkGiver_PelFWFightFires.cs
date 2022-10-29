using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

internal class WorkGiver_PelFWFightFires : WorkGiver_Scanner
{
    private const int NearbyPawnRadius = 15;

    private const int MaxReservationCheckDistance = 15;

    private const float HandledDistance = 5f;

    private const string FEDefName = "Gun_Fire_Ext";

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.Fire);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Fire fire)
        {
            return false;
        }

        if (fire.parent is Pawn pawn2)
        {
            if (pawn2 == pawn)
            {
                return false;
            }

            if ((pawn2.Faction == pawn.Faction || pawn2.HostFaction == pawn.Faction ||
                 pawn2.HostFaction == pawn.HostFaction) && !pawn.Map.areaManager.Home[fire.Position] &&
                IntVec3Utility.ManhattanDistanceFlat(pawn.Position, pawn2.Position) > 15)
            {
                return false;
            }

            if (!pawn.CanReach(pawn2, PathEndMode.Touch, Danger.Deadly))
            {
                return false;
            }
        }
        else
        {
            if (pawn.WorkTagIsDisabled(WorkTags.Firefighting))
            {
                return false;
            }

            // Since pawns can use the extinguisher if they are incapable of violence now the following is not needed
            //if (pawn.WorkTagIsDisabled(WorkTags.Violent))
            //{
            //	return false;
            //}
            if (pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac))
            {
                return false;
            }

            if (!pawn.Map.areaManager.Home[fire.Position])
            {
                JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
                return false;
            }
        }

        if ((pawn.Position - fire.Position).LengthHorizontalSquared <= 225)
        {
            return !FireIsBeingHandled(fire, pawn);
        }

        LocalTargetInfo target = fire;
        if (!pawn.CanReserve(target, 1, -1, null, forced))
        {
            return false;
        }

        return !FireIsBeingHandled(fire, pawn);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(DefDatabase<JobDef>.GetNamed("PelFESimple"), t);
    }

    private static bool FireIsBeingHandled(Fire f, Pawn FW)
    {
        if (!f.Spawned)
        {
            return false;
        }

        var pawn = f.Map.reservationManager.FirstRespectedReserver(f, FW);
        if (pawn == null || pawn.Drafted || pawn.IsBurning())
        {
            return false;
        }

        if (Controller.Settings.HandleAnyway)
        {
            if (FW.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden))
            {
                if (FWHasFE(FW))
                {
                    return false;
                }

                if (FWHasFE(pawn) && pawn.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden))
                {
                    return pawn.Position.InHorDistOf(f.Position, (float)Controller.Settings.HandledRange);
                }

                return pawn.Position.InHorDistOf(f.Position, 5f);
            }

            if (FWHasFE(pawn) && pawn.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden))
            {
                return pawn.Position.InHorDistOf(f.Position, (float)Controller.Settings.HandledRange);
            }

            return pawn.Position.InHorDistOf(f.Position, 5f);
        }

        if (FWHasFE(pawn) && pawn.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden))
        {
            return pawn.Position.InHorDistOf(f.Position, (float)Controller.Settings.HandledRange);
        }

        return pawn.Position.InHorDistOf(f.Position, 5f);
    }

    private static bool FWHasFE(Pawn FW)
    {
        if (FW.equipment.Primary != null)
        {
            return FW.equipment.Primary.def.defName == "Gun_Fire_Ext" &&
                   FWFoamUtility.HasFEFoam(FW.equipment.Primary);
        }

        if (FW.inventory.innerContainer.NullOrEmpty())
        {
            return false;
        }

        foreach (var invFECheck in FW.inventory.innerContainer)
        {
            if (invFECheck.def.defName == "Gun_Fire_Ext" && FWFoamUtility.HasFEFoam(invFECheck))
            {
                return true;
            }
        }

        return false;
    }

    [DefOf]
    public static class FWWorkTypeDef
    {
        public static WorkTypeDef PelFireWarden;
    }
}