using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Managers.Crafting.Recipes
{
    [System.Serializable]
    public class Recipe : ScriptableObject
    {
        public ItemBase requiredItem1;
        public ItemBase requiredItem2;
        public ItemBase requiredItem3;
        public CraftingCatalyst requiredCatalyst;
        public List<ItemBase> results = new List<ItemBase>();
    }
}
