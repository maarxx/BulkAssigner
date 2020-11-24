using ModButtons;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BulkAssigner
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.github.harmony.rimworld.maarx.bulkassigner");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(MainTabWindow_ModButtons))]
    [HarmonyPatch("DoWindowContents")]
    class Patch_MainTabWindow_ModButtons_DoWindowContents
    {
        static void Prefix(MainTabWindow_ModButtons __instance, ref Rect canvas)
        {
            BulkAssigner_RegisterToMainTab.ensureMainTabRegistered();
        }
    }

    class BulkAssigner_RegisterToMainTab
    {
        public static bool wasRegistered = false;

        public static void ensureMainTabRegistered()
        {
            if (wasRegistered) { return; }

            Log.Message("Hello from BulkAssigner_RegisterToMainTab ensureMainTabRegistered");

            List<List<ModButton_Text>> columns = MainTabWindow_ModButtons.columns;

            List<FloatMenuOption> hostilityResponseOptions = new List<FloatMenuOption>();
            foreach (HostilityResponseMode hrm in Enum.GetValues(typeof(HostilityResponseMode)))
            {
                hostilityResponseOptions.Add(new FloatMenuOption(hrm.ToString(), delegate { BulkAssigner_Functions.setHostilityResponseMode(hrm); }));
            }

            List<FloatMenuOption> outfitOptions = new List<FloatMenuOption>();
            foreach (Outfit outfit in Current.Game.outfitDatabase.AllOutfits)
            {
                outfitOptions.Add(new FloatMenuOption(outfit.label, delegate { BulkAssigner_Functions.setOutfit(outfit); }));
            }

            List<FloatMenuOption> drugPolicyOptions = new List<FloatMenuOption>();
            foreach (DrugPolicy dp in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                drugPolicyOptions.Add(new FloatMenuOption(dp.label, delegate { BulkAssigner_Functions.setDrugPolicy(dp); }));
            }

            List<FloatMenuOption> foodRestrictionOptions = new List<FloatMenuOption>();
            foreach (FoodRestriction fr in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
            {
                foodRestrictionOptions.Add(new FloatMenuOption(fr.label, delegate { BulkAssigner_Functions.setFoodRestriction(fr); }));
            }

            List<FloatMenuOption> allowedAreas = new List<FloatMenuOption>();
            allowedAreas.Add(new FloatMenuOption("Unrestricted", delegate { BulkAssigner_Functions.setAllowedArea(null); }));
            foreach (Area a in Find.CurrentMap.areaManager.AllAreas)
            {
                if (a.AssignableAsAllowed())
                {
                    allowedAreas.Add(new FloatMenuOption(a.Label, delegate { BulkAssigner_Functions.setAllowedArea(a); }));
                }
            }
            List<ModButtons.ModButton_Text> buttons = new List<ModButtons.ModButton_Text>();
            buttons.Add(new ModButton_Text("Set Enemy Response", delegate {
                Find.WindowStack.Add(new FloatMenu(hostilityResponseOptions));
            }));
            buttons.Add(new ModButton_Text("Set Outfit", delegate {
                Find.WindowStack.Add(new FloatMenu(outfitOptions));
            }));
            buttons.Add(new ModButton_Text("Set Drug Policy", delegate {
                Find.WindowStack.Add(new FloatMenu(drugPolicyOptions));
            }));
            buttons.Add(new ModButton_Text("Set Food Restriction", delegate {
                Find.WindowStack.Add(new FloatMenu(foodRestrictionOptions));
            }));
            buttons.Add(new ModButton_Text("Set Allowed Area", delegate {
                Find.WindowStack.Add(new FloatMenu(allowedAreas));
            }));
            buttons.Add(new ModButton_Text("Drop Everything", delegate {
                BulkAssigner_Functions.dropEverythingFromInventory();
            }));

            columns.Add(buttons);

            wasRegistered = true;
        }
    }
}
