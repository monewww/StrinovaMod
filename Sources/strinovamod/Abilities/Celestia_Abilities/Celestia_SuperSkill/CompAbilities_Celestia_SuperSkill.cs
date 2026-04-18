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
    public class CompAbilities_Celestia_SuperSkill : CompAbilities_Common_SuperSkill
    {
        public new CompProperties_Celestia_SuperSkill Props => (CompProperties_Celestia_SuperSkill)this.props;
        public Pawn GetPawn => this.parent.pawn;

        public void DoCelestia_SuperSkill_affact(Pawn pawn)
        {
            Comp_SuperstringShield comp = pawn.GetComp<Comp_SuperstringShield>();
            if (comp != null)
            {
                comp.tempShield += 100f;
                comp.tempShieldLastTime = comp.tempShieldLastTime_fault; //每次获得会刷新所有临时护甲持续时间，想分开计算就写成list

            }
            else
            {
                Log.Message("target is not surperstring".Translate());
            }

        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (target.Thing is Pawn pawn && pawn.kindDef == StrinovaPawnKindDefof.Superstring_Colonist)
            {
                if (pawn == null || !pawn.Spawned || pawn.DeadOrDowned || pawn.Faction != Faction.OfPlayer)
                {
                    Messages.Message("Invalid target", MessageTypeDefOf.RejectInput);
                    return;
                }

                Pawn caster = GetPawn;
                DoCelestia_SuperSkill_affact(caster);
                DoCelestia_SuperSkill_affact(pawn);
                // 记录原位置
                IntVec3 origin = caster.Position;
                caster.DeSpawn();

                // 生成召唤物
                ThingDef def = ThingDef.Named("Celestia_SuperSkill_Entity");

                var entity = (Celestia_SuperSkill_Entity)ThingMaker.MakeThing(def);
                entity.caster = caster;
                entity.target = pawn;
                entity.originPos = origin;

                GenSpawn.Spawn(entity, pawn.Position, pawn.Map);
            }
        }

    }

    public class Celestia_SuperSkill_Entity : ThingWithComps
    {
        public Pawn caster;
        public Pawn target;
        public IntVec3 originPos;

        private int lifeTime = 180; // 3秒（60tick=1秒）
        private int tick = 0;
        private Vector3 exactPos;
        public override Vector3 DrawPos => exactPos;

        private static FleckDef[] flecks = new FleckDef[]
        {
            DefDatabase<FleckDef>.GetNamed("Celestia_SuperSkill_fleck1"),
            DefDatabase<FleckDef>.GetNamed("Celestia_SuperSkill_fleck2"),
            DefDatabase<FleckDef>.GetNamed("Celestia_SuperSkill_fleck3"),
        };

        private void SpawnRandomFleck()
        {
            if (Map == null) return;

            FleckDef fleck = flecks.RandomElement();

            Vector3 pos = DrawPos + new Vector3(
                Rand.Range(-1f, 1f),
                0,
                Rand.Range(-1f, 1f)
            );

            FleckCreationData data = FleckMaker.GetDataStatic(
                pos,
                Map,
                fleck
            );

            data.scale = Rand.Range(0.15f, 0.4f);
            data.rotationRate = Rand.Range(-5f, 5f);

            Map.flecks.CreateFleck(data);
        }

        protected override void Tick()
        {
            base.Tick();
            exactPos = target.DrawPos + new Vector3(0, 0, 1);
            Position = exactPos.ToIntVec3();
            tick++;
            FleckMaker.Static(DrawPos, Map, FleckDefOf.PsycastAreaEffect);

            if (caster == null || target == null)
            {
                Destroy();
                return;
            }
            float chance = Mathf.Clamp01(tick / 1000f);
            // 0 → 1（3秒内从0%到18%）

            if (Rand.Chance(chance))
            {
                SpawnRandomFleck();
            }

            if (tick >= lifeTime)
            {
                ReturnToTarget();
            }
        }

        private void ReturnToTarget()
        {
            if (caster != null && target != null && target.Spawned)
            {
                GenSpawn.Spawn(caster, target.Position, target.Map);
                caster.drafter.Drafted = true; 
            }

            Destroy();
        }

        private void ReturnToOrigin()
        {
            if (caster != null && Map != null)
            {
                GenSpawn.Spawn(caster, originPos, Map);
                caster.drafter.Drafted = true;
            }

            Destroy();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;

            yield return new Command_Action
            {
                defaultLabel = "Cancel Recall",
                defaultDesc = "Return to original position",
                icon = TexCommand.CannotShoot, // 取消的图标
                action = () =>
                {
                    ReturnToOrigin();
                }
            };
        }

    }
}