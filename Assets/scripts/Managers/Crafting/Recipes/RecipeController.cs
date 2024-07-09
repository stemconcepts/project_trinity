using AssemblyCSharp;
using Assets.scripts.Helpers.Utility;
using Assets.scripts.Managers.Crafting.DiscoveredRecipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


namespace Assets.scripts.Managers.Crafting.Recipes
{
    public class RecipeResult
    {
        public List<ItemBase> items = new List<ItemBase>();
        public MixMode mixMode;
    }

    public class RecipeController : MonoBehaviour
    {
        public List<Recipe> Recipes = new List<Recipe>();
        public CraftingController CraftingController;
        public GameObject DiscoveredRecipeHolder;
        public GameObject RecipeTemplate;

        private void Start()
        {
            RefreshDiscoveredPanel();
        }

        private void RefreshDiscoveredPanel()
        {
            if (DiscoveredRecipeHolder) { 
                if (DiscoveredRecipeHolder.transform.childCount > 0)
                {
                    for (int i = 0; i < DiscoveredRecipeHolder.transform.childCount; i++)
                    {
                        var child = DiscoveredRecipeHolder.transform.GetChild(i);
                        Destroy(child.gameObject);
                    }
                }
                foreach (var recipe in GetDiscoveredRecipes())
                {
                    var newRecipe = Instantiate(RecipeTemplate, DiscoveredRecipeHolder.transform);
                    var recipeController = newRecipe.GetComponent<DiscoveredRecipeController>();
                    recipeController.SetRecipe(recipe, CraftingController);
                }
            }
        }

        private List<Recipe> GetDiscoveredRecipes()
        {
            return Recipes
                .OrderBy(recipe => recipe.name)
                .Where(recipe => recipe.Discovered)
                .ToList();
        }

        public RecipeResult HasCombination(List<ItemBase> items, CraftingCatalyst catalyst)
        {

            RecipeResult res = new RecipeResult();
            Recipes.ForEach(recipe =>
            {
                var requiredItems = recipe.GetRequiredItemList();

                if (recipe.requiredCatalyst == catalyst)
                {
                    switch (recipe.mixMode)
                    {
                        case MixMode.requiresAllOf:
                            if (requiredItems.All(items.Contains) && requiredItems.Count == items.Count)
                            {
                                res.items.AddRange(recipe.results);
                                recipe.SetDiscovered();
                                RefreshDiscoveredPanel();
                            };
                            break;
                        case MixMode.requiresAnyOf:
                            if (requiredItems.Any(items.Contains))
                            {
                                res.items.AddRange(recipe.results);
                                recipe.SetDiscovered();
                                RefreshDiscoveredPanel();
                            };
                            break;
                        default:
                            break;
                    }
                }
            });
            return res;
        }
    }
}
