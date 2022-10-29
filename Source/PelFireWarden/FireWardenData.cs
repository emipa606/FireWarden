using System.Collections.Generic;
using Verse;

namespace PelFireWarden;

public class FireWardenData : ThingWithComps
{
    public static readonly string FEDefName = "Gun_Fire_Ext";

    public int FEFoamUses = 100;

    public int FWPawnID;

    public string FWPrimDef = "N";

    public string FWSwapType = "N";

    public CompFWData FWComp => GetComp<CompFWData>();

    public override void PostMake()
    {
        base.PostMake();
        if (FWComp.Props.FWSwapType != null)
        {
            FWSwapType = FWComp.Props.FWSwapType;
        }

        FWPawnID = FWComp.Props.FWPawnID;
        if (FWComp.Props.FWPrimDef != null)
        {
            FWPrimDef = FWComp.Props.FWPrimDef;
        }

        FEFoamUses = FWComp.Props.FEFoamUses;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref FWSwapType, "FWSwapType", "N");
        Scribe_Values.Look(ref FWPawnID, "FWPawnID");
        Scribe_Values.Look(ref FWPrimDef, "FWPrimDef", "N");
        Scribe_Values.Look(ref FEFoamUses, "FEFoamUses", 100);
        FWSwapType ??= "N";
        FWPrimDef ??= "N";
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        yield return new Gizmo_FEFoamStatus
        {
            FE = this
        };
        foreach (var item in base.GetGizmos())
        {
            yield return item;
        }
    }
}