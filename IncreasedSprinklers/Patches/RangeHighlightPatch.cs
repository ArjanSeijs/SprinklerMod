using System;
using Microsoft.Xna.Framework;
using RangeHighlight;
using StardewModdingAPI.Utilities;
using StardewValley;
using Object = StardewValley.Object;

namespace IncreasedSprinklers.Patches
{
    public static class RangeHighlightPatch
    {
        public static void Apply(ModEntry modEntry)
        {
            var api = modEntry.Helper.ModRegistry.GetApi<IRangeHighlightAPI>("jltaylor-us.RangeHighlight");
            if (api is null) return;

            Tuple<Color, bool[,]> Highlighter(Item item, int id, string name)
            {
                modEntry.Monitor.Log($"Name is: {name}");

                if (item is not Object obj) return null;

                modEntry.Monitor.Log($"Increases radius {name}: {ModEntry.IncreaseRadius(item)}");

                if (!ModEntry.IncreaseRadius(item)) return null;

                var sprinklerRangeTint = api.GetSprinklerRangeTint();
                var modifiedRadiusForSprinkler = obj.GetModifiedRadiusForSprinkler();
                if (modifiedRadiusForSprinkler < 0) return null;
                
                var squareCircle = api.GetSquareCircle((uint)modifiedRadiusForSprinkler);
                return new Tuple<Color, bool[,]>(sprinklerRangeTint, squareCircle);
            }

            api.AddItemRangeHighlighter("eternalsoap.sprinkler",
                () => true,
                () => new KeybindList(),
                () => true,
                Highlighter);
        }
    }
}