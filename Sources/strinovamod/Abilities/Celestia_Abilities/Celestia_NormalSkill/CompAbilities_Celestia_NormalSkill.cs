using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Strinova
{
    public class CompAbilities_Celestia_NormalSkill : CompAbilityEffect
    {
        public new CompProperties_Celestia_NormalSkill Props => (CompProperties_Celestia_NormalSkill)this.props;
        public Pawn GetPawn => this.parent.pawn;


        private static void DoCelestia_NormalSkill_Fleck(Pawn caster, Pawn target)
        {
            if (caster.Map == null) return;

            ThingDef def = ThingDef.Named("Celestia_NormalSkill_Orb"); // XML里要定义

            Celestia_NormalSkill_Orb orb = (Celestia_NormalSkill_Orb)ThingMaker.MakeThing(def);
            orb.caster = caster;
            orb.target = target;

            GenSpawn.Spawn(orb, caster.Position, caster.Map);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target.Thing is Pawn pawn && pawn.kindDef == StrinovaPawnKindDefof.Superstring_Colonist)
            {
                if (pawn == null || !pawn.Spawned || pawn.DeadOrDowned || pawn.Faction != Faction.OfPlayer)
                {
                    Log.Message("Invalid target for Celestia's Normal Skill.".Translate());
                    return;
                }
                DoCelestia_NormalSkill_Fleck(GetPawn, pawn);
            }
            else
            {
                // 目标不符合 → 输出失败提示
                Messages.Message("Celestia's Normal Skill failed: please target alive superstring", MessageTypeDefOf.RejectInput, false);
            }
        }

    }

    public class Celestia_NormalSkill_Orb : ThingWithComps
    {
        public Pawn caster;
        public Pawn target;

        private bool returning = false;
        private float speed = 0.1f;
        private float leftSpeed = 0.08f;

        public bool awaken_1 = true;
        public bool awaken_2 = true;
        public bool awaken_3 = true;

        private Vector3 exactPos;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (caster != null)
            {
                exactPos = caster.DrawPos;   
            }
            else
            {
                exactPos = base.DrawPos;
            }
        }

        public void DoCelestia_NormalSkill_affact(Pawn pawn)
        {
            Comp_SuperstringShield comp = pawn.GetComp<Comp_SuperstringShield>();
            if (comp != null)
            {
                comp.tempShield += 35f;
                comp.tempShieldLastTime = comp.tempShieldLastTime_fault; //每次获得会刷新所有临时护甲持续时间，想分开计算就写成list

                // 一觉
                if (awaken_1)
                {
                    tempAutoGenShield tempAutoGenShield = new tempAutoGenShield(4,310,60); //310帧共恢复5*4点护盾
                    comp.AddTempAutoGenShield(tempAutoGenShield);
                }

                // 二觉
                //if (awaken_2)
                //{

                //}

                // 三觉
                if (awaken_3)
                {
                    comp.nowShield += 35;
                    if(comp.nowShield > comp.maxShield){
                        comp.nowShield = comp.maxShield;
                    }
                }

            }
            else
            {
                Log.Message("target is not surperstring".Translate());
            }

        }

        protected override void Tick()
        {
            base.Tick();
            if (caster == null || target == null || Destroyed)
            {
                Destroy();
                return;
            }

            Vector3 dest = returning ? caster.DrawPos : target.DrawPos;
            FleckCreationData data = FleckMaker.GetDataStatic(
                exactPos,
                Map,
                StrinovaFelckDefof.Celestia_NormalSkill_Tail // 可以换成别的
            );

            data.scale = 0.3f;
            data.velocitySpeed = 0f;
            Map.flecks.CreateFleck(data);

            // --- 前进 ---
            Vector3 dir = (dest - exactPos).normalized;

            exactPos += dir * speed + dir.RotatedBy(270) * leftSpeed;
            if (leftSpeed > 0)
            {
                leftSpeed -= 0.002f; // 每帧递减，直到为0
            }
            else
            {
                leftSpeed = 0;
            }

            // 同步逻辑位置（必须）
            Position = exactPos.ToIntVec3();

            // --- 到达检测 ---
            if ((exactPos - dest).sqrMagnitude < 0.2f)
            {
                if (!returning)
                {
                    // 到目标 
                    if (!target.Dead)
                    {
                        DoCelestia_NormalSkill_affact(target);
                    }

                    returning = true;
                    leftSpeed = 0.08f;
                }
                else
                {
                    DoCelestia_NormalSkill_affact(caster);
                    Destroy();
                }
            }
        }
        public override Vector3 DrawPos => exactPos;

    }
}
