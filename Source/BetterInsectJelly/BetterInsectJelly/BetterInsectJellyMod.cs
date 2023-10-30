using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using Verse.Sound;

namespace BetterInsectJelly
{
    public class BetterInsectJellyMod : Mod
    {
        public Settings settings;

        public BetterInsectJellyMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.Label("Values for Vanilla Insect Jelly");
            listingStandard.Label("Insect Jelly Nutrition");
            settings.InsectJellyNutrition = listingStandard.Slider(settings.InsectJellyNutrition, 0.05f, 1.00f);
            listingStandard.Label("Insect Jelly Market Value");
            settings.InsectJellyValue = listingStandard.Slider(settings.InsectJellyValue, 0f, 32f);
            listingStandard.Label("Insect Jelly Food Poison Chance");
            settings.InsectJellyFoodPoisonChance = listingStandard.Slider(settings.InsectJellyFoodPoisonChance, 0f, 1.00f);
            listingStandard.Label("Insect Jelly Joy");
            settings.InsectJellyJoy = listingStandard.Slider(settings.InsectJellyJoy, 0f, 1.00f);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "BetterInsectJelly".Translate();
        }
    }

    public class Settings : ModSettings
    {
        public float InsectJellyNutrition = 0.01f;
        public float InsectJellyValue = 16.00f;
        public float InsectJellyFoodPoisonChance = 0.02f;
        public float InsectJellyJoy = 0.16f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref InsectJellyNutrition, "InsectJellyNutrition", 0.01f);
            Scribe_Values.Look(ref InsectJellyValue, "InsectJellyValue", 16.00f);
            Scribe_Values.Look(ref InsectJellyFoodPoisonChance, "InsectJellyFoodPoisonChance", 0.02f);
            Scribe_Values.Look(ref InsectJellyJoy, "InsectJellyJoy", 0.16f);
            base.ExposeData();
        }

    }
}
