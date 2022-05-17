using UnityEngine;
using UnityEditor;
using TMPro;

namespace AssemblyCSharp
{
    public class ItemInfoController : MonoBehaviour
    {
        public TextMeshProUGUI ItemName;
        public ItemBase itemBase;

        void Start()
        {
            ItemName.text = itemBase.name;
        }
    }
}