using AssemblyCSharp;
using Assets.scripts.Helpers.Utility;
using ModularMotion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.scripts.Managers.ExplorerScene_Scripts
{
    public class fieldInventoryController : MonoBehaviour
    {
        public GameObject fieldItemTemplate;
        public List<GameObject> fieldItems = new List<GameObject>();
        public UIMotion inventoryHolderUIController;
        [Header("Starting Items")]
        public List<ItemBase> StartingLoot = new List<ItemBase>();
        [Header("Inventory Sounds")]
        public List<AudioClip> openCraftingSounds = new List<AudioClip>();
        public List<AudioClip> showInventorySounds = new List<AudioClip>();
        [Header("Loot to add for testing")]
        public List<ItemBase> testingLoot = new List<ItemBase>();

        private void Start()
        {
            StartingLoot.ForEach(o =>
            {
                AddToObtainedItems(o);
            });
        }

        /// <summary>
        /// Add field Items from testingLoot for testing
        /// </summary>
        public void AddFieldItem()
        {
            testingLoot.ForEach(o =>
            {
                AddToObtainedItems(o);
            });
        }

        /// <summary>
        /// Returns current field items
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetFieldItems()
        {
            return fieldItems;
        }

        private bool HasBagSpace()
        {
            return GetFieldItems().Count < MainGameManager.instance.exploreManager.MaxBagSize;
        }

        /// <summary>
        /// Add or increate obtained field items to inventory display as well as set up the tooltip needed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="gameObject"></param>
        public void AddToObtainedItems(ItemBase item, GameObject gameObject = null)
        {
            if (!HasBagSpace())
            {
                print("Out of bag space");
                return;
            }

            if (fieldItems.Any(o => o.GetComponent<ExplorerItemsController>().itemBase == item) && item.canStack)
            {
                fieldItems.ForEach(o =>
                {
                    var itemController = o.GetComponent<ExplorerItemsController>();
                    if (itemController.itemBase == item && item.canStack)
                    {
                        itemController.total += 1;
                        if (gameObject)
                        {
                            gameObject.GetComponent<ExplorerItemsController>().DestroyToolTips();
                            Destroy(gameObject);
                        }
                    }
                });
            }
            else
            {
                //MainGameManager.instance.exploreManager.obtainedItems.Add(item);
                if (!gameObject)
                {
                    gameObject = item.InstantiateAsGameObject(MainGameManager.instance.exploreManager.inventoryHolder.transform);
                    fieldItems.Add(gameObject);
                } else
                {
                    var explorerItem = item.InstantiateAsGameObject(MainGameManager.instance.exploreManager.inventoryHolder.transform);
                    fieldItems.Add(explorerItem);
                    gameObject.GetComponent<ExplorerItemsController>().DestroyToolTips();
                    Destroy(gameObject);
                    //gameObject.transform.SetParent(MainGameManager.instance.exploreManager.inventoryHolder.transform, false);
                    //gameObject.GetComponent<BoxCollider2D>().size = new Vector2(50, 50);
                }
            }
        }


        /// <summary>
        /// Remove from fieldItems
        /// </summary>
        /// <param name="item"></param>
        public void RemoveFromObtainedItems(ItemBase item)
        {
            var itemInFieldItems = fieldItems.Where(o => o.GetComponent<ExplorerItemsController>().itemBase == item).FirstOrDefault();
            fieldItems.Remove(itemInFieldItems);
            Destroy(itemInFieldItems);
        }

        public void ShowCraftingView()
        {
            MainGameManager.instance.soundManager.playAllSounds(openCraftingSounds);
            MainGameManager.instance.SceneManager.LoadCrafting();
        }

        /// <summary>
        /// Show UI inventory slots
        /// </summary>
        public void ShowInventorySlots()
        {
            MainGameManager.instance.soundManager.playAllSounds(showInventorySounds);
            inventoryHolderUIController.Play();
        }
    }
}