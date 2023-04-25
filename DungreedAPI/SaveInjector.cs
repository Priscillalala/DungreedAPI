using System;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using System.Collections.Generic;

namespace DungreedAPI
{
    internal static class SaveInjector
	{
        internal static event Action<Dictionary<string, SaveData.DataContainer>> beforeSave;

        internal static void Init()
        {
            IL.SaveData.Save += SaveData_Save;
        }

        private static void SaveData_Save(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool found = c.TryGotoNext(x => x.MatchStfld<SaveData>(nameof(SaveData.serializedData)));
            if (found)
            {
                c.Emit<SaveData>(OpCodes.Ldfld, nameof(SaveData.serializedData));
                c.EmitDelegate<Func<SaveData.DataContainer[], SaveData.DataContainer[]>>(orig => 
                {
                    beforeSave?.Invoke(orig.ToDictionary(x => x.key));
                    return orig;
                });
                c.Emit<SaveData>(OpCodes.Stfld, nameof(SaveData.serializedData));
            } 
            else
            {
                DungreedAPI.logger.LogWarning("Save Injector IL hook failed!");
            }
        }
    }
}
