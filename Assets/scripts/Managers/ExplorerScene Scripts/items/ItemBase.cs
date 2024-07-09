using Assets.scripts.Helpers.Utility;
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
        public string itemName
        {
            get {
                return LabelConverter.ConvertCamelCaseToWord(this.name);
            }
        }
        [TextArea]
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
        [Range(0.0f, 1.0f)]
        public float dropChancePercentage;

        /// <summary>
        /// Instantiate Item as GameObject in the world and load necessary components
        /// </summary>
        /// <returns></returns>
        public GameObject InstantiateAsGameObject(Transform tranform)
        {
            var fieldItem = MainGameManager.instance.ItemFinder.ExplorerItem;
            var gameObject = Instantiate(fieldItem, tranform);
            var explorererItemController = gameObject.GetComponent<ExplorerItemsController>();
           // explorererItemController.itemBase = this;
            explorererItemController.SetUpItem(this, "obtained_item");
            if (explorererItemController.itemBase.GetType() == typeof(Consumable))
            {
                var interactWithObjectController = gameObject.AddComponent<InteractWithObjectController>();
                var consumable = (Consumable)explorererItemController.itemBase;
                interactWithObjectController.SetMessageText("Use Item", "Cancel", consumable);
                consumable.effectsOnUse.ForEach(effect =>
                {
                    effect.LoadEffectData(consumable.power, 0, 1, true, effect.EffectGrp, true, null, 0); //lots of values here are irrelevant for explorer item use
                    interactWithObjectController.useItemAction += effect.RunEffectFromItemToRoles;
                });
            }
            return gameObject;
        }
    }
}