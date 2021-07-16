using Verse;

namespace PelFireWarden
{
    // Token: 0x02000003 RID: 3
    public class FWFoamUtility
    {
        // Token: 0x04000001 RID: 1
        public const string FEdefName = "Gun_Fire_Ext";

        // Token: 0x06000008 RID: 8 RVA: 0x000024F0 File Offset: 0x000006F0
        public static void FEFillUp(Thing thing)
        {
            if (IsFEFoamThing(thing))
            {
                ((FireWardenData) thing).FEFoamUses = ((FireWardenData) thing).FWComp.Props.FEFoamUses;
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x0000251C File Offset: 0x0000071C
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

        // Token: 0x0600000A RID: 10 RVA: 0x000025D8 File Offset: 0x000007D8
        public static bool IsFEFoamThing(Thing thing)
        {
            return (thing as ThingWithComps).TryGetComp<CompFWData>() != null && thing.def.defName == "Gun_Fire_Ext";
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002601 File Offset: 0x00000801
        public static bool HasFEFoam(Thing thing)
        {
            return !IsFEFoamThing(thing) || ((FireWardenData) thing).FEFoamUses > 0;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000261C File Offset: 0x0000081C
        public static bool ReplaceFEFoam(Thing thing)
        {
            var replace = false;
            if (!IsFEFoamThing(thing))
            {
                return false;
            }

            var foamLevel = (float) ((FireWardenData) thing).FEFoamUses;
            var maxLevel = (float) ((FireWardenData) thing).FWComp.Props.FEFoamUses;
            if (maxLevel > 0f && foamLevel * 100f / maxLevel <= Controller.Settings.ReplacePct)
            {
                replace = true;
            }

            return replace;
        }
    }
}