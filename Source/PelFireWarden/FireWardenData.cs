using System.Collections.Generic;
using Verse;

namespace PelFireWarden
{
    // Token: 0x02000011 RID: 17
    public class FireWardenData : ThingWithComps
    {
        // Token: 0x04000016 RID: 22
        public static readonly string FEDefName = "Gun_Fire_Ext";

        // Token: 0x04000015 RID: 21
        public int FEFoamUses = 100;

        // Token: 0x04000013 RID: 19
        public int FWPawnID;

        // Token: 0x04000014 RID: 20
        public string FWPrimDef = "N";

        // Token: 0x04000012 RID: 18
        public string FWSwapType = "N";

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600003A RID: 58 RVA: 0x00003431 File Offset: 0x00001631
        public CompFWData FWComp => GetComp<CompFWData>();

        // Token: 0x0600003B RID: 59 RVA: 0x0000343C File Offset: 0x0000163C
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

        // Token: 0x0600003C RID: 60 RVA: 0x000034CC File Offset: 0x000016CC
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

        // Token: 0x0600003D RID: 61 RVA: 0x00003556 File Offset: 0x00001756
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
}