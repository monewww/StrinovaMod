using HarmonyLib;
using LudeonTK;
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

    // 超弦体攻击命中时获得能量
    [HarmonyPatch(typeof(DamageWorker), "Apply")]
    public static class DamageWorker_Apply_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(DamageInfo dinfo, Thing victim)
        {
            if (dinfo.Amount <= 0) return;
            if (!(dinfo.Instigator is Pawn attacker) || attacker.Dead) return;
            if (attacker == victim) return;
            if (attacker.Faction != Faction.OfPlayer) return;

            var energy = attacker.GetComp<Comp_Superstring_Energy>();
            energy?.AddEnergy(15f);
        }
    }

    public static class Strinova_DebugActions
    {
        [DebugAction("Strinova", "Fill energy (pawn)", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void FillEnergyForPawn(Pawn pawn)
        {
            var comp = pawn.GetComp<Comp_Superstring_Energy>();
            if (comp == null) return;
            comp.energy = comp.maxEnergy;
            MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Energy filled", 2f);
        }

        [DebugAction("Strinova", "Fill energy (all)", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void FillEnergyAll()
        {
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists)
            {
                var comp = pawn.GetComp<Comp_Superstring_Energy>();
                if (comp != null)
                    comp.energy = comp.maxEnergy;
            }
        }
    }
}
