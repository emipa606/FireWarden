using Verse;

namespace PelFireWarden
{
    // Token: 0x0200000F RID: 15
    public class CompFWData : ThingComp
    {
        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000036 RID: 54 RVA: 0x00003323 File Offset: 0x00001523
        public CompProperties_FWData Props => (CompProperties_FWData) props;

        // Token: 0x06000037 RID: 55 RVA: 0x00003330 File Offset: 0x00001530
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad || Props.FEFoamUses <= 0)
            {
                return;
            }

            Thing thing = parent;
            if (thing.GetType() == typeof(FireWardenData) || thing.def.thingClass != typeof(FireWardenData) ||
                !thing.Spawned || thing.Map == null)
            {
                return;
            }

            var newthing = ThingMaker.MakeThing(thing.def);
            var pos = IntVec3.Zero;
            var map = thing.Map;
            if (map != null)
            {
                pos = thing.Position;
            }

            thing.Destroy();
            if (pos != IntVec3.Zero)
            {
                GenSpawn.Spawn(newthing, pos, map);
            }
        }
    }
}