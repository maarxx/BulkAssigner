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
    }
}
