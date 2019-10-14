using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Panels_Manager : MonoBehaviour
    {
        public GameObject currentOccupier;
        public Animation_Manager animationManager;
        public Movement_Manager movementManager;
        public int sortingLayerNumber;
        public bool isOccupied;
        private RectTransform UItransform;
        public bool isVoidZone;
        public bool isVoidCounter;
        public bool isThreatPanel;
        public Color panelColor;
        public Color voidZoneColor;
        public Color counterZoneColor;
        public bool friendlyPanel;
        private bool dragging = false;
        public GameObject positionArrowType;
        private float distance;
        private Vector2 currentPosition;
        private Task holdTimeTask;
        private GameObject positionArrow;
        public Image imageScript;
        public voidZoneType voidZonesTypes;
        public enum voidZoneType{
            HorizontalA,
            HorizontalB,
            HorizontalC,
            VerticalA,
            VerticalB,
            VerticalC
        };
        public bool moved = false;

        void OnRenderObject()
        {
            if( !moved && currentOccupier ){
               SetStartingPanel(currentOccupier);
               moved = true;
            }
        }

        public void SetStartingPanel( GameObject currentOccupier ){
            var panelTransform = GetComponent<RectTransform>();
            panelTransform.position = panelTransform.position;
                animationManager = currentOccupier.GetComponent<Animation_Manager>();
                movementManager = currentOccupier.GetComponent<Movement_Manager>();
                currentOccupier.transform.position = movementManager.origPosition = new Vector2(panelTransform.position.x, panelTransform.position.y + 6f);
                animationManager.SetSortingLayer( sortingLayerNumber );
                movementManager.SetSortingLayer( sortingLayerNumber );
        }

        public void VoidZoneMark(){
            //if( type == "All" ){
            //  imageScript.color = new Color(0.9f, 0.1f, 0.1f, 0.8f);
                imageScript.color = voidZoneColor;
                //print ("Void Zone Mark");
                isVoidZone = true;
                if( isOccupied ){
                    currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidZone = true;
                }
            /*} else if( type == "Random" ) {
                imageScript.color = new Color(0.9f, 0.1f, 0.1f, 0.8f);
                isVoidZone = true;
                if( isOccupied ){
                    currentOccupier.GetComponent<character_data>().inVoidZone = true;
                }
            } */
        }
    
        public void ClearVoidZone(){
            //imageScript.color = new Color(1f,1f,1f,1f);
            imageScript.color = panelColor;
            isVoidZone = false;
            isVoidCounter = false;
            if( isOccupied ){
                currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidZone = false;
                currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidCounter = false;
            }
        }
    
        public void SafePanel(){
        //  imageScript.color = new Color(0.1f,0.9f,0.1f,0.8f);
            imageScript.color = counterZoneColor;
            isVoidZone = false;
            isVoidCounter = true;
            if( isOccupied && currentOccupier.GetComponent<Character_Manager>().characterModel.role == "tank" ){
                currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidCounter = true;
            }
        }  
    }
}

 