using System.Reflection;
using HarmonyLib;
using Verse;

namespace PelFireWarden;

[StaticConstructorOnStartup]
internal static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("com.Pelador.Rimworld.FireWarden").PatchAll(Assembly.GetExecutingAssembly());
    }
}