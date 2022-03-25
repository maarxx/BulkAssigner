using HarmonyLib;
using ModButtons;
using UnityEngine;

namespace BulkAssigner;

[HarmonyPatch(typeof(MainTabWindow_ModButtons))]
[HarmonyPatch("DoWindowContents")]
internal class Patch_MainTabWindow_ModButtons_DoWindowContents
{
    private static void Prefix(MainTabWindow_ModButtons __instance, ref Rect canvas)
    {
        BulkAssigner_RegisterToMainTab.ensureMainTabRegistered();
    }
}