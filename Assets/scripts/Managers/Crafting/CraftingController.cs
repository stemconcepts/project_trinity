using AssemblyCSharp;
using Assets.scripts.Helpers.Utility;
using Assets.scripts.Managers.Crafting.Recipes;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Managers.Crafting
{
    public enum CraftingCatalyst
    {
        grind,
        boil,
        burn
    }

    public class CraftingController : MonoBehaviour
    {
        //public List<GameObject> fieldItems = new List<GameObject>();
        public GameObject itemHolder;
        public GameObject craftingGroupHolder;
        public RecipeController recipeController;
        public Slider catalystSlider;
        public TextMeshProUGUI currentCatalystText;
        public List<ItemBase> itemCombinationToCraft;
        fieldInventoryController fieldInventoryController;
        CraftingCatalyst activeCatalyst;

        /// <summary>
        /// Close crafting panel
        /// </summary>
        public void ExitCrafting()
        {
            MoveFieldItemsBackToExplorer();
            MainGameManager.instance.SceneManager.UnLoadScene("craftingOverlay");
            MainGameManager.instance.GetActiveBoxColliders().ForEach(o =>
            {
                o.enabled = true;
            });
        }

        /// <summary>
        /// Add item to crafting combination list
        /// </summary>
        /// <param name="item"></param>
        void AddToCraftingCombination(ItemBase item)
        {
            itemCombinationToCraft.Add(item);
        }

        /// <summary>
        /// Add field items to crafting view and load relevant scripts
        /// </summary>
        void AddFromFieldItems()
        {
            var fi = fieldInventoryController.GetFieldItems();
            fi.ForEach(o =>
            {
                SetItemForCraftingControls(o);
            });
        }

        void MoveFieldItemsBackToExplorer()
        {
            var fi = fieldInventoryController.GetFieldItems();
            fi.ForEach(o =>
            {
                o.transform.SetParent(fieldInventoryController.gameObject.transform);
                if (o.GetComponent<DragAndDropController>())
                {
                    o.GetComponent<DragAndDropController>().enabled = false;
                }
            });
        }

        /// <summary>
        /// Add classes to items so that they work within the crafting system
        /// </summary>
        /// <param name="item"></param>
        void SetItemForCraftingControls(GameObject item)
        {
            item.AddComponent<BoxColliderGenerator>();
            item.transform.SetParent(itemHolder.transform);
            var dd = item.GetComponent<DragAndDropController>();
            if (!dd)
            {
                var d = item.AddComponent<DragAndDropController>();
                d.draggable = true;
                d.mouseUpAction += AddToCraftingCombination;
                d.dragAndDropMaster = craftingGroupHolder.GetComponent<DragAndDropMasterController>();
            } else
            {
                dd.mouseUpAction += AddToCraftingCombination;
                dd.dragAndDropMaster = craftingGroupHolder.GetComponent<DragAndDropMasterController>();
            }
            item.GetComponent<BoxCollider2D>().enabled = true;
        }

        /// <summary>
        /// Attempt to craft an item from combination of items and return a result
        /// </summary>
        public void CraftFromItems()
        {
            if (itemCombinationToCraft.Count > 0)
            {
                var fi = fieldInventoryController.GetFieldItems();
                var result = recipeController.HasCombination(itemCombinationToCraft, activeCatalyst);
                if (result.Count() > 0)
                {
                    itemCombinationToCraft.ForEach(o =>
                    {
                        var i = fi.FindAll(f => f.GetComponent<ExplorerItemsController>().itemBase == o);
                        i.ForEach(d =>
                        {
                            var ec = d.GetComponent<ExplorerItemsController>();
                            if (ec.total <= 1)
                            {
                                fi.Remove(d);
                                Destroy(d);
                            } else
                            {
                                --ec.total;
                            }
                        });
                    });
                    result.ForEach(o =>
                    {
                        var item = o.InstantiateAsGameObject(itemHolder.transform);
                        fi.Add(item);
                        SetItemForCraftingControls(item);
                        //fieldItems.Add(o.InstantiateAsGameObject(itemHolder.transform));
                    });
                    itemCombinationToCraft = new List<ItemBase>();
                 }
            }
        }

        /// <summary>
        /// Set catalyst for crafting
        /// </summary>
        public void SetCatalyst()
        {
            switch (catalystSlider.value)
            {
                case 0:
                    activeCatalyst = CraftingCatalyst.burn;
                    break;
                case 1:
                    activeCatalyst = CraftingCatalyst.grind;
                    break;
                case 2:
                    activeCatalyst = CraftingCatalyst.boil;
                    break;
                default:
                    break;
            }
            currentCatalystText.text = activeCatalyst.ToString();
        }

        private void Start()
        {
            fieldInventoryController = ExploreManager.inventoryHolder.GetComponent<fieldInventoryController>();
            AddFromFieldItems();
            SetCatalyst();
        }
    }
}
