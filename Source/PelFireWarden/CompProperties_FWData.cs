using Verse;

namespace PelFireWarden;

public class CompProperties_FWData : CompProperties
{
    public readonly int FEFoamUses = 100;

    public readonly string FWPrimDef = "N";

    public readonly string FWSwapType = "N";

    public int FWPawnID;

    public CompProperties_FWData()
    {
        compClass = typeof(CompFWData);
    }
}