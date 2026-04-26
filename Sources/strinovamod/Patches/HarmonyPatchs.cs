using HarmonyLib;
using RimWorld;
using Verse;

namespace Strinova
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Superstring.Mod");
            harmony.PatchAll();
        }
    }

    // 超弦体击中时获得能量
    [HarmonyPatch(typeof(DamageWorker), "Apply")]
    public static class DamageWorker_Apply_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(DamageInfo dinfo, Thing victim)
        {
            if (victim is Pawn pawn && !pawn.Dead && dinfo.Amount > 0)
            {
                TryGetEnergy(pawn);
            }
        }

        public static void TryGetEnergy(Pawn pawn)
        {
            if (pawn.Faction == Faction.OfPlayer)
            {
                var energy = pawn.GetComp<Comp_Superstring_Energy>();
                energy?.AddEnergy(15f);
            }
        }
    }
}
