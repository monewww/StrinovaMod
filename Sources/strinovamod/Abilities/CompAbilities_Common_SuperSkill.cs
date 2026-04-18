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
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!base.CanApplyOn(target, dest)) return false;

            var comp = pawn.GetComp<Comp_Superstring_Energy>();
            if (comp == null) return false;

            return comp.energy >= 1000f;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var comp = pawn.GetComp<Comp_Superstring_Energy>();

            if (comp == null || !comp.Consume(1000f))
            {
                Messages.Message("Not enough energy!", MessageTypeDefOf.RejectInput);
                return;
            }

            base.Apply(target, dest);
        }
    }
}
