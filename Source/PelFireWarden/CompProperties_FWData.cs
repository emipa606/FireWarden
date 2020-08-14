using System;
using Verse;

namespace PelFireWarden
{
	// Token: 0x02000010 RID: 16
	public class CompProperties_FWData : CompProperties
	{
		// Token: 0x06000039 RID: 57 RVA: 0x000033FB File Offset: 0x000015FB
		public CompProperties_FWData()
		{
			this.compClass = typeof(CompFWData);
		}

		// Token: 0x0400000E RID: 14
		public string FWSwapType = "N";

		// Token: 0x0400000F RID: 15
		public int FWPawnID;

		// Token: 0x04000010 RID: 16
		public string FWPrimDef = "N";

		// Token: 0x04000011 RID: 17
		public int FEFoamUses = 100;
	}
}
