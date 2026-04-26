using RimWorld;
using Strinova;
using UnityEngine;
using Verse;

namespace Strinova
{
    [StaticConstructorOnStartup]
    public class Gizmo_SuperstringStatus : Gizmo
    {
        private Comp_SuperstringShield shield;
        private Comp_Superstring_Energy energy;

        public Gizmo_SuperstringStatus(Comp_SuperstringShield shield, Comp_Superstring_Energy energy)
        {
            this.shield = shield;
            this.energy = energy;
            this.Order = 201f;
        }

        public override float GetWidth(float maxWidth) => 200f;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 90f);
            Widgets.DrawWindowBackground(rect);

            float innerX = rect.x + 10f;
            float innerW = rect.width - 20f;

            // --- 护盾条 ---
            float nowS = shield.nowShield;
            float tempS = shield.tempShield;
            float maxS = shield.maxShield;
            float totalS = Mathf.Max(maxS, nowS + tempS);
            float nowSPct = Mathf.Clamp01(nowS / totalS);
            float tempSPct = Mathf.Clamp01(tempS / totalS);

            Rect shieldBarBg = new Rect(innerX, rect.y + 18f, innerW, 14f);
            Widgets.DrawBoxSolid(shieldBarBg, new Color(0.15f, 0.15f, 0.15f));
            Widgets.DrawBoxSolid(new Rect(shieldBarBg.x, shieldBarBg.y, shieldBarBg.width * nowSPct, shieldBarBg.height), new Color(0.3f, 0.7f, 1f));
            Widgets.DrawBoxSolid(new Rect(shieldBarBg.x + shieldBarBg.width * nowSPct, shieldBarBg.y, shieldBarBg.width * tempSPct, shieldBarBg.height), new Color(0.6f, 0.85f, 1f));

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(new Rect(innerX, rect.y + 4f, innerW, 14f), "Shield");
            Text.Anchor = TextAnchor.MiddleRight;
            string shieldText = tempS > 1f ? $"{nowS:F0}+{tempS:F0}/{maxS:F0}" : $"{nowS:F0}/{maxS:F0}";
            Widgets.Label(new Rect(innerX, rect.y + 4f, innerW, 14f), shieldText);

            // --- 能量条 ---
            float nowE = energy.energy;
            float maxE = energy.maxEnergy;
            float nowEPct = Mathf.Clamp01(nowE / maxE);

            Rect energyBarBg = new Rect(innerX, rect.y + 56f, innerW, 14f);
            Widgets.DrawBoxSolid(energyBarBg, new Color(0.15f, 0.15f, 0.15f));
            Widgets.DrawBoxSolid(new Rect(energyBarBg.x, energyBarBg.y, energyBarBg.width * nowEPct, energyBarBg.height), new Color(0.9f, 0.75f, 0.2f));

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(new Rect(innerX, rect.y + 42f, innerW, 14f), "Energy");
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(new Rect(innerX, rect.y + 42f, innerW, 14f), $"{nowE:F0}/{maxE:F0}");

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            return new GizmoResult(GizmoState.Clear);
        }
    }
}
