using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace AssemblyCSharp
{
    public class ExplorerItemsController : MonoBehaviour
    {
        public ItemBase itemBase;
        public int position;
        public int total;
        public TextMeshPro text;
        public Image image;
        ToolTipTriggerController tooltipController;
        private bool loaded = false;

        /// <summary>
        /// Add set up data for item that was manually placed
        /// </summary>
        public void TriggerSetUp()
        {
            SetUpItem(itemBase, "preload");
        }

        public void SetUpItem(ItemBase itemBase, string prefix)
        {
            this.gameObject.name = itemBase.name;
            itemBase.id = $"resource_{prefix}_{itemBase.itemName}";
            this.itemBase = itemBase;

            tooltipController = gameObject.GetComponent<ToolTipTriggerController>();
            tooltipController.AddtoolTip(itemBase.itemName, itemBase.itemName, itemBase.itemDesc);
            if (image != null)
            {
                image.sprite = itemBase.itemIcon;
            }
            if (text != null)
            {
                text.text = total > 1 ? $"x{total}" : "";
            }
            //Used to check if internal set up is required
            loaded = true;
        }

        public void DestroyToolTips()
        {
            tooltipController.DestroyToolTipDisplay(itemBase.itemName);
        }

        // Use this for initialization
        void Start()
        {
            if (image && MainGameManager.instance.SceneManager.currentScene.ToLower() != "exploration")
            {
                image.DOFade(1f, 0.8f).SetDelay(1f);
            }
            if (!loaded)
            {
                SetUpItem(itemBase, "preload");
            }
        }

        void Awake()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseUp()
        {
           if (MainGameManager.instance.SceneManager.currentScene.ToLower() == "exploration")
           {
                MainGameManager.instance.exploreManager.AddToObtainedItems(itemBase, this.gameObject);
           }
        }
    }
}