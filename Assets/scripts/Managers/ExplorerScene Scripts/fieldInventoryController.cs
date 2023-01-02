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
        public List<ItemBase> testingLoot = new List<ItemBase>();
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

        /// <summary>
        /// Add or increate obtained field items to inventory display as well as set up the tooltip needed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="gameObject"></param>
        public void AddToObtainedItems(ItemBase item, GameObject gameObject = null)
        {
            if (/*ExploreManager.obtainedItems.Any(o => o == item)*/ fieldItems.Any(o => o.GetComponent<ExplorerItemsController>().itemBase == item))
            {
                fieldItems.ForEach(o =>
                {
                    var itemController = o.GetComponent<ExplorerItemsController>();
                    if (itemController.itemBase == item && item.canStack)
                    {
                        itemController.total += 1;
                        gameObject.GetComponent<ExplorerItemsController>().DestroyToolTips();
                        Destroy(gameObject);
                    }
                });
            }
            else
            {
                //ExploreManager.obtainedItems.Add(item);
                if (!gameObject)
                {
                    gameObject = item.InstantiateAsGameObject(ExploreManager.inventoryHolder.transform);
                    fieldItems.Add(gameObject);
                } else
                {
                    var explorerItem = item.InstantiateAsGameObject(ExploreManager.inventoryHolder.transform);
                    fieldItems.Add(explorerItem);
                    gameObject.GetComponent<ExplorerItemsController>().DestroyToolTips();
                    Destroy(gameObject);
                    //gameObject.transform.SetParent(ExploreManager.inventoryHolder.transform, false);
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
            MainGameManager.instance.SceneManager.LoadCrafting();
        }

        /// <summary>
        /// Show UI inventory slots
        /// </summary>
        public void ShowInventorySlots()
        {
            inventoryHolderUIController.Play();
        }
    }
}