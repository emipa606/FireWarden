using System;
using UnityEngine;
using Verse;

namespace PelFireWarden
{
	// Token: 0x02000012 RID: 18
	public class Settings : ModSettings
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000035A0 File Offset: 0x000017A0
		public void DoWindowContents(Rect canvas)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = canvas.width;
			listing_Standard.Begin(canvas);
			listing_Standard.Gap(10f);
			listing_Standard.CheckboxLabeled("FWrd.EquippingDone".Translate(), ref this.EquippingDone, null);
			listing_Standard.Gap(8f);
			listing_Standard.Label("FWrd.SearchRange".Translate() + "  " + this.SearchRange, -1f, null);
			this.SearchRange = Math.Round((double)listing_Standard.Slider((float)this.SearchRange, 25f, 75f), 2);
			Text.Font = GameFont.Tiny;
			listing_Standard.Label("          " + "FWrd.SearchRangeTip".Translate(), -1f, null);
			Text.Font = GameFont.Small;
			listing_Standard.Gap(8f);
			listing_Standard.CheckboxLabeled("FWrd.ShowEqWarnMsgs".Translate(), ref this.ShowEqWarnMsgs, null);
			listing_Standard.Gap(8f);
			listing_Standard.Label("FWrd.ReplacePct".Translate() + "  " + this.ReplacePct, -1f, null);
			this.ReplacePct = listing_Standard.Slider((float)(checked((int)this.ReplacePct)), 10f, 25f);
			listing_Standard.Gap(8f);
			listing_Standard.CheckboxLabeled("FWrd.SendReplaceMsgs".Translate(), ref this.SendReplaceMsgs, null);
			listing_Standard.Gap(8f);
			listing_Standard.Gap(8f);
			listing_Standard.CheckboxLabeled("FWrd.HandleAnyway".Translate(), ref this.HandleAnyway, null);
			listing_Standard.Gap(8f);
			listing_Standard.Label("FWrd.HandledRange".Translate() + "  " + this.HandledRange, -1f, null);
			this.HandledRange = Math.Round((double)listing_Standard.Slider((float)this.HandledRange, 1f, 15f), 2);
			Text.Font = GameFont.Tiny;
			listing_Standard.Label("          " + "FWrd.HandledRangeTip".Translate(), -1f, null);
			Text.Font = GameFont.Small;
			listing_Standard.Gap(8f);
			listing_Standard.Gap(8f);
			listing_Standard.CheckboxLabeled("FWrd.TooBrave".Translate(), ref this.TooBrave, null);
			Text.Font = GameFont.Tiny;
			listing_Standard.Label("          " + "FWrd.TooBraveTip".Translate(), -1f, null);
			Text.Font = GameFont.Small;
			listing_Standard.Gap(8f);
			listing_Standard.Gap(8f);
			listing_Standard.Label("FWrd.HowClose".Translate() + "  " + this.HowClose, -1f, null);
			this.HowClose = Math.Round((double)listing_Standard.Slider((float)this.HowClose, 50f, 100f), 2);
			Text.Font = GameFont.Tiny;
			listing_Standard.Label("          " + "FWrd.HowCloseTip".Translate(), -1f, null);
			Text.Font = GameFont.Small;
			listing_Standard.Gap(8f);
			listing_Standard.Gap(8f);
			listing_Standard.CheckboxLabeled("FWrd.BrawlerNotOK".Translate(), ref this.BrawlerNotOK, null);
			listing_Standard.Gap(8f);
			listing_Standard.End();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003938 File Offset: 0x00001B38
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.EquippingDone, "EquippingDone", false, false);
			Scribe_Values.Look<bool>(ref this.RestockingDone, "RestockingDone", false, false);
			Scribe_Values.Look<bool>(ref this.ReturnToSpot, "ReturnToSpot", false, false);
			Scribe_Values.Look<double>(ref this.SearchRange, "SearchRange", 50.0, false);
			Scribe_Values.Look<bool>(ref this.ShowEqWarnMsgs, "ShowEqWarnMsgs", true, false);
			Scribe_Values.Look<float>(ref this.ReplacePct, "ReplacePct", 25f, false);
			Scribe_Values.Look<bool>(ref this.SendReplaceMsgs, "SendReplaceMsgs", false, true);
			Scribe_Values.Look<bool>(ref this.HandleAnyway, "HandleAnyway", true, false);
			Scribe_Values.Look<double>(ref this.HandledRange, "HandledRange", 5.0, false);
			Scribe_Values.Look<bool>(ref this.TooBrave, "TooBrave", false, false);
			Scribe_Values.Look<double>(ref this.HowClose, "HowClose", 80.0, false);
			Scribe_Values.Look<bool>(ref this.BrawlerNotOK, "BrawlerNotOK", true, false);
		}

		// Token: 0x04000017 RID: 23
		public bool EquippingDone;

		// Token: 0x04000018 RID: 24
		public bool RestockingDone;

		// Token: 0x04000019 RID: 25
		public bool ReturnToSpot;

		// Token: 0x0400001A RID: 26
		public double SearchRange = 50.0;

		// Token: 0x0400001B RID: 27
		public bool ShowEqWarnMsgs = true;

		// Token: 0x0400001C RID: 28
		public float ReplacePct = 25f;

		// Token: 0x0400001D RID: 29
		public bool HandleAnyway = true;

		// Token: 0x0400001E RID: 30
		public double HandledRange = 5.0;

		// Token: 0x0400001F RID: 31
		public bool TooBrave;

		// Token: 0x04000020 RID: 32
		public double HowClose = 80.0;

		// Token: 0x04000021 RID: 33
		public bool BrawlerNotOK = true;

		// Token: 0x04000022 RID: 34
		public bool SendReplaceMsgs;
	}
}
