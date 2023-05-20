using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace IncreasedSprinklers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public ModConfig Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            Config = Helper.ReadConfig<ModConfig>();
            Instance = this;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            // example patch, you'll need to edit this for your patch
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object),
                    nameof(StardewValley.Object.GetBaseRadiusForSprinkler)),
                postfix: new HarmonyMethod(typeof(SprinklerPatch),
                    nameof(SprinklerPatch.GetBaseRadiusForSprinkler_Postfix))
            );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu =
                this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Range",
                tooltip: () => "The Sprinkler Range Increase",
                getValue: () => Config.RangeIncrease,
                setValue: value => Config.RangeIncrease = Math.Max(1, value)
            );
        }
    }
}