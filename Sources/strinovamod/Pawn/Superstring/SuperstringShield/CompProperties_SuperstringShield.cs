using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Strinova
{
    public class CompProperties_SuperstringShield : CompProperties
    {
        public string shieldGraphicPath;

        public CompProperties_SuperstringShield() {
            this.compClass = typeof(Comp_SuperstringShield);
        }

    }
}
