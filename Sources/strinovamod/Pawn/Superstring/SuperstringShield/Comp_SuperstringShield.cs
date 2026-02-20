using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Strinova
{
    public class Comp_SuperstringShield : ThingComp
    {
        public float maxShield = 80f;
        public float nowShield = 80f;
        public float autoGenShield = 2f;
        public int lastGenTick = -9999;
        public bool canAutoGen = true;
        private int lastDamageTick = -9999;
        public CompProperties_SuperstringShield Props
        {
            get
            {
                return (CompProperties_SuperstringShield)this.props;
            }
        }

        public Pawn GetPawn
        {
            get
            {
                Pawn pawn = this.parent as Pawn;
                return pawn;
            }
        }

        private void Action_AbsorbedDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            float damageAmount = dinfo.Amount;
            if (this.nowShield > damageAmount)
            {
                this.nowShield -= damageAmount;
                absorbed = true;
            }
            //不足以抵消
            else
            {
                if (this.nowShield > 0)
                {
                    float actDamage = dinfo.Amount - this.nowShield;
                    this.nowShield = 0;
                    dinfo.SetAmount(actDamage);
                }
            }
            return;
        }


        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if ((GetPawn == null || !GetPawn.Spawned || GetPawn.Dead || GetPawn.IsMutant)) return;
            //判断护盾是否存在
            if (nowShield <= 0f) return;
            Action_AbsorbedDamage(ref dinfo, out absorbed);
            lastDamageTick = Find.TickManager.TicksGame;
            return;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (GetPawn == null || !GetPawn.Spawned || GetPawn.IsMutant) return;
            //5秒未受伤自动恢复护盾
            if (canAutoGen && nowShield < maxShield && Find.TickManager.TicksGame - lastDamageTick > 300)
            {
                if (Find.TickManager.TicksGame - lastGenTick > 120)
                {
                    nowShield += autoGenShield;
                    if (nowShield > maxShield)
                        nowShield = maxShield;
                    lastGenTick = Find.TickManager.TicksGame;
                }

            }
        }

        private void Draw_Shield()
        {
            Log.Message("Draw Shield");

            if (this.nowShield <= 0f) return;

            float alpha = Mathf.Clamp01(
                (this.nowShield * 0.6f + this.maxShield * 0.4f) / this.maxShield
            );

            Vector3 drawPos = this.GetPawn.DrawPos;
            drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor();

            float size = this.GetPawn.BodySize * 2f;

            Matrix4x4 matrix = Matrix4x4.TRS(
                drawPos,
                Quaternion.identity,
                new Vector3(size, 1f, size)
            );

            Material baseMat = MaterialPool.MatFrom(
                Props.shieldGraphicPath,
                ShaderDatabase.Transparent
            );

            Material bubbleMat = new Material(baseMat);

            Color color = bubbleMat.color;
            color.a = alpha;
            bubbleMat.color = color;

            Graphics.DrawMesh(MeshPool.plane10, matrix, bubbleMat, 0);
        }



        public override void PostDraw()
        {
            base.PostDraw();
            if (GetPawn != null && GetPawn.Spawned && !GetPawn.DeadOrDowned && GetPawn.GetPosture() == PawnPosture.Standing && !GetPawn.IsMutant)
            {
                if (this.nowShield > 0 ) Draw_Shield();
            }

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            Pawn pawn = GetPawn;
            if (pawn == null) yield break;
            if (pawn.Faction != Faction.OfPlayer) yield break;

            yield return new Gizmo_SuperstringShield(this);
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.nowShield, "nowShield", 0f, false);
            Scribe_Values.Look<int>(ref this.lastGenTick, "lastGenTick", 0, false);
            Scribe_Values.Look<int>(ref this.lastDamageTick, "lastDamageTick", 0, false);
        }
    }
}
