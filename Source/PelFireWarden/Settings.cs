using System;
using UnityEngine;
using Verse;

namespace PelFireWarden;

public class Settings : ModSettings
{
    public bool BrawlerNotOK = true;

    public bool EquippingDone;

    public bool HandleAnyway = true;

    public double HandledRange = 5.0;

    public double HowClose = 80.0;

    public float ReplacePct = 25f;

    public bool RestockingDone;

    public bool ReturnToSpot;

    public double SearchRange = 50.0;

    public bool SendReplaceMsgs;

    public bool ShowEqWarnMsgs = true;

    public bool TooBrave;

    public void DoWindowContents(Rect canvas)
    {
        var listing_Standard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listing_Standard.Begin(canvas);
        listing_Standard.Gap(10f);
        listing_Standard.CheckboxLabeled("FWrd.EquippingDone".Translate(), ref EquippingDone);
        listing_Standard.Gap(8f);
        listing_Standard.Label("FWrd.SearchRange".Translate() + "  " + SearchRange);
        SearchRange = Math.Round(listing_Standard.Slider((float)SearchRange, 25f, 75f), 2);
        Text.Font = GameFont.Tiny;
        listing_Standard.Label("          " + "FWrd.SearchRangeTip".Translate());
        Text.Font = GameFont.Small;
        listing_Standard.Gap(8f);
        listing_Standard.CheckboxLabeled("FWrd.ShowEqWarnMsgs".Translate(), ref ShowEqWarnMsgs);
        listing_Standard.Gap(8f);
        listing_Standard.Label("FWrd.ReplacePct".Translate() + "  " + ReplacePct);
        ReplacePct = listing_Standard.Slider(checked((int)ReplacePct), 10f, 25f);
        listing_Standard.Gap(8f);
        listing_Standard.CheckboxLabeled("FWrd.SendReplaceMsgs".Translate(), ref SendReplaceMsgs);
        listing_Standard.Gap(8f);
        listing_Standard.Gap(8f);
        listing_Standard.CheckboxLabeled("FWrd.HandleAnyway".Translate(), ref HandleAnyway);
        listing_Standard.Gap(8f);
        listing_Standard.Label("FWrd.HandledRange".Translate() + "  " + HandledRange);
        HandledRange = Math.Round(listing_Standard.Slider((float)HandledRange, 1f, 15f), 2);
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
        HowClose = Math.Round(listing_Standard.Slider((float)HowClose, 50f, 100f), 2);
        Text.Font = GameFont.Tiny;
        listing_Standard.Label("          " + "FWrd.HowCloseTip".Translate());
        Text.Font = GameFont.Small;
        listing_Standard.Gap(8f);
        listing_Standard.Gap(8f);
        listing_Standard.CheckboxLabeled("FWrd.BrawlerNotOK".Translate(), ref BrawlerNotOK);
        if (Controller.currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("FWrd.CurrentModVersion".Translate(Controller.currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

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