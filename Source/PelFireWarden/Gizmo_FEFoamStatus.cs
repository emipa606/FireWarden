using UnityEngine;
using Verse;

namespace PelFireWarden
{
    // Token: 0x02000004 RID: 4
    [StaticConstructorOnStartup]
    internal class Gizmo_FEFoamStatus : Gizmo
    {
        // Token: 0x04000006 RID: 6
        private const float ArrowScale = 0.5f;

        // Token: 0x04000003 RID: 3
        private static readonly Texture2D FullBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

        // Token: 0x04000004 RID: 4
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

        // Token: 0x04000005 RID: 5
        private static readonly Texture2D TargetLevelArrow =
            ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

        // Token: 0x04000002 RID: 2
        public ThingWithComps FE;

        // Token: 0x0600000E RID: 14 RVA: 0x0000267F File Offset: 0x0000087F
        public Gizmo_FEFoamStatus()
        {
            order = -305f;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002692 File Offset: 0x00000892
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // Token: 0x06000010 RID: 16 RVA: 0x0000269C File Offset: 0x0000089C
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(overRect);
            Rect rect2;
            var rect4 = rect2 = overRect.ContractedBy(6f);
            rect2.height = overRect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect2, "FE Foam level");
            var rect3 = rect4;
            rect3.yMin = rect2.yMin + (overRect.height / 2f) - 6f;
            rect3.height = (overRect.height / 2f) - 6f;
            var fillPercent = ((FireWardenData) FE).FEFoamUses / (float) ((FireWardenData) FE).FWComp.Props.FEFoamUses;
            Widgets.FillableBar(rect3, fillPercent, FullBarTex, EmptyBarTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect3,
                ((FireWardenData) FE)?.FEFoamUses.ToString("F0") + " / " +
                ((FireWardenData) FE)?.FWComp.Props.FEFoamUses.ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }
    }
}