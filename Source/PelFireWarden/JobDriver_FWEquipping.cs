using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PelFireWarden;

public class JobDriver_FWEquipping : JobDriver
{
    private static readonly string FEDefName = "Gun_Fire_Ext";

    private static readonly string FBDefName = "Firebeater";

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    private bool IsFW(Pawn p)
    {
        return p.workSettings.WorkIsActive(FWWorkTypeDef.PelFireWarden) &&
               !p.story.traits.HasTrait(TraitDefOf.Pyromaniac);
    }

    private static bool FWHasFE(Pawn p)
    {
        if (p.equipment.Primary != null)
        {
            if (p.equipment.Primary.def.defName == FEDefName && FWFoamUtility.HasFEFoam(p.equipment.Primary))
            {
                return true;
            }
        }
        else if (!p.inventory.innerContainer.NullOrEmpty())
        {
            using var enumerator = p.inventory.innerContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null && enumerator.Current.def.defName == FEDefName &&
                    FWFoamUtility.HasFEFoam(p.equipment.Primary))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool FWHasFB(Pawn p)
    {
        if (p.equipment.Primary != null)
        {
            if (p.equipment.Primary.def.defName == FBDefName)
            {
                return true;
            }
        }
        else if (!p.inventory.innerContainer.NullOrEmpty())
        {
            using var enumerator = p.inventory.innerContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current?.def.defName == FBDefName)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var toilEquipGoto = new Toil();
        var toilEquip = new Toil();
        if (!Controller.Settings.EquippingDone)
        {
            yield break;
        }

        var ThingToGrab = job.GetTarget(TargetIndex.A).Thing;
        if (ThingToGrab == null)
        {
            yield break;
        }

        toilEquipGoto.initAction = delegate
        {
            if (Map.reservationManager.CanReserve(pawn, ThingToGrab))
            {
                pawn.Reserve(ThingToGrab, job);
            }

            pawn.pather.StartPath(ThingToGrab, PathEndMode.OnCell);
        };
        toilEquipGoto.tickAction = delegate { };
        toilEquipGoto.AddFailCondition(() => !IsFW(pawn));
        toilEquipGoto.AddFailCondition(() => FWHasFE(pawn) && ThingToGrab.def.defName == FEDefName);
        toilEquipGoto.AddFailCondition(() => FWHasFB(pawn) && ThingToGrab.def.defName == FBDefName);
        toilEquipGoto.FailOn(ThingToGrab.DestroyedOrNull);
        toilEquipGoto.defaultCompleteMode = ToilCompleteMode.PatherArrival;
        yield return toilEquipGoto;

        toilEquip.initAction = delegate
        {
            var FEGrabWithComps = (ThingWithComps)ThingToGrab;
            ThingWithComps FEGrabbed;
            if (FEGrabWithComps.def.stackLimit > 1 && FEGrabWithComps.stackCount > 1)
            {
                FEGrabbed = (ThingWithComps)FEGrabWithComps.SplitOff(1);
            }
            else
            {
                FEGrabbed = FEGrabWithComps;
                FEGrabbed.DeSpawn();
            }

            var returnType = "N";
            var pawnIDNumber = 0;
            var primDef = "N";
            if (FEGrabbed.def.defName == FEDefName || FEGrabbed.def.defName == FBDefName)
            {
                ((FireWardenData)FEGrabbed).FWSwapType = returnType;
                (FEGrabbed as FireWardenData).FWPawnID = pawnIDNumber;
                (FEGrabbed as FireWardenData).FWPrimDef = primDef;
            }

            pawn.inventory.innerContainer.TryAdd(FEGrabbed);
        };
        toilEquip.AddFailCondition(() => !IsFW(pawn));
        toilEquip.AddFailCondition(() => FWHasFE(pawn) && ThingToGrab.def.defName == FEDefName);
        toilEquip.AddFailCondition(() => FWHasFB(pawn) && ThingToGrab.def.defName == FBDefName);
        toilEquip.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
        yield return toilEquip;
    }
}