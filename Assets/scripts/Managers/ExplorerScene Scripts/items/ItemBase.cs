using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class ItemBase : ScriptableObject
    {
        public string itemName;
        [Multiline]
        public string itemDesc;
        public Sprite itemIcon;
        public bool canStack;
        public itemQuality quality;
        public enum itemQuality
        {
            Common,
            Rare,
            Epic,
            Legendary
        };
        public classRestriction classReq;
        public enum classRestriction
        {
            None,
            Guardian,
            Stalker,
            Walker
        };
    }
}