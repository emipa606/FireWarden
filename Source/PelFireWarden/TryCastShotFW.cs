using HarmonyLib;
using Verse;

namespace PelFireWarden;

[HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
public class TryCastShotFW
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, ref Verb_Shoot __instance)
    {
        if (!__result)
        {
            return;
        }

        var ChkFE = __instance?.EquipmentSource;
        if (ChkFE == null || !ChkFE.def.HasComp(typeof(CompFWData)))
        {
            return;
        }

        var uses = ((FireWardenData)ChkFE).FEFoamUses;
        uses--;
        if (uses <= 0)
        {
            ChkFE.Destroy();
            return;
        }

        (ChkFE as FireWardenData).FEFoamUses = uses;
    }
}