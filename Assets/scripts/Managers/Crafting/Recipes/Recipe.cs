using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Managers.Crafting.Recipes
{
    public enum MixMode
    {
        requiresAllOf,
        requiresAnyOf
    }

    [System.Serializable]
    public class Recipe : ScriptableObject
    {
        public MixMode mixMode;
        public bool Discovered;
        public ItemBase requiredItem1;
        public ItemBase requiredItem2;
        public ItemBase requiredItem3;
        public CraftingCatalyst requiredCatalyst;
        public List<ItemBase> results = new List<ItemBase>();

        public void SetDiscovered()
        {
            this.Discovered = true;
        }

        public List<ItemBase> GetRequiredItemList()
        {
            var items = new List<ItemBase>()
                {
                    requiredItem1, 
                    requiredItem2, 
                    requiredItem3
                }.OrderBy(i => i == null).ToList();
            items.RemoveAll(i => i == null);
            return items;
        }
    }
}
