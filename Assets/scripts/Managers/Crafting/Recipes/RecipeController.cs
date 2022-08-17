using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.scripts.Managers.Crafting.Recipes
{
    public class RecipeController : MonoBehaviour
    {
        public List<Recipe> recipes = new List<Recipe>();

        public List<ItemBase> HasCombination(List<ItemBase> items, CraftingCatalyst catalyst)
        {
            List<ItemBase> res = new List<ItemBase>();
            recipes.ForEach(o =>
            {
                var r = new List<ItemBase>()
                {
                    o.requiredItem1, o.requiredItem2, o.requiredItem3
                }.OrderBy(i => i == null).ToList();
                r.RemoveAll(i => i == null);
                if (o.requiredCatalyst == catalyst)
                {
                    if (r.All(items.Contains) && r.Count == items.Count)
                    {
                        res.AddRange(o.results);
                    };
                }
            });
            return res;
        }
    }
}
