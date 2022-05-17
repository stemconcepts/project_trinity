using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public enum itemQuality
    {
        Common,
        Rare,
        Epic,
        Legendary
    };

    [System.Serializable]
    public class ItemBase : ScriptableObject
    {
        public string id;
        public string itemName;
        [Multiline]
        public string itemDesc;
        public Sprite itemIcon;
        public bool canStack;
        public itemQuality quality;
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