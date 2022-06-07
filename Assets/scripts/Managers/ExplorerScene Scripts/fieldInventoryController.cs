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
            } else
            {
                ExploreManager.obtainedItems.Add(item);
                if (!gameObject)
                {
                    gameObject = Instantiate(fieldItemTemplate, ExploreManager.inventoryHolder.transform);
                    var explorererItemController = gameObject.GetComponent<ExplorerItemsController>();
                    explorererItemController.itemBase = item;
                    explorererItemController.SetUpItem();
                }
                //gameObject.transform.localScale = new Vector3(15f, 15f, 1f);
                fieldItems.Add(gameObject);
            }
        }
    }
}