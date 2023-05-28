using System;
using RangeHighlight;
using StardewModdingAPI;

// using Object = StardewValley.Object;

namespace IncreasedSprinklers.Patches
{
    public static class RangeHighlightPatch
    {

        public static IMonitor Monitor { get; set; }
        public static ModEntry ModEntry { get; set; }
        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor, ModEntry modEntry)
        {
            Monitor = monitor;
            ModEntry = modEntry;
        }

        internal static bool GetSprinkler_Prefix(string name, bool hasPressureNozzleAttached, ref bool[,] __result)
        {
            try
            {
                var api = ModEntry.Helper.ModRegistry.GetApi<IRangeHighlightAPI>("jltaylor-us.RangeHighlight");
                if (api is null) return true;
                
                // The values with increased range
                var sprinkler = api.GetSquareCircle((uint)(0 + ModEntry.Config.RangeIncrease));
                var qualitySprinkler = api.GetSquareCircle((uint)(1 + ModEntry.Config.RangeIncrease));
                var iridiumSprinkler = api.GetSquareCircle((uint)(2 + ModEntry.Config.RangeIncrease));
                var iridiumSprinklerWithNozzle = api.GetSquareCircle((uint)(3 + ModEntry.Config.RangeIncrease));
                var prismaticSprinkler = api.GetSquareCircle((uint)(3 + ModEntry.Config.RangeIncrease));
                var radioactiveSprinkler = api.GetSquareCircle((uint)(3 + ModEntry.Config.RangeIncrease));

                // The original method
                bool[,] GetSprinkler()
                {
                    if (name.Contains("iridium"))
                        return hasPressureNozzleAttached ? iridiumSprinklerWithNozzle : iridiumSprinkler;
                    if (name.Contains("quality"))
                        return hasPressureNozzleAttached ? iridiumSprinkler : qualitySprinkler;
                    if (name.Contains("prismatic")) return prismaticSprinkler;
                    if (name.Contains("radioactive")) return radioactiveSprinkler;
                    return hasPressureNozzleAttached ? qualitySprinkler : sprinkler;
                }

                __result = GetSprinkler();
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GetSprinkler_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}