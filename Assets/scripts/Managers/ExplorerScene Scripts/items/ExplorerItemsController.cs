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

        public void SetUpItem(ItemBase itemBase, string prefix)
        {
            this.gameObject.name = itemBase.name;
            itemBase.id = $"resource_{prefix}_{itemBase.itemName}";
            this.itemBase = itemBase;

            tooltipController = gameObject.GetComponent<ToolTipTriggerController>();
            tooltipController.AddtoolTip(itemBase.itemName, itemBase.itemName, itemBase.itemDesc);
            image.sprite = itemBase.itemIcon;
            text.text = total > 1 ? $"x{total}" : "";
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
                ExploreManager.AddToObtainedItems(itemBase, this.gameObject);
           }
        }
    }
}