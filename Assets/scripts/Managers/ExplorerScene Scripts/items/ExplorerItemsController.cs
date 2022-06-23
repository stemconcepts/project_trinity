using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;

namespace AssemblyCSharp
{
    public class ExplorerItemsController : MonoBehaviour
    {
        public ItemBase itemBase;
        public int position;
        public int total;
        public TextMeshPro text;
        public SpriteRenderer spriteRenderer;
        ToolTipTriggerController tooltipController;

        public void SetUpItem()
        {
            tooltipController = gameObject.GetComponent<ToolTipTriggerController>();
            tooltipController.AddtoolTip(itemBase.itemName, itemBase.itemName, itemBase.itemDesc);
            spriteRenderer.sprite = itemBase.itemIcon;
            text.text = total > 1 ? $"x{total.ToString()}" : "";
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
            if (!ExploreManager.obtainedItems.Any(o => o == itemBase))
            {
                ExploreManager.AddToObtainedItems(itemBase, this.gameObject);
                //this.gameObject.transform.localScale = new Vector3(15f, 15f, 1f);
                //this.gameObject.transform.SetParent(ExploreManager.inventoryHolder.transform);
            }
        }
    }
}