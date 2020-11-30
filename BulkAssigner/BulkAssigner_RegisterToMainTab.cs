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

            List<ModButton_Text> buttons = new List<ModButton_Text>();
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Set Outfit";
                },
                delegate {
                    Find.WindowStack.Add(BulkAssigner_Functions.getOutfitMenu());
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Set Allowed Area";
                },
                delegate {
                    Find.WindowStack.Add(BulkAssigner_Functions.getAllowedAreaMenu());
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Set Drug Policy";
                },
                delegate {
                    Find.WindowStack.Add(BulkAssigner_Functions.getDrugPolicyMenu());
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Set Food Restriction";
                },
                delegate {
                    Find.WindowStack.Add(BulkAssigner_Functions.getFoodRestrictionMenu());
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Set Medical Care";
                },
                delegate {
                    Find.WindowStack.Add(BulkAssigner_Functions.getMedicalCareMenu());
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Set Enemy Response";
                },
                delegate {
                    Find.WindowStack.Add(BulkAssigner_Functions.getHostilityResponseModeMenu());
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Drop Everything";
                },
                delegate {
                    BulkAssigner_Functions.dropEverythingFromInventory();
                }
            ));

            columns.Add(buttons);

            wasRegistered = true;
        }
    }
}
