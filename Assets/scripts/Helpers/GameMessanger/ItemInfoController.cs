using UnityEngine;
using UnityEditor;
using TMPro;

namespace AssemblyCSharp
{
    public class ItemInfoController : MonoBehaviour
    {
        public TextMeshProUGUI ItemName;
        public ItemBase itemBase;
        public int amount;
        void Start()
        {
            ItemName.text = $"{itemBase.itemName} x{amount}";
        }
    }
}