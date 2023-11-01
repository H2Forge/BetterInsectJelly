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

        private static void AddSettingsNumberLine(Listing_Standard listing, string name, float vanillaVal, float modVal, ref float val, float min, float max)
        {
            Rect rect = listing.GetRect(30f);
            Rect rect2 = rect.LeftPart(0.5f).Rounded();
            Rect rect3 = rect.RightPart(0.5f).Rounded();
            Rect rect4 = rect3.LeftPart(0.25f).Rounded();
            Rect rect5 = rect3.RightPart(0.7f).Rounded();
            Text.Anchor = TextAnchor.MiddleLeft;
            TooltipHandler.TipRegion(rect, ("Vanilla: " + string.Format("{0:N3}", vanillaVal) + ", Default: " + string.Format("{0:N3}", modVal)));
            Widgets.Label(rect2, (name));
            string buffer = $"{val:F2}";
            Widgets.TextFieldNumeric(rect4, ref val, ref buffer, min, max);
            Text.Anchor = TextAnchor.UpperLeft;
            float num = Widgets.HorizontalSlider(rect5, val, min, max, middleAlignment: true);
            if (num != val)
            {
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                val = num;
            }
            listing.Gap(2f);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(inRect);

            ls.Label("Vanilla (Insect Jelly)");

            ls.GapLine();
            AddSettingsNumberLine(ls, "Insect Jelly Nutrition", 0.05f, 0.01f, ref settings.InsectJellyNutrition, 0f, 1f);
            AddSettingsNumberLine(ls, "Insect Jelly Market Value", 8f, 16f, ref settings.InsectJellyValue, 0f, 100f);
            AddSettingsNumberLine(ls, "Insect Jelly Food Poison Chance", 0.02f, 0f, ref settings.InsectJellyFoodPoisonChance, 0f, 1f);
            AddSettingsNumberLine(ls, "Insect Jelly Joy", 0.08f, 0.16f, ref settings.InsectJellyJoy, 0f, 1f);
            //AddSettingsNumberLine(ls, "Insect Jelly Rest Offset per unit", 0f, 0.025f, ref settings.InsectJellyRestOffset, 0f, 1f);
            ls.GapLine();
            ls.GapLine();

            ls.Gap(2f);
            ls.Label("Vanilla Cooking Expanded (Insect Jelly Preserve)");
            ls.GapLine();
            ls.CheckboxLabeled("Apply to Insect Jelly Preserve", ref settings.applyToInsectJellyPreserve);
            ls.GapLine();
            ls.GapLine();

            ls.Gap(2f);
            ls.Label("Vanilla Factions Expanded - Insectoids (Royal Insect Jelly)");
            ls.GapLine();
            ls.CheckboxLabeled("Apply to Royal Insect Jelly", ref settings.applyToRoyalInsectJelly);
            AddSettingsNumberLine(ls, "Multiplier for Royal Insect Jelly stats", 0f, 0.05f, ref settings.factorRoyalInsectJelly, 0f, 5f);
            ls.GapLine();
            ls.GapLine();

            ls.End();

            //Vanilla settings
            Rect applyButton = inRect.BottomPart(0.1f).LeftPart(0.1f);
            bool apply = Widgets.ButtonText(applyButton, "Vanilla Settings", true, true, true);
            if (apply)
            {
                VanillaSettings();
            }
            //Defaullt settings
            Rect resetButton = inRect.BottomPart(0.1f).RightPart(0.1f);
            bool reset = Widgets.ButtonText(resetButton, "Default Settings", true, true, true);
            if (reset)
            {
                ResetSettings();
            }
            base.DoSettingsWindowContents(inRect);
        }

        private void VanillaSettings()
        {
            settings.InsectJellyNutrition = 0.05f;
            settings.InsectJellyValue = 0.8f;
            settings.InsectJellyJoy = 0.08f;
            settings.InsectJellyFoodPoisonChance = 0.02f;
            settings.factorRoyalInsectJelly = 1f;

        }

        private void ResetSettings()
        {
            settings.InsectJellyNutrition = 0.1f;
            settings.InsectJellyValue = 0.16f;
            settings.InsectJellyJoy = 0.16f;
            settings.InsectJellyFoodPoisonChance = 0f;
            settings.factorRoyalInsectJelly = 1f;
        }
        public override string SettingsCategory()
        {
            return "BetterInsectJelly";
        }

        public override void WriteSettings()
        {


            ThingDef InsectJelly = DefDatabase<ThingDef>.GetNamed("InsectJelly");
            if (InsectJelly != null)
            {
                InsectJelly.statBases.Find(x => x.stat == StatDefOf.Nutrition).value = settings.InsectJellyNutrition;
                InsectJelly.statBases.Find(x => x.stat == StatDefOf.MarketValue).value = settings.InsectJellyValue;
                InsectJelly.statBases.Find(x => x.stat == StatDefOf.FoodPoisonChanceFixedHuman).value = settings.InsectJellyFoodPoisonChance;
                InsectJelly.ingestible.joy = settings.InsectJellyJoy;
            }

            /*

            ThingDef InsectJellyPreserve = DefDatabase<ThingDef>.GetNamed("VCE_InsectJellyPreserves");
            if (InsectJellyPreserve != null && settings.applyToInsectJellyPreserve)
            {
                InsectJellyPreserve.statBases.Find(x => x.stat == StatDefOf.Nutrition).value = settings.InsectJellyNutrition;
                InsectJellyPreserve.statBases.Find(x => x.stat == StatDefOf.MarketValue).value = settings.InsectJellyValue;
                InsectJellyPreserve.statBases.Find(x => x.stat == StatDefOf.FoodPoisonChanceFixedHuman).value = settings.InsectJellyFoodPoisonChance;
                InsectJellyPreserve.ingestible.joy = settings.InsectJellyJoy;
            }

            ThingDef RoyalInsectJelly = DefDatabase<ThingDef>.GetNamed("VFEI_RoyalInsectJelly");
            if (RoyalInsectJelly != null && settings.applyToRoyalInsectJelly)
            {
                RoyalInsectJelly.statBases.Find(x => x.stat == StatDefOf.Nutrition).value = settings.InsectJellyNutrition;
                RoyalInsectJelly.statBases.Find(x => x.stat == StatDefOf.MarketValue).value = settings.factorRoyalInsectJelly * settings.InsectJellyValue;
                RoyalInsectJelly.ingestible.joy = settings.factorRoyalInsectJelly * settings.InsectJellyJoy;
            }
            */

            base.WriteSettings();
        }
    }



    public class Settings : ModSettings
    {
        public float InsectJellyNutrition = 0.01f;
        public float InsectJellyValue = 16.00f;
        public float InsectJellyFoodPoisonChance = 0.02f;
        public float InsectJellyJoy = 0.16f;
        //public float InsectJellyRestOffset = 0.025f;

        public bool applyToInsectJellyPreserve = true;
        public bool applyToRoyalInsectJelly = true;
        public float factorRoyalInsectJelly = 2.00f;


        public override void ExposeData()
        {
            Scribe_Values.Look(ref InsectJellyNutrition, "InsectJellyNutrition", 0.01f);
            Scribe_Values.Look(ref InsectJellyValue, "InsectJellyValue", 16.00f);
            Scribe_Values.Look(ref InsectJellyFoodPoisonChance, "InsectJellyFoodPoisonChance", 0.02f);
            Scribe_Values.Look(ref InsectJellyJoy, "InsectJellyJoy", 0.16f);
            //Scribe_Values.Look(ref InsectJellyRestOffset, "InsectJellyRestOffset", 0.025f);
            Scribe_Values.Look(ref applyToInsectJellyPreserve, "applyToInsectJellyPreserve", true);
            Scribe_Values.Look(ref applyToRoyalInsectJelly, "applyToRoyalInsectJelly", true);
            Scribe_Values.Look(ref factorRoyalInsectJelly, "factorRoyalInsectJelly", 2.00f);
            base.ExposeData();
        }

    }
}
