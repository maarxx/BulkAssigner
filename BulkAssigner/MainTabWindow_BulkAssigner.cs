using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BulkAssigner
{
    public class MainTabWindow_BulkAssigner : MainTabWindow
    {

        private const float BUTTON_HEIGHT = 50f;
        private const float BUTTON_SPACE = 10f;


        public MainTabWindow_BulkAssigner()
        {
            //base.forcePause = true;
        }

        public override Vector2 InitialSize
        {
            get
            {
                //return base.InitialSize;
                return new Vector2(250f, 400f);
            }
        }

        public override MainTabWindowAnchor Anchor =>
            MainTabWindowAnchor.Right;

        public void setHostilityResponseMode(HostilityResponseMode hrm)
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

        public void setOutfit(Outfit outfit)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn) obj;
                    p.outfits.CurrentOutfit = outfit;
                }
            }
        }

        public void setDrugPolicy(DrugPolicy dp)
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

        public void setFoodRestriction(FoodRestriction fr)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn)obj;
                    p.foodRestriction.CurrentFoodRestriction = fr;
                }
            }
        }

        public void dropEverythingFromInventory()
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

        public void setAllowedArea(Area a)
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                if (obj is Pawn)
                {
                    Pawn p = (Pawn)obj;
                    p.playerSettings.AreaRestriction = a;
                }
            }
        }

        public override void DoWindowContents(Rect canvas)
        {
            base.DoWindowContents(canvas);

            List<FloatMenuOption> hostilityResponseOptions = new List<FloatMenuOption>();
            foreach (HostilityResponseMode hrm in Enum.GetValues(typeof(HostilityResponseMode)))
            {
                hostilityResponseOptions.Add(new FloatMenuOption(hrm.ToString(), delegate { setHostilityResponseMode(hrm); }));
            }

            List<FloatMenuOption> outfitOptions = new List<FloatMenuOption>();
            foreach (Outfit outfit in Current.Game.outfitDatabase.AllOutfits)
            {
                outfitOptions.Add(new FloatMenuOption(outfit.label, delegate { setOutfit(outfit); }));
            }

            List<FloatMenuOption> drugPolicyOptions = new List<FloatMenuOption>();
            foreach (DrugPolicy dp in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                drugPolicyOptions.Add(new FloatMenuOption(dp.label, delegate { setDrugPolicy(dp); }));
            }

            List<FloatMenuOption> foodRestrictionOptions = new List<FloatMenuOption>();
            foreach (FoodRestriction fr in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
            {
                foodRestrictionOptions.Add(new FloatMenuOption(fr.label, delegate { setFoodRestriction(fr); }));
            }

            List<FloatMenuOption> allowedAreas = new List<FloatMenuOption>();
            allowedAreas.Add(new FloatMenuOption("Unrestricted", delegate { setAllowedArea(null); }));
            foreach (Area a in Find.CurrentMap.areaManager.AllAreas)
            {
                if (a.AssignableAsAllowed())
                {
                    allowedAreas.Add(new FloatMenuOption(a.Label, delegate { setAllowedArea(a); }));
                }
            }

            Text.Font = GameFont.Small;
            for (int i = 0; i <= 5; i++)
            {
                Rect nextButton = new Rect(canvas);
                nextButton.y = i * (BUTTON_HEIGHT + BUTTON_SPACE);
                nextButton.height = BUTTON_HEIGHT;

                string buttonLabel;
                switch (i)
                {
                    case 0:
                        buttonLabel = "Set Enemy Response";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            Find.WindowStack.Add(new FloatMenu(hostilityResponseOptions));
                        }
                        break;
                    case 1:
                        buttonLabel = "Set Outfit";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            Find.WindowStack.Add(new FloatMenu(outfitOptions));
                        }
                        break;
                    case 2:
                        buttonLabel = "Set Drug Policy";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            Find.WindowStack.Add(new FloatMenu(drugPolicyOptions));
                        }
                        break;
                    case 3:
                        buttonLabel = "Set Food Restriction";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            Find.WindowStack.Add(new FloatMenu(foodRestrictionOptions));
                        }
                        break;
                    case 4:
                        buttonLabel = "Set Allowed Area";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            Find.WindowStack.Add(new FloatMenu(allowedAreas));
                        }
                        break;
                    case 5:
                        buttonLabel = "Drop Everything";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            dropEverythingFromInventory();
                        }
                        break;
                }
            }
        }

    }
}
