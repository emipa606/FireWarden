using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace PelFireWarden
{
	// Token: 0x02000005 RID: 5
	[StaticConstructorOnStartup]
	internal static class HarmonyPatching
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002823 File Offset: 0x00000A23
		static HarmonyPatching()
		{
			new Harmony("com.Pelador.Rimworld.FireWarden").PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
