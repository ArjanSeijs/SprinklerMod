using System;
using HarmonyLib;
using IncreasedSprinklers.API;
using IncreasedSprinklers.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace IncreasedSprinklers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public ModConfig Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            Instance = this;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            SprinklerPatch.Initialize(Monitor);

            ApplyHarmonyPatch();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("jltaylor-us.RangeHighlight"))
            {
                RangeHighlightPatch.Apply(this);
            }
        }

        private void ApplyHarmonyPatch()
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object),
                    nameof(StardewValley.Object.GetModifiedRadiusForSprinkler)),
                postfix: new HarmonyMethod(typeof(SprinklerPatch),
                    nameof(SprinklerPatch.GetBaseRadiusForSprinkler_Postfix))
            );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

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

        public static bool IncreaseRadius(Item instance)
        {
            return Utility.IsNormalObjectAtParentSheetIndex(instance, 599) ||
                   Utility.IsNormalObjectAtParentSheetIndex(instance, 621) ||
                   Utility.IsNormalObjectAtParentSheetIndex(instance, 645);
        }
    }
}