using System;
using UnityEngine;
using Verse;

namespace PelFireWarden
{
	// Token: 0x02000004 RID: 4
	[StaticConstructorOnStartup]
	internal class Gizmo_FEFoamStatus : Gizmo
	{
		// Token: 0x0600000E RID: 14 RVA: 0x0000267F File Offset: 0x0000087F
		public Gizmo_FEFoamStatus()
		{
			this.order = -305f;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002692 File Offset: 0x00000892
		public override float GetWidth(float maxWidth)
		{
			return 140f;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000269C File Offset: 0x0000089C
		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
		{
			Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
			Widgets.DrawWindowBackground(overRect);
			Rect rect2;
			Rect rect4 = rect2 = overRect.ContractedBy(6f);
			rect2.height = overRect.height / 2f;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect2, "FE Foam level");
			Rect rect3 = rect4;
			rect3.yMin = rect2.yMin + overRect.height / 2f - 6f;
			rect3.height = overRect.height / 2f - 6f;
			float fillPercent = (float)(this.FE as FireWardenData).FEFoamUses / (float)(this.FE as FireWardenData).FWComp.Props.FEFoamUses;
			Widgets.FillableBar(rect3, fillPercent, Gizmo_FEFoamStatus.FullBarTex, Gizmo_FEFoamStatus.EmptyBarTex, false);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect3, (this.FE as FireWardenData).FEFoamUses.ToString("F0") + " / " + (this.FE as FireWardenData).FWComp.Props.FEFoamUses.ToString("F0"));
			Text.Anchor = TextAnchor.UpperLeft;
			return new GizmoResult(GizmoState.Clear);
		}

		// Token: 0x04000002 RID: 2
		public ThingWithComps FE;

		// Token: 0x04000003 RID: 3
		private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

		// Token: 0x04000004 RID: 4
		private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

		// Token: 0x04000005 RID: 5
		private static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated", true);

		// Token: 0x04000006 RID: 6
		private const float ArrowScale = 0.5f;
	}
}
