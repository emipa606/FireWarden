using UnityEngine;
using Verse;

namespace PelFireWarden
{
    // Token: 0x0200000B RID: 11
    public class Controller : Mod
    {
        // Token: 0x0400000B RID: 11
        public static Settings Settings;

        // Token: 0x06000028 RID: 40 RVA: 0x00002F6A File Offset: 0x0000116A
        public Controller(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002F4C File Offset: 0x0000114C
        public override string SettingsCategory()
        {
            return "FWrd.Name".Translate();
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002F5D File Offset: 0x0000115D
        public override void DoSettingsWindowContents(Rect canvas)
        {
            Settings.DoWindowContents(canvas);
        }
    }
}