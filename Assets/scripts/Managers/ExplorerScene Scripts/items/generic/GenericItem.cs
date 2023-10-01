using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace AssemblyCSharp
{
    public class GenericItem : ItemBase
    {
        public int sellPrice;
        public int buyPrice;
        public ResourceSourceType resourceSourceType;
        /*[Range(0.0f, 1.0f)]
        public float dropChancePercentage;*/
    }
}