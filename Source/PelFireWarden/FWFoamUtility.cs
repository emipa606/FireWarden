using Verse;

namespace PelFireWarden;

public class FWFoamUtility
{
    public const string FEdefName = "Gun_Fire_Ext";

    public static void FEFillUp(Thing thing)
    {
        if (IsFEFoamThing(thing))
        {
            ((FireWardenData)thing).FEFoamUses = ((FireWardenData)thing).FWComp.Props.FEFoamUses;
        }
    }

    public static Thing GetFE(Pawn pawn)
    {
        bool hasEquipment;
        if (pawn == null)
        {
            hasEquipment = false;
        }
        else
        {
            var equipment = pawn.equipment;
            hasEquipment = equipment?.Primary != null;
        }

        if (hasEquipment)
        {
            Thing chkPrim = pawn.equipment.Primary;
            if (IsFEFoamThing(chkPrim))
            {
                return chkPrim;
            }
        }

        if (pawn != null && !pawn.inventory.innerContainer.Any)
        {
            return null;
        }

        if (pawn == null)
        {
            return null;
        }

        var invList = pawn.inventory.innerContainer.InnerListForReading;
        if (invList.Count <= 0)
        {
            return null;
        }

        foreach (var chkInv in invList)
        {
            if (IsFEFoamThing(chkInv))
            {
                return chkInv;
            }
        }

        return null;
    }

    public static bool IsFEFoamThing(Thing thing)
    {
        return (thing as ThingWithComps).TryGetComp<CompFWData>() != null && thing.def.defName == "Gun_Fire_Ext";
    }

    public static bool HasFEFoam(Thing thing)
    {
        return !IsFEFoamThing(thing) || ((FireWardenData)thing).FEFoamUses > 0;
    }

    public static bool ReplaceFEFoam(Thing thing)
    {
        var replace = false;
        if (!IsFEFoamThing(thing))
        {
            return false;
        }

        var foamLevel = (float)((FireWardenData)thing).FEFoamUses;
        var maxLevel = (float)((FireWardenData)thing).FWComp.Props.FEFoamUses;
        if (maxLevel > 0f && foamLevel * 100f / maxLevel <= Controller.Settings.ReplacePct)
        {
            replace = true;
        }

        return replace;
    }
}