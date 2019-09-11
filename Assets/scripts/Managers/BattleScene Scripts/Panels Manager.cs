using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Panels_Manager : BasicManager
    {
        public GameObject currentOccupier { get; set; }
        public Animation_Manager animationManager { get; set; }
        public Movement_Manager movementManager { get; set; }
        public int sortingLayerNumber { get; set;}

        public Panels_Manager()
        {
            animationManager = currentOccupier.GetComponent<Animation_Manager>();
            movementManager = currentOccupier.GetComponent<Movement_Manager>();
        }

        public void SetStartingPanel( GameObject currentOccupier ){
            var panelTransform = GetComponent<RectTransform>();
            //panelTransform.position.y = panelTransform.position.y;
            panelTransform.position = panelTransform.position;
            if( currentOccupier ){
                currentOccupier.transform.position = new Vector2(panelTransform.position.x, panelTransform.position.y + 6f);
                animationManager.SetSortingLayer( sortingLayerNumber );
                movementManager.SetSortingLayer( sortingLayerNumber );
            }
        }
    }
}

 