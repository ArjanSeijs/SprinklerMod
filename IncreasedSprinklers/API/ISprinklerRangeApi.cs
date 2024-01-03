using System;
using StardewValley;

namespace IncreasedSprinklers.API
{
    public interface ISprinklerRangeApi
    {
        public int GetRangeIncrease();
        public void AddSprinklerType(Func<Item, bool> itemTestFunction);
    }

    public class SprinklerRangeApi : ISprinklerRangeApi
    {
        public int GetRangeIncrease()
        {
            return ModEntry.Instance.Config.RangeIncrease;
        }

        public void AddSprinklerType(Func<Item, bool> itemTestFunction)
        {
            ModEntry.Instance.sprinklers.Add(itemTestFunction);
        }
    }
}