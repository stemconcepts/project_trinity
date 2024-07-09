using AssemblyCSharp;
using Assets.scripts.Helpers.Utility;
using Assets.scripts.Managers.Crafting.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.scripts.Managers.Crafting.DiscoveredRecipes
{
    public class DiscoveredRecipeController : MonoBehaviour
    {
        protected Recipe Recipe;
        protected Sprite ItemIcon;
        protected CraftingController CraftingController;
        public TextMeshProUGUI TextMesh;
        public ToolTipTriggerController ToolTipController;

        public void SetRecipe(Recipe recipe, CraftingController craftingController)
        {
            Recipe = recipe;
            CraftingController = craftingController;
            TextMesh.text = LabelConverter.ConvertCamelCaseToWord(recipe.name);

            var initialResult = recipe.results.FirstOrDefault();
            ItemIcon = initialResult?.itemIcon;
            ToolTipController.AddtoolTip(initialResult.id, initialResult.name, initialResult.itemDesc);
            ToolTipController.enabled = true;
        }

        public void LoadRecipe()
        {
            var requiredItems = Recipe.GetRequiredItemList();

            foreach (var item in requiredItems)
            {
                CraftingController.AddToCraftingCombination(item);
            }

            CraftingController.catalystSlider.value = (int)Recipe.requiredCatalyst;
            CraftingController.SetCatalyst();
        }
    }
}
