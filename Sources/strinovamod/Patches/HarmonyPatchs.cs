using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
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
                if (energy != null)
                {
                    energy.energy += 15f;
                }
            }
        }
    }

    //替换大招按钮
    [HarmonyPatch(typeof(Ability), nameof(Ability.GetGizmos))]
    public static class Ability_GetGizmos_Patch
    {
        static IEnumerable<Command> Postfix(IEnumerable<Command> __result, Ability __instance)
        {
            // 只处理你的技能
            if (!__instance.comps.Any(c => c is CompAbilities_Common_SuperSkill))
                return __result;

            var pawn = __instance.pawn;
            if (pawn == null) return __result;

            var comp = pawn.GetComp<Comp_Superstring_Energy>();
            if (comp == null) return __result;

            // 原gizmo全部丢掉，换成你的
            return new List<Command>
        {
            new Command_Action
            {
                defaultLabel = "Super Skill",
                action = delegate
                {
                    __instance.QueueCastingJob(pawn);
                }
            }
        };
        }
    }
}
