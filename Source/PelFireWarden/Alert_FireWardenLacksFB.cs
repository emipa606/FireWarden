using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PelFireWarden
{
	// Token: 0x0200000D RID: 13
	public class Alert_FireWardenLacksFB : Alert
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002FD2 File Offset: 0x000011D2
		private IEnumerable<Pawn> FWNoFB
		{
			get
			{
				foreach (Pawn p in PawnsFinder.AllMaps_FreeColonistsSpawned)
				{
					if (Controller.Settings.ShowEqWarnMsgs && ((Alert_FireWardenLacksFB.FWResearch.FireBeater.IsFinished && !Alert_FireWardenLacksFB.FWResearch.FireExt.IsFinished) || (Alert_FireWardenLacksFB.FWResearch.FireExt.IsFinished && Controller.Settings.BrawlerNotOK && p.story.traits.HasTrait(TraitDefOf.Brawler))) && !p.story.traits.HasTrait(TraitDefOf.Pyromaniac) && p.workSettings.WorkIsActive(Alert_FireWardenLacksFB.FWWorkTypeDef.PelFireWarden) && !this.HasFB(p) && !p.Downed && !Controller.Settings.EquippingDone)
					{
						yield return p;
					}
				}
				List<Pawn>.Enumerator enumerator = default(List<Pawn>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002FE4 File Offset: 0x000011E4
		public bool HasFB(Pawn FW)
		{
			if (FW.equipment.Primary == null)
			{
				bool HasFBcheck = false;
				using (List<Thing>.Enumerator enumerator = FW.inventory.innerContainer.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.def.defName == Alert_FireWardenLacksFB.FBDefName)
						{
							HasFBcheck = true;
							break;
						}
					}
				}
				return HasFBcheck;
			}
			if (FW.equipment.Primary.def.defName != Alert_FireWardenLacksFB.FBDefName)
			{
				bool HasFBcheck2 = false;
				using (List<Thing>.Enumerator enumerator = FW.inventory.innerContainer.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.def.defName == Alert_FireWardenLacksFB.FBDefName)
						{
							HasFBcheck2 = true;
							break;
						}
					}
				}
				return HasFBcheck2;
			}
			return true;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000030E0 File Offset: 0x000012E0
		public Alert_FireWardenLacksFB()
		{
			this.defaultLabel = "PelFWLacksFB".Translate();
			this.defaultExplanation = "PelFWLacksFBDesc".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003119 File Offset: 0x00001319
		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(this.FWNoFB.ToList<Pawn>());
		}

		// Token: 0x0400000C RID: 12
		public static string FBDefName = "Firebeater";

		// Token: 0x02000028 RID: 40
		[DefOf]
		public static class FWWorkTypeDef
		{
			// Token: 0x04000055 RID: 85
			public static WorkTypeDef PelFireWarden;
		}

		// Token: 0x02000029 RID: 41
		[DefOf]
		public static class FWResearch
		{
			// Token: 0x04000056 RID: 86
			public static ResearchProjectDef FireBeater;

			// Token: 0x04000057 RID: 87
			public static ResearchProjectDef FireExt;
		}
	}
}
