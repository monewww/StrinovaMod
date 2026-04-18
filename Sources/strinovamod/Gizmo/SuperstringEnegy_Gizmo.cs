using RimWorld;
using Strinova;
using UnityEngine;
using Verse;

public class SuperstringEnegy_Gizmo_Ability : Gizmo
{
    private Comp_Superstring_Energy energy;
    private Ability ability;

    public SuperstringEnegy_Gizmo_Ability(Comp_Superstring_Energy energy, Ability ability)
    {
        this.energy = energy;
        this.ability = ability;
        this.Order = 201f;
    }

    public override float GetWidth(float maxWidth) => 140f;

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);

        Widgets.DrawWindowBackground(rect);

        float now = energy.energy;
        float max = energy.maxEnergy;
        float percent = Mathf.Clamp01(now / max);

        Rect barRect = rect.ContractedBy(10f);
        barRect.y += 10f;
        barRect.height = 20f;

        Widgets.DrawBoxSolid(barRect, new Color(0.15f, 0.15f, 0.15f));

        Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * percent, barRect.height);
        Widgets.DrawBoxSolid(fillRect, new Color(0.3f, 0.7f, 1f));

        // 🔥 是否可释放
        bool canCast = now >= max;

        // 点击
        if (Widgets.ButtonInvisible(rect))
        {
            if (canCast)
            {
                ability.verb.TryStartCastOn(ability.pawn);
                energy.energy = 0f;
            }
            else
            {
                Messages.Message("Not enough energy!", MessageTypeDefOf.RejectInput);
            }
        }

        // 灰掉效果
        if (!canCast)
        {
            Widgets.DrawBox(rect, 2); // 简单灰框
        }

        // 文本
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect, $"{now:F0} / {max:F0}");
        Text.Anchor = TextAnchor.UpperLeft;

        return new GizmoResult(GizmoState.Clear);
    }
}