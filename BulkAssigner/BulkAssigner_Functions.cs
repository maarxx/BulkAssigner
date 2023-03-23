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

        public static FloatMenu getBulkOperationFloatMenu()
        {

        }

        public static List<FloatMenuOption> getAllSurgeryOptionsForPawn(Pawn pawn)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            int num = 0;
            foreach (RecipeDef allRecipe in pawn.def.AllRecipes)
            {
                if (allRecipe.AvailableNow)
                {
                    AcceptanceReport report = allRecipe.Worker.AvailableReport(pawn);
                    if (report.Accepted || !report.Reason.NullOrEmpty())
                    {
                        IEnumerable<ThingDef> enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.MapHeld);
                        if (!enumerable.Any((ThingDef x) => x.isTechHediff) && !enumerable.Any((ThingDef x) => x.IsDrug) && (!enumerable.Any() || !allRecipe.dontShowIfAnyIngredientMissing))
                        {
                            if (allRecipe.targetsBodyPart)
                            {
                                foreach (BodyPartRecord item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                                {
                                    if (allRecipe.AvailableOnNow(pawn, item))
                                    {
                                        list.Add(GenerateSurgeryOption(pawn, pawn, allRecipe, enumerable, report, num, item));
                                        num++;
                                    }
                                }
                            }
                            else
                            {
                                list.Add(GenerateSurgeryOption(pawn, pawn, allRecipe, enumerable, report, num));
                                num++;
                            }
                        }
                    }
                }
            }
            return list;
        }

        private static FloatMenuOption GenerateSurgeryOption(Pawn pawn, Thing thingForMedBills, RecipeDef recipe, IEnumerable<ThingDef> missingIngredients, AcceptanceReport report, int index, BodyPartRecord part = null)
        {
            string label = recipe.Worker.GetLabelWhenUsedOn(pawn, part).CapitalizeFirst();
            if (part != null && !recipe.hideBodyPartNames)
            {
                label = label + " (" + part.Label + ")";
            }
            FloatMenuOption floatMenuOption;
            if (!report.Reason.NullOrEmpty())
            {
                label = label + " (" + report.Reason + ")";
                floatMenuOption = new FloatMenuOption(label, null);
            }
            else if (missingIngredients.Any())
            {
                label += " (";
                bool flag = true;
                foreach (ThingDef missingIngredient in missingIngredients)
                {
                    if (!flag)
                    {
                        label += ", ";
                    }
                    flag = false;
                    label += "MissingMedicalBillIngredient".Translate(missingIngredient.label);
                }
                label += ")";
                floatMenuOption = new FloatMenuOption(label, null);
            }
            else
            {
                Action action = delegate
                {
                    Pawn medPawn = thingForMedBills as Pawn;
                    if (medPawn != null)
                    {
                        HediffDef hediffDef = recipe.addsHediff ?? recipe.changesHediffLevel;
                        if (hediffDef != null)
                        {
                            TaggedString text = CompRoyalImplant.CheckForViolations(medPawn, hediffDef, recipe.hediffLevelOffset);
                            if (!text.NullOrEmpty())
                            {
                                Find.WindowStack.Add(new Dialog_MessageBox(text, "Yes".Translate(), delegate
                                {
                                    HealthCardUtility.CreateSurgeryBill(medPawn, recipe, part);
                                }, "No".Translate()));
                            }
                            else
                            {
                                TaggedString confirmation = recipe.Worker.GetConfirmation(medPawn);
                                if (!confirmation.NullOrEmpty())
                                {
                                    Find.WindowStack.Add(new Dialog_MessageBox(confirmation, "Yes".Translate(), delegate
                                    {
                                        HealthCardUtility.CreateSurgeryBill(medPawn, recipe, part);
                                    }, "No".Translate()));
                                }
                                else
                                {
                                    HealthCardUtility.CreateSurgeryBill(medPawn, recipe, part);
                                }
                            }
                        }
                        else if (recipe.Worker is Recipe_ImplantXenogerm)
                        {
                            Find.WindowStack.Add(new Dialog_SelectXenogerm(pawn, pawn.Map, null, delegate (Xenogerm x)
                            {
                                x.SetTargetPawn(pawn);
                            }));
                        }
                        else
                        {
                            HealthCardUtility.CreateSurgeryBill(medPawn, recipe, part);
                        }
                    }
                };
                floatMenuOption = ((recipe.Worker is Recipe_AdministerIngestible) ? new FloatMenuOption(label, action, recipe.ingredients.FirstOrDefault()?.FixedIngredient) : ((!(recipe.Worker is Recipe_RemoveBodyPart)) ? new FloatMenuOption(label, action, recipe.UIIconThing) : new FloatMenuOption(label, action, part.def.spawnThingOnRemoved)));
            }
            floatMenuOption.extraPartWidth = 29f;
            floatMenuOption.extraPartOnGUI = ((Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, recipe));
            floatMenuOption.mouseoverGuiAction = delegate (Rect rect)
            {
                BillUtility.DoBillInfoWindow(index, label, rect, recipe);
            };
            return floatMenuOption;
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
