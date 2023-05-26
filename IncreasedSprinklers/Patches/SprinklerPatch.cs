using System;
using StardewModdingAPI;
using StardewValley;

// ReSharper disable InconsistentNaming

namespace IncreasedSprinklers.Patches
{
    public class SprinklerPatch
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        
        // patches need to be static!]
        internal static void GetBaseRadiusForSprinkler_Postfix(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                if (ModEntry.IncreaseRadius(__instance) || __result >= 0)
                {
                    __result += ModEntry.Instance.Config.RangeIncrease;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GetBaseRadiusForSprinkler_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}