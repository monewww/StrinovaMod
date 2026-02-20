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
}
