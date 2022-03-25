using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BulkAssigner;

public static class BulkAssigner_Functions
{
    public static void setHostilityResponseMode(HostilityResponseMode hrm)
    {
        foreach (var o in Find.Selector.SelectedObjects)
        {
            if (o is not Pawn pawn)
            {
                continue;
            }

            pawn.playerSettings.hostilityResponse = hrm;
        }
    }

    public static FloatMenu getHostilityResponseModeMenu()
    {
        var hostilityResponseOptions = new List<FloatMenuOption>();
        foreach (HostilityResponseMode hrm in Enum.GetValues(typeof(HostilityResponseMode)))
        {
            hostilityResponseOptions.Add(new FloatMenuOption(hrm.ToString(),
                delegate { setHostilityResponseMode(hrm); }));
        }

        return new FloatMenu(hostilityResponseOptions);
    }

    public static void setOutfit(Outfit outfit)
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            pawn.outfits.CurrentOutfit = outfit;
            pawn.mindState.Notify_OutfitChanged();
        }
    }

    public static FloatMenu getOutfitMenu()
    {
        var outfitOptions = new List<FloatMenuOption>();
        foreach (var outfit in Current.Game.outfitDatabase.AllOutfits)
        {
            outfitOptions.Add(new FloatMenuOption(outfit.label, delegate { setOutfit(outfit); }));
        }

        return new FloatMenu(outfitOptions);
    }

    public static void setDrugPolicy(DrugPolicy dp)
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            pawn.drugs.CurrentPolicy = dp;
        }
    }

    public static FloatMenu getDrugPolicyMenu()
    {
        var drugPolicyOptions = new List<FloatMenuOption>();
        foreach (var dp in Current.Game.drugPolicyDatabase.AllPolicies)
        {
            drugPolicyOptions.Add(new FloatMenuOption(dp.label, delegate { setDrugPolicy(dp); }));
        }

        return new FloatMenu(drugPolicyOptions);
    }

    public static void setFoodRestriction(FoodRestriction fr)
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            if (pawn.foodRestriction?.CurrentFoodRestriction != null)
            {
                pawn.foodRestriction.CurrentFoodRestriction = fr;
            }
        }
    }

    public static FloatMenu getFoodRestrictionMenu()
    {
        var foodRestrictionOptions = new List<FloatMenuOption>();
        foreach (var fr in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
        {
            foodRestrictionOptions.Add(new FloatMenuOption(fr.label, delegate { setFoodRestriction(fr); }));
        }

        return new FloatMenu(foodRestrictionOptions);
    }

    public static void dropEverythingFromInventory()
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            pawn.inventory.DropAllNearPawn(pawn.Position);
        }
    }

    public static void setAllowedArea(Area a)
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.AreaRestriction = a;
            }
        }
    }

    public static FloatMenu getAllowedAreaMenu()
    {
        var allowedAreas = new List<FloatMenuOption>
            { new FloatMenuOption("Unrestricted", delegate { setAllowedArea(null); }) };
        foreach (var a in Find.CurrentMap.areaManager.AllAreas)
        {
            if (a.AssignableAsAllowed())
            {
                allowedAreas.Add(new FloatMenuOption(a.Label, delegate { setAllowedArea(a); }));
            }
        }

        return new FloatMenu(allowedAreas);
    }

    public static void setMedicalCare(MedicalCareCategory mcc)
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.medCare = mcc;
            }
        }
    }

    public static FloatMenu getMedicalCareMenu()
    {
        var medicalCareCategories = new List<FloatMenuOption>();
        foreach (MedicalCareCategory mcc in Enum.GetValues(typeof(MedicalCareCategory)))
        {
            medicalCareCategories.Add(new FloatMenuOption(mcc.GetLabel(), delegate { setMedicalCare(mcc); }));
        }

        return new FloatMenu(medicalCareCategories);
    }

    //all from HealthCardUtility.DrawMedOperationsTab
    public static string generateSurgeryText(Pawn pawn, RecipeDef recipe, BodyPartRecord part)
    {
        var text = recipe.Worker.GetLabelWhenUsedOn(pawn, part).CapitalizeFirst();
        if (part != null && !recipe.hideBodyPartNames)
        {
            text = text + " (" + part.Label + ")";
        }

        //text.Replace(" (teetotaler will be unhappy)", "");
        return text;
    }

    public static bool doesPawnAlreadyHaveSurgery(Pawn pawn, SurgeryOption surgery)
    {
        foreach (var b in pawn.BillStack.Bills)
        {
            if (b is not Bill_Medical existing)
            {
                continue;
            }

            if (existing.recipe != surgery.recipe)
            {
                continue;
            }

            if (existing.Part == surgery.part)
            {
                return true;
            }
        }

        return false;
    }

    public static bool canPawnGetSurgery(Pawn pawn, SurgeryOption surgery)
    {
        foreach (var allRecipe in pawn.def.AllRecipes)
        {
            if (!allRecipe.AvailableNow || !allRecipe.AvailableOnNow(pawn))
            {
                continue;
            }

            var enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.Map);
            if (enumerable.Any(x => x.isTechHediff) || enumerable.Any(x => x.IsDrug) ||
                enumerable.Any() && allRecipe.dontShowIfAnyIngredientMissing)
            {
                continue;
            }

            if (allRecipe.targetsBodyPart)
            {
                foreach (var item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                {
                    var temp = new SurgeryOption
                    {
                        text = generateSurgeryText(pawn, allRecipe, item),
                        recipe = allRecipe,
                        part = item
                    };
                    if (temp.equals(surgery))
                    {
                        return true;
                    }
                }
            }
            else
            {
                var temp = new SurgeryOption
                {
                    text = generateSurgeryText(pawn, allRecipe, null),
                    recipe = allRecipe,
                    part = null
                };
                if (temp.equals(surgery))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static List<SurgeryOption> getAllPossibleSurgeries(List<Pawn> pawns)
    {
        var set = new HashSet<SurgeryOption>();
        foreach (var pawn in pawns)
        {
            foreach (var allRecipe in pawn.def.AllRecipes)
            {
                if (!allRecipe.AvailableNow || !allRecipe.AvailableOnNow(pawn))
                {
                    continue;
                }

                var enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.Map);
                if (enumerable.Any(x => x.isTechHediff) || enumerable.Any(x => x.IsDrug) ||
                    enumerable.Any() && allRecipe.dontShowIfAnyIngredientMissing)
                {
                    continue;
                }

                if (allRecipe.targetsBodyPart)
                {
                    foreach (var item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                    {
                        var temp = new SurgeryOption
                        {
                            text = generateSurgeryText(pawn, allRecipe, item),
                            recipe = allRecipe,
                            part = item
                        };
                        if (!set.Contains(temp))
                        {
                            set.Add(temp);
                        }
                    }
                }
                else
                {
                    var temp = new SurgeryOption
                    {
                        text = generateSurgeryText(pawn, allRecipe, null),
                        recipe = allRecipe,
                        part = null
                    };
                    if (!set.Contains(temp))
                    {
                        set.Add(temp);
                    }
                }
            }
        }

        return set.ToList();
    }

    public static void addSurgeryIfNotAlready(List<Pawn> pawns, SurgeryOption surgery)
    {
        foreach (var pawn in pawns)
        {
            if (!canPawnGetSurgery(pawn, surgery) || doesPawnAlreadyHaveSurgery(pawn, surgery))
            {
                continue;
            }

            var bm = new Bill_Medical(surgery.recipe);
            pawn.BillStack.AddBill(bm);
            bm.Part = surgery.part;
        }
    }

    public static FloatMenu getBulkOperationFloatMenu()
    {
        var pawns = new List<Pawn>();
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is Pawn pawn)
            {
                pawns.Add(pawn);
            }
        }

        var available = getAllPossibleSurgeries(pawns);
        var menuAvailable = new List<FloatMenuOption>();
        foreach (var so in available)
        {
            menuAvailable.Add(new FloatMenuOption(so.text, delegate { addSurgeryIfNotAlready(pawns, so); }));
        }

        return new FloatMenu(menuAvailable);
    }

    public static List<ThingDef> getAllThingDefsIngestableFromInventoryBySelected()
    {
        var thingsToIngest = new List<ThingDef>();
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            if (!pawn.IsColonistPlayerControlled)
            {
                continue;
            }

            foreach (var thing in pawn.inventory.innerContainer.ToList())
            {
                if (!FoodUtility.WillIngestFromInventoryNow(pawn, thing))
                {
                    continue;
                }

                if (!thingsToIngest.Contains(thing.def))
                {
                    thingsToIngest.Add(thing.def);
                }
            }
        }

        return thingsToIngest;
    }

    public static void allSelectedIngestBySpecifiedDef(ThingDef td)
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Pawn pawn)
            {
                continue;
            }

            if (!pawn.IsColonistPlayerControlled)
            {
                continue;
            }

            foreach (var thing in pawn.inventory.innerContainer.ToList())
            {
                if (thing.def != td || !FoodUtility.WillIngestFromInventoryNow(pawn, thing))
                {
                    continue;
                }

                FoodUtility.IngestFromInventoryNow(pawn, thing);
                break;
            }
        }
    }

    public static FloatMenu getBulkConsumeFromInventoryFloatMenu()
    {
        var thingsToIngest = getAllThingDefsIngestableFromInventoryBySelected();
        var menuAvailable = new List<FloatMenuOption>();
        foreach (var td in thingsToIngest)
        {
            menuAvailable.Add(new FloatMenuOption("Consume " + td.label,
                delegate { allSelectedIngestBySpecifiedDef(td); }));
        }

        return new FloatMenu(menuAvailable);
    }

    public struct SurgeryOption
    {
        public string text;
        public RecipeDef recipe;
        public BodyPartRecord part;

        public bool equals(SurgeryOption so2)
        {
            return text == so2.text && recipe == so2.recipe && part == so2.part;
        }
    }
}