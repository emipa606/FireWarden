using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PelFireWarden
{
    // Token: 0x02000008 RID: 8
    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    public class Pawn_DraftController_GetGizmos_PatchFW
    {
        // Token: 0x0600001A RID: 26 RVA: 0x00002AA8 File Offset: 0x00000CA8
        public static void Postfix(ref IEnumerable<Gizmo> __result, ref Pawn_DraftController __instance)
        {
            if (__result == null)
            {
                return;
            }

            var list = __result.ToList();
            var pawn = __instance.pawn;
            if (pawn != null && pawn.equipment.Primary != null)
            {
                var ChkFE = pawn.equipment.Primary;
                if (ChkFE.def.HasComp(typeof(CompFWData)))
                {
                    Gizmo item = new Gizmo_FEFoamStatus
                    {
                        FE = ChkFE
                    };
                    list.Add(item);
                }
            }

            __result = list;
        }
    }
}