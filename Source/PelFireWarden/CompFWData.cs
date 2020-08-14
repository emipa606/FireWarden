using System;
using Verse;

namespace PelFireWarden
{
	// Token: 0x0200000F RID: 15
	public class CompFWData : ThingComp
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00003323 File Offset: 0x00001523
		public CompProperties_FWData Props
		{
			get
			{
				return (CompProperties_FWData)this.props;
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003330 File Offset: 0x00001530
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (respawningAfterLoad && this.Props.FEFoamUses > 0)
			{
				Thing thing = this.parent;
				if (thing.GetType() != typeof(FireWardenData) && thing.def.thingClass == typeof(FireWardenData) && thing.Spawned && (thing?.Map) != null)
				{
					Thing newthing = ThingMaker.MakeThing(thing.def, null);
					IntVec3 pos = IntVec3.Zero;
					Map map = thing?.Map;
					if (map != null)
					{
						pos = thing.Position;
					}
					thing.Destroy(DestroyMode.Vanish);
					if (pos != IntVec3.Zero)
					{
						GenSpawn.Spawn(newthing, pos, map, WipeMode.Vanish);
					}
				}
			}
		}
	}
}
