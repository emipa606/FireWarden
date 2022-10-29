using Verse;

namespace PelFireWarden;

public class CompFWData : ThingComp
{
    public CompProperties_FWData Props => (CompProperties_FWData)props;

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