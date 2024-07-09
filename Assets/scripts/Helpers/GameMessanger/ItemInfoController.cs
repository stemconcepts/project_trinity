using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class ItemInfoController : MonoBehaviour
    {
        public TextMeshProUGUI ItemName;
        public Image itemImage;
        public ItemBase itemBase;
        public int amount;
        void Start()
        {
            if (itemBase.GetType() == typeof(baubleItem))
            {
                itemBase.itemIcon = ((baubleItem)itemBase).bauble.itemIcon;
            }
            var amountTxt = amount > 1 ? $"x {amount}" : "";
            ItemName.text = $"{itemBase.itemName} {amountTxt}";
            if (itemBase.itemIcon)
            {
                itemImage.sprite = itemBase.itemIcon;
            }
        }
    }
}