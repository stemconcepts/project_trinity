using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssemblyCSharp
{
    public class miniMapController : MonoBehaviour
    {
        public GameObject miniMapIconsHolder;
        List<Transform> iconTransforms = new List<Transform>();
        bool dragging;

        // Start is called before the first frame update
        void Start()
        {
            //Invoke("GetIconTranforms", 0.01f);
        }

        void GetIconTranforms()
        {
            iconTransforms = miniMapIconsHolder.transform.GetComponentsInChildren<Transform>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            if (dragging) {
                ExploreManager.gameManager.DragObject(miniMapIconsHolder.transform);
            }
        }

        void OnMouseEnter()
        {
            
        }

        void OnMouseExit()
        {
            
        }


        void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ExploreManager.gameManager.SetDragOrigin();
            }
            dragging = true;
        }

        void OnMouseUp()
        {
            dragging = false;
        }
    }
}