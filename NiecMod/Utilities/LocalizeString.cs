/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 15/09/2018
 * Time: 21:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NiecMod.Utilities
{
	// Token: 0x02000037 RID: 55
	internal static class LocalizationUtils
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x000070DC File Offset: 0x000060DC
		internal static string LocalizeString(string objectName, string actionName, params object[] parameters)
		{
			string text = "NiecMod/Sims3Mod/KillSim/" + objectName + ":" + actionName;
			string result;
			if (StringTable.HasLocalizedString(text))
			{
				result = Localization.LocalizeString(text, parameters);
			}
			else
			{
				result = objectName + " - " + actionName;
			}
			return result;
		}

		// Token: 0x04000063 RID: 99
		private const string ns = "NiecMod/Sims3Mod/KillSim";
	}

}	