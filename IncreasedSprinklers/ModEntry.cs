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
            HarmonyPatch();
        }

        /// <summary>
        /// Applies the harmony patch for the base game and range highlight if it is installed
        /// </summary>
        private void HarmonyPatch()
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            SprinklerPatch.Initialize(Monitor);
            RangeHighlightPatch.Initialize(Monitor, this);

            ApplyBaseGamePatch(harmony);
            ApplyHighlightPatch(harmony);
        }

        /// <summary>
        /// Base game harmony patch
        /// </summary>
        /// <param name="harmony"></param>
        private void ApplyBaseGamePatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.GetModifiedRadiusForSprinkler)),
                postfix: new HarmonyMethod(typeof(SprinklerPatch), nameof(SprinklerPatch.GetModifiedRadiusForSprinkler_Postfix))
            );
        }

        /// <summary>
        /// If installed patch RangeHighlight getSprinkler method
        /// </summary>
        /// <param name="harmony"></param>
        private void ApplyHighlightPatch(Harmony harmony)
        {
            if (!Helper.ModRegistry.IsLoaded("jltaylor-us.RangeHighlight")) return;

            // RangeHighlight.TheMod.DefaultShapes
            var type = AccessTools.TypeByName("DefaultShapes");
            harmony.Patch(
                original: AccessTools.Method(type, "GetSprinkler"),
                prefix: new HarmonyMethod(typeof(RangeHighlightPatch), nameof(RangeHighlightPatch.GetSprinkler_Prefix))
            );
        }

        /// <summary>
        /// Registering the config menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                setValue: value => { Config.RangeIncrease = Math.Max(1, value); });
        }

        /// <summary>
        /// Returns if a item should have its modified radius increased. (Currently all vanilla sprinklers)
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IncreaseRadius(Item instance)
        {
            return Utility.IsNormalObjectAtParentSheetIndex(instance, 599) ||
                   Utility.IsNormalObjectAtParentSheetIndex(instance, 621) ||
                   Utility.IsNormalObjectAtParentSheetIndex(instance, 645);
        }
    }
}