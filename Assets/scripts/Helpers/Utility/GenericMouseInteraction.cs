using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static AssemblyCSharp.itemBehaviour;
//using static UnityEditor.Progress;

namespace Assets.scripts.Helpers.Utility
{
    public class GenericMouseInteraction : MonoBehaviour 
    {

        /// <summary>
        /// Return the object that was right clicked
        /// </summary>
        /// <returns></returns>
        public GameObject CheckRightClick()
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (Input.GetMouseButtonDown(1))
            {
                if (hit.collider != null)
                {
                    // An object has been right-clicked
                    return hit.collider.gameObject;
                }
            }
            return null;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
               
            }
        }
    }
}
