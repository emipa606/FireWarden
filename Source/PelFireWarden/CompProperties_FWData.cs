using Verse;

namespace PelFireWarden;

public class CompProperties_FWData : CompProperties
{
    public int FEFoamUses = 100;

    public int FWPawnID;

    public string FWPrimDef = "N";

    public string FWSwapType = "N";

    public CompProperties_FWData()
    {
        compClass = typeof(CompFWData);
    }
}