﻿using HarmonyLib;
using Verse;

namespace PelFireWarden
{
    // Token: 0x02000009 RID: 9
    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    public class TryCastShotFW
    {
        // Token: 0x0600001C RID: 28 RVA: 0x00002B20 File Offset: 0x00000D20
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

            var uses = ((FireWardenData) ChkFE).FEFoamUses;
            uses--;
            if (uses <= 0)
            {
                ChkFE.Destroy();
                return;
            }

            (ChkFE as FireWardenData).FEFoamUses = uses;
        }
    }
}