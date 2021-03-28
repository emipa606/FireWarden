using System;
using UnityEngine;
using Verse;

namespace PelFireWarden
{
    // Token: 0x02000012 RID: 18
    public class Settings : ModSettings
    {
        // Token: 0x04000021 RID: 33
        public bool BrawlerNotOK = true;

        // Token: 0x04000017 RID: 23
        public bool EquippingDone;

        // Token: 0x0400001D RID: 29
        public bool HandleAnyway = true;

        // Token: 0x0400001E RID: 30
        public double HandledRange = 5.0;

        // Token: 0x04000020 RID: 32
        public double HowClose = 80.0;

        // Token: 0x0400001C RID: 28
        public float ReplacePct = 25f;

        // Token: 0x04000018 RID: 24
        public bool RestockingDone;

        // Token: 0x04000019 RID: 25
        public bool ReturnToSpot;

        // Token: 0x0400001A RID: 26
        public double SearchRange = 50.0;

        // Token: 0x04000022 RID: 34
        public bool SendReplaceMsgs;

        // Token: 0x0400001B RID: 27
        public bool ShowEqWarnMsgs = true;

        // Token: 0x0400001F RID: 31
        public bool TooBrave;

        // Token: 0x06000041 RID: 65 RVA: 0x000035A0 File Offset: 0x000017A0
        public void DoWindowContents(Rect canvas)
        {
            var listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = canvas.width;
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("FWrd.EquippingDone".Translate(), ref EquippingDone);
            listing_Standard.Gap(8f);
            listing_Standard.Label("FWrd.SearchRange".Translate() + "  " + SearchRange);
            SearchRange = Math.Round(listing_Standard.Slider((float) SearchRange, 25f, 75f), 2);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("          " + "FWrd.SearchRangeTip".Translate());
            Text.Font = GameFont.Small;
            listing_Standard.Gap(8f);
            listing_Standard.CheckboxLabeled("FWrd.ShowEqWarnMsgs".Translate(), ref ShowEqWarnMsgs);
            listing_Standard.Gap(8f);
            listing_Standard.Label("FWrd.ReplacePct".Translate() + "  " + ReplacePct);
            ReplacePct = listing_Standard.Slider(checked((int) ReplacePct), 10f, 25f);
            listing_Standard.Gap(8f);
            listing_Standard.CheckboxLabeled("FWrd.SendReplaceMsgs".Translate(), ref SendReplaceMsgs);
            listing_Standard.Gap(8f);
            listing_Standard.Gap(8f);
            listing_Standard.CheckboxLabeled("FWrd.HandleAnyway".Translate(), ref HandleAnyway);
            listing_Standard.Gap(8f);
            listing_Standard.Label("FWrd.HandledRange".Translate() + "  " + HandledRange);
            HandledRange = Math.Round(listing_Standard.Slider((float) HandledRange, 1f, 15f), 2);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("          " + "FWrd.HandledRangeTip".Translate());
            Text.Font = GameFont.Small;
            listing_Standard.Gap(8f);
            listing_Standard.Gap(8f);
            listing_Standard.CheckboxLabeled("FWrd.TooBrave".Translate(), ref TooBrave);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("          " + "FWrd.TooBraveTip".Translate());
            Text.Font = GameFont.Small;
            listing_Standard.Gap(8f);
            listing_Standard.Gap(8f);
            listing_Standard.Label("FWrd.HowClose".Translate() + "  " + HowClose);
            HowClose = Math.Round(listing_Standard.Slider((float) HowClose, 50f, 100f), 2);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("          " + "FWrd.HowCloseTip".Translate());
            Text.Font = GameFont.Small;
            listing_Standard.Gap(8f);
            listing_Standard.Gap(8f);
            listing_Standard.CheckboxLabeled("FWrd.BrawlerNotOK".Translate(), ref BrawlerNotOK);
            listing_Standard.Gap(8f);
            listing_Standard.End();
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003938 File Offset: 0x00001B38
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EquippingDone, "EquippingDone");
            Scribe_Values.Look(ref RestockingDone, "RestockingDone");
            Scribe_Values.Look(ref ReturnToSpot, "ReturnToSpot");
            Scribe_Values.Look(ref SearchRange, "SearchRange", 50.0);
            Scribe_Values.Look(ref ShowEqWarnMsgs, "ShowEqWarnMsgs", true);
            Scribe_Values.Look(ref ReplacePct, "ReplacePct", 25f);
            Scribe_Values.Look(ref SendReplaceMsgs, "SendReplaceMsgs", false, true);
            Scribe_Values.Look(ref HandleAnyway, "HandleAnyway", true);
            Scribe_Values.Look(ref HandledRange, "HandledRange", 5.0);
            Scribe_Values.Look(ref TooBrave, "TooBrave");
            Scribe_Values.Look(ref HowClose, "HowClose", 80.0);
            Scribe_Values.Look(ref BrawlerNotOK, "BrawlerNotOK", true);
        }
    }
}