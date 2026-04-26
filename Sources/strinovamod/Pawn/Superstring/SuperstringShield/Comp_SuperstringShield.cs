using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Strinova
{
    public class tempAutoGenShield : IExposable
    {
        public float shieldAmount;
        public int autoGengap;
        public int startTime;
        public int existTime;
        public int lastGenTime;
        public bool isInterruptible; //暂时没写对应处理逻辑

        public tempAutoGenShield(float shieldAmount, int existTime, int autoGengap = 120, bool isInterruptible = false)
        {
            this.shieldAmount = shieldAmount;
            this.existTime = existTime;
            this.autoGengap = autoGengap;
            startTime = Find.TickManager.TicksGame;
            lastGenTime = Find.TickManager.TicksGame;
            this.isInterruptible = isInterruptible;
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref shieldAmount, "shieldAmount");
            Scribe_Values.Look(ref autoGengap, "autoGengap");
            Scribe_Values.Look(ref existTime, "existTime");
            Scribe_Values.Look(ref lastGenTime, "lastGenTime");
            Scribe_Values.Look(ref startTime, "startTime");
            Scribe_Values.Look(ref isInterruptible, "isInterruptible");
        }
    }
    public class Comp_SuperstringShield : ThingComp
    {
        

        public float maxShield = 80f;
        public float tempShield = 0f;
        public int tempShieldLastTime = 0;
        public int tempShieldLastTime_fault = 1200;
        public float nowShield = 80f;
        public float autoGenShield = 2f; // 每两秒回盾量
        public List<tempAutoGenShield> tempAutoGenShields = new List<tempAutoGenShield>();
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

        public void AddTempAutoGenShield(tempAutoGenShield tempAutoGenShield)
        {
            tempAutoGenShields.Add(tempAutoGenShield);
        }

        private void Action_AbsorbedDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            //先给大招加能量
            var energyComp = GetPawn.GetComp<Comp_Superstring_Energy>();
            if (energyComp != null)
            {
                energyComp.AddEnergy(30f);
            }
            absorbed = false;
            float damageAmount = dinfo.Amount;
            if (this.tempShield > damageAmount)
            {
                this.tempShield -= damageAmount;
                absorbed = true;
            }
            else
            {
                damageAmount -= this.tempShield;
                this.tempShield = 0;
                this.tempShieldLastTime = 0;

                //临时护甲不足，继续判断现在护甲
                if (this.nowShield >= damageAmount)
                {
                    this.nowShield -= damageAmount;
                    absorbed = true;
                }
                else
                {
                    if (this.nowShield > 0)
                    {
                        float actDamage = dinfo.Amount - this.nowShield;
                        this.nowShield = 0;
                        dinfo.SetAmount(actDamage);
                    }
                }
            }
            
            return;
        }


        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if ((GetPawn == null || !GetPawn.Spawned || GetPawn.Dead || GetPawn.IsMutant)) return;
            //判断护盾是否存在
            if (nowShield + tempShield <= 0f) return;
            Action_AbsorbedDamage(ref dinfo, out absorbed);
            lastDamageTick = Find.TickManager.TicksGame;
            return;
        }

        public override void CompTick()
        {
            base.CompTick();
            int curTick = Find.TickManager.TicksGame;
            if (GetPawn == null || !GetPawn.Spawned || GetPawn.IsMutant) return;
            //5秒未受伤自动恢复护盾
            if (canAutoGen && nowShield < maxShield && curTick - lastDamageTick > 300)
            {
                if (curTick - lastGenTick > 120)
                {
                    nowShield += autoGenShield;
                    if (nowShield > maxShield)
                        nowShield = maxShield;
                    lastGenTick = curTick;
                }

            }

            for (int i = tempAutoGenShields.Count - 1; i >= 0; i--) // 倒序遍历（方便删除）
            {
                var shield = tempAutoGenShields[i];

                // --- 判断是否过期 ---
                if (curTick - shield.startTime > shield.existTime)
                {
                    tempAutoGenShields.RemoveAt(i);
                    continue;
                }

                // --- 自动生成 ---
                if (curTick - shield.lastGenTime >= shield.autoGengap)
                {
                    nowShield += shield.shieldAmount;
                    if (nowShield > maxShield)
                        nowShield = maxShield;
                    // 更新生成时间
                    shield.lastGenTime = curTick;
                }
            }

            if (tempShieldLastTime > 0)
            {
                tempShieldLastTime -= 1;
            }
            else {
                tempShield = 0f;
            }
        }

        private void Draw_Shield()
        {
            if (this.nowShield <= 0f) return;


            Vector3 drawPos = this.GetPawn.DrawPos;
            drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor();

            float size = this.GetPawn.BodySize * 2f;

            Matrix4x4 matrix = Matrix4x4.TRS(
                drawPos,
                Quaternion.identity,
                new Vector3(size, 1f, size)
            );
            string actualGraphicPath = Props.shieldGraphicPath;

            if (nowShield/maxShield > 0.6f) actualGraphicPath += "_0";
            else if (nowShield/maxShield > 0.3f) actualGraphicPath += "_1";
            else actualGraphicPath += "_2";

            Material baseMat = MaterialPool.MatFrom(
                actualGraphicPath,
                ShaderDatabase.Transparent
            );

            Material bubbleMat = new Material(baseMat);


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

            var energyComp = pawn.GetComp<Comp_Superstring_Energy>();
            yield return new Gizmo_SuperstringStatus(this, energyComp);
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.nowShield, "nowShield", 0f, false);
            Scribe_Values.Look<int>(ref this.lastGenTick, "lastGenTick", 0, false);
            Scribe_Values.Look<int>(ref this.lastDamageTick, "lastDamageTick", 0, false);
            Scribe_Values.Look<float>(ref this.tempShield, "tempShield", 0f, false);
            Scribe_Values.Look<int>(ref this.tempShieldLastTime, "tempShieldLastTime", 0, false);
            Scribe_Collections.Look(ref tempAutoGenShields, "tempAutoGenShields", LookMode.Deep);
        }
    }
}
