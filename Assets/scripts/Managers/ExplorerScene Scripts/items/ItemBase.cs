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
        //[HideInInspector]
        //public int quantity = 1;
        public itemQuality quality;
        public classRestriction classReq;
        public enum classRestriction
        {
            None,
            Guardian,
            Stalker,
            Walker
        };
        [Range(0.0f, 1.0f)]
        public float dropChancePercentage;

        /// <summary>
        /// Instantiate Item as GameObject in the world
        /// </summary>
        /// <returns></returns>
        public GameObject InstantiateAsGameObject(Transform tranform)
        {
            var fieldItem = MainGameManager.instance.assetFinder.GetGameObjectFromPath("Assets/prefabs/explorer/items/explorerItem.prefab");
            var gameObject = Instantiate(fieldItem, tranform);
            var explorererItemController = gameObject.GetComponent<ExplorerItemsController>();
            explorererItemController.itemBase = this;
            explorererItemController.SetUpItem();
            return gameObject;
        }
    }
}