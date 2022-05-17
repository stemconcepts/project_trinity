using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class ExplorerItemsController : MonoBehaviour
    {
        public ItemBase itemBase;
        public int position;
        ToolTipTriggerController tooltipController;
        SpriteRenderer spriteRenderer;

        public void SetUpItem()
        {
            tooltipController = gameObject.GetComponent<ToolTipTriggerController>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            tooltipController.toolTipName = itemBase.itemName;
            tooltipController.toolTipDesc = itemBase.itemDesc;
            spriteRenderer.sprite = itemBase.itemIcon;
        }

        // Use this for initialization
        void Start()
        {

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
            ExploreManager.AddToObtainedItems(itemBase);
            this.gameObject.transform.localScale = new Vector3(15f, 15f, 1f);
            this.gameObject.transform.SetParent(ExploreManager.inventoryHolder.transform);
        }
    }
}