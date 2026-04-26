using RimWorld;
using Verse;


namespace Strinova
{
    public class Comp_Superstring_Energy : ThingComp
    {
        public float energy = 0f;
        public float maxEnergy = 1000f;

        private int lastTick = 0;

        public Pawn Pawn => parent as Pawn;

        public override void CompTick()
        {
            base.CompTick();

            int curTick = Find.TickManager.TicksGame;

            // 每秒 +10（60 tick）
            if (curTick - lastTick >= 60)
            {
                AddEnergy(10f);
                lastTick = curTick;
            }
        }

        public void AddEnergy(float amount)
        {
            energy += amount;
            if (energy > maxEnergy)
                energy = maxEnergy;
        }

        public bool Consume(float amount)
        {
            if (energy < amount) return false;

            energy -= amount;
            return true;
        }


    }

    public class CompProperties_Superstring_Energy : CompProperties
    {
        public CompProperties_Superstring_Energy()
        {
            this.compClass = typeof(Comp_Superstring_Energy);
        }
    }

}
