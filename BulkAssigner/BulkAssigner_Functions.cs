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
    }
}
