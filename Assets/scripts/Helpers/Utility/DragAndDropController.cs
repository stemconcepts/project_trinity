using AssemblyCSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Helpers.Utility
{
    public class DragAndDropController : MonoBehaviour
    {
        public DragAndDropMasterController dragAndDropMaster;
        Transform origParent;
        Transform newParent;
        BoxCollider2D boxCollider;
        public bool draggable;
        bool dragging;
        public delegate void OnMouseUpAction(ItemBase item);
        public OnMouseUpAction mouseUpAction;

        public void OnMouseDown()
        {
            if (draggable && dragAndDropMaster)
            {
                new Task(DraggingTimer(0.1f));
            }
        }

        public void OnMouseUp()
        {
            if (draggable && dragAndDropMaster)
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                if (newParent)
                {
                    mouseUpAction?.Invoke(gameObject.GetComponent<ExplorerItemsController>().itemBase);
                }
                SetOrigParent(newParent);
                dragAndDropMaster.ResetDraggedItem();
                dragging = false;
            }
        }

        public void OnMouseEnter()
        {
            if (!draggable && dragAndDropMaster.draggedItem)
            {
                dragAndDropMaster.draggedItem.GetComponent<DragAndDropController>().SetNewParent(this.gameObject.transform);
            }
        }

        public void OnMouseExit()
        {
            if (!draggable && dragAndDropMaster.draggedItem)
            {
                dragAndDropMaster.draggedItem.GetComponent<DragAndDropController>().SetNewParent();
            }
        }

        IEnumerator DraggingTimer(float waitTime)
        {
            while (!dragging)
            {
                yield return new WaitForSeconds(waitTime);
                dragging = true;
                boxCollider.enabled = false;
                transform.SetParent(dragAndDropMaster.gameObject.transform, false);
                dragAndDropMaster.SetDraggedItem(this.gameObject);
            }
        }

        void SetNewParent(Transform parent = null)
        {
            newParent = parent;
        }

        void SetOrigParent(Transform parent)
        {
            if (parent)
            {
                this.transform.SetParent(parent);
                newParent = null;
            } else
            {
                this.transform.SetParent(origParent);
            }
        }

        /// <summary>
        /// Sets item dragging rules
        /// </summary>
        void SetDragging()
        {
            if (dragging && draggable)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector2 rayPoint = ray.origin;
                transform.position = rayPoint;
            }
        }

        private void Start()
        {
            boxCollider = gameObject.GetComponent<BoxCollider2D>();
            origParent = gameObject.transform.parent;
        }

        void Update()
        {
            SetDragging();
        }
    }
}
