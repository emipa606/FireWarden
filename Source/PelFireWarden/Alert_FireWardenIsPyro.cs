using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden
{
	// Token: 0x0200000C RID: 12
	public class Alert_FireWardenIsPyro : Alert
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002F7E File Offset: 0x0000117E
		private IEnumerable<Pawn> FWIsPyro
		{
			get
			{
				foreach (Pawn p in PawnsFinder.AllMaps_FreeColonistsSpawned)
				{
					if (p.workSettings.WorkIsActive(Alert_FireWardenIsPyro.FWWorkTypeDef.PelFireWarden) && p.story.traits.HasTrait(TraitDefOf.Pyromaniac))
					{
						yield return p;
					}
				}
				List<Pawn>.Enumerator enumerator = default(List<Pawn>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002F87 File Offset: 0x00001187
		public Alert_FireWardenIsPyro()
		{
			this.defaultLabel = "PelFWIsPyro".Translate();
			this.defaultExplanation = "PelFWIsPyroDesc".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002FC0 File Offset: 0x000011C0
		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(this.FWIsPyro.ToList<Pawn>());
		}

		// Token: 0x02000026 RID: 38
		[DefOf]
		public static class FWWorkTypeDef
		{
			// Token: 0x04000050 RID: 80
			public static WorkTypeDef PelFireWarden;
		}
	}
}
