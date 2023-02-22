using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BulkAssigner
{
    public static class BulkAssigner_Functions
    {
        public static void setHostilityResponseMode(HostilityResponseMode hrm)
        {
            foreach (object o in Find.Selector.SelectedObjects)
            {
                if (o is Pawn)
                {
                    Pawn p = (Pawn) o;
                    p.playerSettings.hostilityResponse = hrm;
                }
            }
        }

        public static FloatMenu getHostilityResponseModeMenu()
        {
            List<FloatMenuOption> hostilityResponseOptions = new List<FloatMenuOption>();
            foreach (HostilityResponseMode hrm in Enum.GetValues(typeof(HostilityResponseMode)))
            {
                hostilityResponseOptions.Add(new FloatMenuOption(hrm.ToString(), delegate { BulkAssigner_Functions.setHostilityResponseMode(hrm); }));
            }
            return new FloatMenu(hostilityResponseOptions);
        }

        public static void setOutfit(Outfit outfit)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn) obj;
                    p.outfits.CurrentOutfit = outfit;
                    p.mindState.Notify_OutfitChanged();
                }
            }
        }

        public static FloatMenu getOutfitMenu()
        {
            List<FloatMenuOption> outfitOptions = new List<FloatMenuOption>();
            foreach (Outfit outfit in Current.Game.outfitDatabase.AllOutfits)
            {
                outfitOptions.Add(new FloatMenuOption(outfit.label, delegate { BulkAssigner_Functions.setOutfit(outfit); }));
            }
            return new FloatMenu(outfitOptions);
        }

        public static void setDrugPolicy(DrugPolicy dp)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn )obj;
                    p.drugs.CurrentPolicy = dp;
                }
            }
        }

        public static FloatMenu getDrugPolicyMenu()
        {
            List<FloatMenuOption> drugPolicyOptions = new List<FloatMenuOption>();
            foreach (DrugPolicy dp in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                drugPolicyOptions.Add(new FloatMenuOption(dp.label, delegate { BulkAssigner_Functions.setDrugPolicy(dp); }));
            }
            return new FloatMenu(drugPolicyOptions);
        }

        public static void setFoodRestriction(FoodRestriction fr)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn)obj;
                    if (p?.foodRestriction?.CurrentFoodRestriction != null)
                    {
                        p.foodRestriction.CurrentFoodRestriction = fr;
                    }
                }
            }
        }

        public static FloatMenu getFoodRestrictionMenu()
        {
            List<FloatMenuOption> foodRestrictionOptions = new List<FloatMenuOption>();
            foreach (FoodRestriction fr in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
            {
                foodRestrictionOptions.Add(new FloatMenuOption(fr.label, delegate { BulkAssigner_Functions.setFoodRestriction(fr); }));
            }
            return new FloatMenu(foodRestrictionOptions);
        }

        public static void dropEverythingFromInventory()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn)obj;
                    p.inventory.DropAllNearPawn(p.Position);
                }
            }
        }

        public static void setAllowedArea(Area a)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn)obj;
                    if (p?.playerSettings != null)
                    {
                        p.playerSettings.AreaRestriction = a;
                    }
                }
            }
        }

        public static FloatMenu getAllowedAreaMenu()
        {
            List<FloatMenuOption> allowedAreas = new List<FloatMenuOption>();
            allowedAreas.Add(new FloatMenuOption("Unrestricted", delegate { BulkAssigner_Functions.setAllowedArea(null); }));
            foreach (Area a in Find.CurrentMap.areaManager.AllAreas)
            {
                if (a.AssignableAsAllowed())
                {
                    allowedAreas.Add(new FloatMenuOption(a.Label, delegate { BulkAssigner_Functions.setAllowedArea(a); }));
                }
            }
            return new FloatMenu(allowedAreas);
        }

        public static void setMedicalCare(MedicalCareCategory mcc)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn)obj;
                    if (p?.playerSettings != null)
                    {
                        p.playerSettings.medCare = mcc;
                    }
                }
            }
        }

        public static FloatMenu getMedicalCareMenu()
        {
            List<FloatMenuOption> medicalCareCategories = new List<FloatMenuOption>();
            foreach (MedicalCareCategory mcc in Enum.GetValues(typeof(MedicalCareCategory)))
            {
                medicalCareCategories.Add(new FloatMenuOption(mcc.GetLabel(), delegate { BulkAssigner_Functions.setMedicalCare(mcc); }));
            }
            return new FloatMenu(medicalCareCategories);
        }

        public struct SurgeryOption
        {
            public string text;
            public RecipeDef recipe;
            public BodyPartRecord part;

            public bool equals(SurgeryOption so2)
            {
                return this.text == so2.text && this.recipe == so2.recipe && this.part == so2.part;
            }
        }

        //all from HealthCardUtility.DrawMedOperationsTab
        public static string generateSurgeryText(Pawn pawn, RecipeDef recipe, BodyPartRecord part)
        {
            string text = recipe.Worker.GetLabelWhenUsedOn(pawn, part).CapitalizeFirst();
            if (part != null && !recipe.hideBodyPartNames)
            {
                text = text + " (" + part.Label + ")";
            }
            //text.Replace(" (teetotaler will be unhappy)", "");
            return text;
        }

        public static bool doesPawnAlreadyHaveSurgery(Pawn pawn, SurgeryOption surgery)
        {
            foreach (Bill b in pawn.BillStack.Bills)
            {
                if (b is Bill_Medical)
                {
                    Bill_Medical existing = (Bill_Medical)b;
                    if (existing.recipe == surgery.recipe)
                    {
                        if (existing.Part == surgery.part)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool canPawnGetSurgery(Pawn pawn, SurgeryOption surgery)
        {
            foreach (RecipeDef allRecipe in pawn.def.AllRecipes)
            {
                if (allRecipe.AvailableNow && allRecipe.AvailableOnNow(pawn))
                {
                    IEnumerable<ThingDef> enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.Map);
                    if (!enumerable.Any((ThingDef x) => x.isTechHediff) && !enumerable.Any((ThingDef x) => x.IsDrug) && (!enumerable.Any() || !allRecipe.dontShowIfAnyIngredientMissing))
                    {
                        if (allRecipe.targetsBodyPart)
                        {
                            foreach (BodyPartRecord item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                            {
                                SurgeryOption temp = new SurgeryOption();
                                temp.text = generateSurgeryText(pawn, allRecipe, item);
                                temp.recipe = allRecipe;
                                temp.part = item;
                                if (temp.equals(surgery)) { return true; }
                            }
                        }
                        else
                        {
                            SurgeryOption temp = new SurgeryOption();
                            temp.text = generateSurgeryText(pawn, allRecipe, null);
                            temp.recipe = allRecipe;
                            temp.part = null;
                            if (temp.equals(surgery)) { return true; }
                        }
                    }
                }
            }
            return false;
        }

        public static List<SurgeryOption> getAllPossibleSurgeries(List<Pawn> pawns)
        {
            HashSet<SurgeryOption> set = new HashSet<SurgeryOption>();
            foreach (Pawn pawn in pawns)
            {
                foreach (RecipeDef allRecipe in pawn.def.AllRecipes)
                {
                    if (allRecipe.AvailableNow && allRecipe.AvailableOnNow(pawn))
                    {
                        IEnumerable<ThingDef> enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.Map);
                        if (!enumerable.Any((ThingDef x) => x.isTechHediff) && !enumerable.Any((ThingDef x) => x.IsDrug) && (!enumerable.Any() || !allRecipe.dontShowIfAnyIngredientMissing))
                        {
                            if (allRecipe.targetsBodyPart)
                            {
                                foreach (BodyPartRecord item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                                {
                                    SurgeryOption temp = new SurgeryOption();
                                    temp.text = generateSurgeryText(pawn, allRecipe, item);
                                    temp.recipe = allRecipe;
                                    temp.part = item;
                                    if (!set.Contains(temp)) { set.Add(temp); }
                                }
                            }
                            else
                            {
                                SurgeryOption temp = new SurgeryOption();
                                temp.text = generateSurgeryText(pawn, allRecipe, null);
                                temp.recipe = allRecipe;
                                temp.part = null;
                                if (!set.Contains(temp)) { set.Add(temp); }
                            }
                        }
                    }
                }
            }
            return set.ToList();
        }

        public static void addSurgeryIfNotAlready(List<Pawn> pawns, SurgeryOption surgery)
        {
            foreach (Pawn pawn in pawns)
            {
                if (canPawnGetSurgery(pawn, surgery) && !doesPawnAlreadyHaveSurgery(pawn, surgery))
                {
                    //Bill_Medical bm = new Bill_Medical(surgery.recipe);
                    //pawn.BillStack.AddBill(bm);
                    //bm.Part = surgery.part;
                }
            }
        }

        public static FloatMenu getBulkOperationFloatMenu()
        {
            List<Pawn> pawns = new List<Pawn>();
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    pawns.Add((Pawn)obj);
                }
            }
            List<SurgeryOption> available = getAllPossibleSurgeries(pawns);
            List<FloatMenuOption> menuAvailable = new List<FloatMenuOption>();
            foreach (SurgeryOption so in available)
            {
                menuAvailable.Add(new FloatMenuOption(so.text, delegate { addSurgeryIfNotAlready(pawns, so); }));
            }
            return new FloatMenu(menuAvailable);
        }

        public static List<ThingDef> getAllThingDefsIngestableFromInventoryBySelected()
        {
            List<Pawn> pawns = new List<Pawn>();
            List<ThingDef> thingsToIngest = new List<ThingDef>();
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn pawn = obj as Pawn;
                    if (pawn.IsColonistPlayerControlled)
                    {
                        foreach (Thing thing in pawn.inventory.innerContainer.ToList())
                        {
                            if (FoodUtility.WillIngestFromInventoryNow(pawn, thing))
                            {
                                if (!thingsToIngest.Contains(thing.def))
                                {
                                    thingsToIngest.Add(thing.def);
                                }
                            }
                        }
                    }
                }
            }
            return thingsToIngest;
        }

        public static void allSelectedIngestBySpecifiedDef(ThingDef td)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn pawn = obj as Pawn;
                    if (pawn.IsColonistPlayerControlled)
                    {
                        foreach (Thing thing in pawn.inventory.innerContainer.ToList())
                        {
                            if (thing.def == td && FoodUtility.WillIngestFromInventoryNow(pawn, thing))
                            {
                                FoodUtility.IngestFromInventoryNow(pawn, thing);
                                break;
                            }
                        }
                    }
                }
            }

        }

        public static FloatMenu getBulkConsumeFromInventoryFloatMenu()
        {
            List<ThingDef> thingsToIngest = getAllThingDefsIngestableFromInventoryBySelected();
            List<FloatMenuOption> menuAvailable = new List<FloatMenuOption>();
            foreach (ThingDef td in thingsToIngest)
            {
                menuAvailable.Add(new FloatMenuOption("Consume " + td.label, delegate { allSelectedIngestBySpecifiedDef(td); }));
            }
            return new FloatMenu(menuAvailable);
        }
    }
}
