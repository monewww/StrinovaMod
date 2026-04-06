using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Strinova
{
    public class CompProperties_Celestia_NormalSkill: CompProperties_AbilityEffect
    {
        public FleckDef skillfleck;
        public CompProperties_Celestia_NormalSkill()
        {
            compClass = typeof(CompAbilities_Celestia_NormalSkill);
        }
    }

}
