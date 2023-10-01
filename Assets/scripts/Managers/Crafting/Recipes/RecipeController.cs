using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<Recipe> recipes = new List<Recipe>();

        public RecipeResult HasCombination(List<ItemBase> items, CraftingCatalyst catalyst)
        {

            RecipeResult res = new RecipeResult();
            recipes.ForEach(o =>
            {
                var r = new List<ItemBase>()
                {
                    o.requiredItem1, o.requiredItem2, o.requiredItem3
                }.OrderBy(i => i == null).ToList();
                r.RemoveAll(i => i == null);
                if (o.requiredCatalyst == catalyst)
                {
                    switch (o.mixMode)
                    {
                        case MixMode.requiresAllOf:
                            if (r.All(items.Contains) && r.Count == items.Count)
                            {
                                res.items.AddRange(o.results);
                            };
                            break;
                        case MixMode.requiresAnyOf:
                            if (r.Any(items.Contains))
                            {
                                res.items.AddRange(o.results);
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
