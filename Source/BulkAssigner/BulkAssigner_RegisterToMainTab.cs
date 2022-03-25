using System.Collections.Generic;
using ModButtons;
using Verse;

namespace BulkAssigner;

internal class BulkAssigner_RegisterToMainTab
{
    public static bool wasRegistered;

    public static void ensureMainTabRegistered()
    {
        if (wasRegistered)
        {
            return;
        }

        Log.Message("Hello from BulkAssigner_RegisterToMainTab ensureMainTabRegistered");

        var columns = MainTabWindow_ModButtons.columns;

        var buttons = new List<ModButton_Text>
        {
            new ModButton_Text(
                () => "Set Outfit",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getOutfitMenu()); }
            ),
            new ModButton_Text(
                () => "Set Allowed Area",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getAllowedAreaMenu()); }
            ),
            new ModButton_Text(
                () => "Set Drug Policy",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getDrugPolicyMenu()); }
            ),
            new ModButton_Text(
                () => "Set Food Restriction",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getFoodRestrictionMenu()); }
            ),
            new ModButton_Text(
                () => "Set Medical Care",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getMedicalCareMenu()); }
            ),
            new ModButton_Text(
                () => "Set Enemy Response",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getHostilityResponseModeMenu()); }
            ),
            new ModButton_Text(
                () => "Drop Everything",
                BulkAssigner_Functions.dropEverythingFromInventory
            ),
            new ModButton_Text(
                () => "Bulk Consume from Inventory",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getBulkConsumeFromInventoryFloatMenu()); }
            ),
            new ModButton_Text(
                () => "Bulk Operate Humanoids",
                delegate { Find.WindowStack.Add(BulkAssigner_Functions.getBulkOperationFloatMenu()); }
            )
        };

        columns.Add(buttons);

        wasRegistered = true;
    }
}