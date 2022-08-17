using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Helpers.Utility
{
    public class DragAndDropMasterController : MonoBehaviour
    {
        public GameObject draggedItem;

        /// <summary>
        /// Set current held item as the dragged item
        /// </summary>
        /// <param name="item"></param>
        public void SetDraggedItem(GameObject item)
        {
            draggedItem = item;
        }

        public void ResetDraggedItem()
        {
            draggedItem = null;
        }
    }
}
