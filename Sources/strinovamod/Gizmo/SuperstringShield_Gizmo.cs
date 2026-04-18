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
            this.Order = 201f;
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

            float now = shield.nowShield;
            float temp = shield.tempShield;
            float max = shield.maxShield;

            float total = Mathf.Max(max, now + temp);

            float nowPercent = Mathf.Clamp01(now / total);
            float tempPercent = Mathf.Clamp01(temp / total);

            Rect barRect = rect.ContractedBy(10f);
            barRect.y += 30f;
            barRect.height = 20f;

            // --- 先画背景（未充满部分）---
            Widgets.DrawBoxSolid(barRect, new Color(0.15f, 0.15f, 0.15f));

            // --- 当前护盾 ---
            Rect nowRect = new Rect(
                barRect.x,
                barRect.y,
                barRect.width * nowPercent,
                barRect.height
            );
            Widgets.DrawBoxSolid(nowRect, new Color(0.3f, 0.7f, 1f));

            // --- 临时护盾（接在当前护盾后面）---
            Rect tempRect = new Rect(
                nowRect.xMax,
                barRect.y,
                barRect.width * tempPercent,
                barRect.height
            );
            Widgets.DrawBoxSolid(tempRect, new Color(0.6f, 0.85f, 1f)); // 更亮一点区分

            // --- 文字 ---
            Text.Anchor = TextAnchor.MiddleCenter;
            if(temp > 1)
            {
                Widgets.Label(rect, $"Shield\n\n{now:F0} + {temp:F0} / {max:F0}");
            }
            else
            {
                Widgets.Label(rect, $"Shield\n\n{now:F0} / {max:F0}");
            }

                Text.Anchor = TextAnchor.UpperLeft;

            return new GizmoResult(GizmoState.Clear);
        }
    }
}
