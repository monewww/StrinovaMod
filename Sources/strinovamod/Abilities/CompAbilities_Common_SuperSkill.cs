using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Strinova
{
    public class CompAbilities_Common_SuperSkill : CompAbilityEffect
    {
        public Pawn pawn => this.parent.pawn;

        private Comp_Superstring_Energy EnergyComp => pawn.GetComp<Comp_Superstring_Energy>();

        public override bool GizmoDisabled(out string reason)
        {
            var comp = EnergyComp;
            if (comp == null || comp.energy < comp.maxEnergy)
            {
                float cur = comp?.energy ?? 0f;
                float max = comp?.maxEnergy ?? 1000f;
                reason = $"能量不足：{cur:F0} / {max:F0}";
                return true;
            }
            reason = null;
            return false;
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!base.CanApplyOn(target, dest)) return false;
            var comp = EnergyComp;
            return comp != null && comp.energy >= comp.maxEnergy;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var comp = EnergyComp;
            if (comp == null || !comp.Consume(comp.maxEnergy))
            {
                Messages.Message("Not enough energy!", MessageTypeDefOf.RejectInput);
                return;
            }
            base.Apply(target, dest);
        }
    }
}
