using AssemblyCSharp;
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
            if (ExploreManager.obtainedItems.Any(o => o == item))
            {
                fieldItems.ForEach(o =>
                {
                    var itemController = o.GetComponent<ExplorerItemsController>();
                    if (itemController.itemBase == item)
                    {
                        itemController.total += 1;
                        itemController.SetUpItem();
                    }
                });
            }
            else
            {
                ExploreManager.obtainedItems.Add(item);
                if (!gameObject)
                {
                    gameObject = item.InstantiateAsGameObject(ExploreManager.inventoryHolder.transform);
                    /*gameObject = Instantiate(fieldItemTemplate, ExploreManager.inventoryHolder.transform);
                    var explorererItemController = gameObject.GetComponent<ExplorerItemsController>();
                    explorererItemController.itemBase = item;
                    explorererItemController.SetUpItem();*/
                }
                fieldItems.Add(gameObject);
            }
        }

        void OnMouseUp()
        {
            MainGameManager.instance.SceneManager.LoadCrafting();
        }
    }
}