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
            MainGameManager.instance.DisableEnableLiveBoxColliders(true);
        }

        /// <summary>
        /// Remove from crafting combination list
        /// </summary>
        /// <param name="item"></param>
        void RemoveFromCraftingCombination(ItemBase item)
        {
            itemCombinationToCraft.Remove(item);
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
            //item.AddComponent<BoxColliderGenerator>();
            item.transform.SetParent(itemHolder.transform);
            var dd = item.GetComponent<DragAndDropController>();
            if (!dd)
            {
                var d = item.AddComponent<DragAndDropController>();
                d.draggable = true;
                d.mouseUpAction += AddToCraftingCombination;
                d.mouseUpActionRemove += RemoveFromCraftingCombination;
                d.dragAndDropMaster = craftingGroupHolder.GetComponent<DragAndDropMasterController>();
            } else
            {
                dd.mouseUpAction += AddToCraftingCombination;
                dd.mouseUpActionRemove += RemoveFromCraftingCombination;
                dd.dragAndDropMaster = craftingGroupHolder.GetComponent<DragAndDropMasterController>();
                dd.enabled = true;
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
                var fieldItems = this.fieldInventoryController.GetFieldItems();
                var result = recipeController.HasCombination(itemCombinationToCraft, activeCatalyst);
                if (result.items.Count() > 0)
                {
                    switch (result.mixMode)
                    {
                        case MixMode.requiresAllOf:
                            itemCombinationToCraft.ForEach(itemBase =>
                            {
                                var fieldItem = fieldItems.FindAll(f => f.GetComponent<ExplorerItemsController>().itemBase == itemBase);
                                fieldItem.ForEach(itemGameObject =>
                                {
                                    var explorerController = itemGameObject.GetComponent<ExplorerItemsController>();
                                    if (explorerController.total <= 1)
                                    {
                                        fieldItems.Remove(itemGameObject);
                                        Destroy(itemGameObject);
                                    }
                                    else
                                    {
                                        --explorerController.total;
                                    }
                                });
                            });
                            break;
                        case MixMode.requiresAnyOf:
                            var fieldItem = fieldItems.FirstOrDefault(f => f.GetComponent<ExplorerItemsController>().itemBase == itemCombinationToCraft.FirstOrDefault());
                            if (fieldItem)
                            {
                                var explorerController = fieldItem.GetComponent<ExplorerItemsController>();
                                if (explorerController.total <= 1)
                                {
                                    fieldItems.Remove(fieldItem);
                                    Destroy(fieldItem);
                                }
                                else
                                {
                                    --explorerController.total;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    result.items.ForEach(o =>
                    {
                        var item = o.InstantiateAsGameObject(itemHolder.transform);
                        fieldItems.Add(item);
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
            fieldInventoryController = MainGameManager.instance.exploreManager.inventoryHolder.GetComponent<fieldInventoryController>();
            AddFromFieldItems();
            SetCatalyst();
        }
    }
}
