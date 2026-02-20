using RimWorld;
using Verse;

using UnityEngine;

using System;
using System.Reflection.Emit;
using static HarmonyLib.Code;
using System.Collections.Generic;
using Verse.Sound;
using LudeonTK;
using System.Text;
using System.Linq;

namespace Strinova
{
    [StaticConstructorOnStartup]
    public class Gizmo_SuperstringShield : Gizmo
    {
        private Comp_SuperstringShield shield;
        public Gizmo_SuperstringShield(Comp_SuperstringShield shield)
        {
            this.shield = shield;
        }
        public override float GetWidth(float maxWidth)
        {
            return 200f;
        }

        public float getHeight()
        {
            return 75f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(rect);

            float fillPercent = shield.nowShield / shield.maxShield;

            Rect barRect = rect.ContractedBy(10f);
            barRect.y += 30f;
            barRect.height = 20f;

            Widgets.FillableBar(
                barRect,
                fillPercent,
                SolidColorMaterials.NewSolidColorTexture(new Color(0.3f, 0.7f, 1f)),
                SolidColorMaterials.NewSolidColorTexture(new Color(0.15f, 0.15f, 0.15f)),
                false
            );

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(
                rect,
                $"Shield\n\n{shield.nowShield:F0} / {shield.maxShield:F0}"
            );
            Text.Anchor = TextAnchor.UpperLeft;

            return new GizmoResult(GizmoState.Clear);
        }
    }
}
