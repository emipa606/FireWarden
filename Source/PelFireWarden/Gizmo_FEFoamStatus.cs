using UnityEngine;
using Verse;

namespace PelFireWarden;

[StaticConstructorOnStartup]
internal class Gizmo_FEFoamStatus : Gizmo
{
    private const float ArrowScale = 0.5f;

    private static readonly Texture2D FullBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

    private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

    private static readonly Texture2D TargetLevelArrow =
        ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

    public ThingWithComps FE;

    public Gizmo_FEFoamStatus()
    {
        base.Order = -305f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 140f;
    }

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
        var fillPercent = ((FireWardenData)FE).FEFoamUses / (float)((FireWardenData)FE).FWComp.Props.FEFoamUses;
        Widgets.FillableBar(rect3, fillPercent, FullBarTex, EmptyBarTex, false);
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect3,
            $"{((FireWardenData)FE)?.FEFoamUses:F0} / {((FireWardenData)FE)?.FWComp.Props.FEFoamUses:F0}");
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}