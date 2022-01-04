using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Panels_Manager : MonoBehaviour
    {
        public GameObject currentOccupier;
        private RectTransform panelTransform;
        public Animation_Manager animationManager;
        public Character_Manager characterManager;
        public Movement_Manager movementManager;
        public int sortingLayerNumber;
        public int panelNumber;
        //public bool isOccupied;
       // private RectTransform UItransform;
        public bool isVoidZone;
        public bool isVoidCounter;
        public bool isThreatPanel;
        public bool isEnemyPanel;
        public Color threatPanelColor;
        public Color panelColor;
        public Color enemyPanelColor;
        public Color voidZoneColor;
        public Color counterZoneColor;
        public bool friendlyPanel;
        //private bool dragging = false;
        public GameObject positionArrowType;
        //private float distance;
        //private Vector2 currentPosition;
        //private Task holdTimeTask;
        //private GameObject positionArrow;
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

        void Start()
        {
            imageScript = GetComponent<Image>();
            voidZoneColor = new Color(0.9f, 0.1f, 0.1f, 1f);
            panelColor = new Color(1f, 1f, 1f, 1f);
            enemyPanelColor = new Color(0.6f, 0.4f, 0.4f, 1f);
            counterZoneColor = new Color(0.1f, 0.9f, 0.1f, 1f);
            threatPanelColor = new Color(0.9f, 0.9f, 0.1f, 1f);
            imageScript.color = isVoidZone ? voidZoneColor : isThreatPanel ? threatPanelColor : isEnemyPanel ? enemyPanelColor : panelColor;
            Invoke("SetPosition", 0.2f);
        }

        void SetPosition()
        {
            if (currentOccupier /*&& !moved*/)
            {
                SetStartingPanel(currentOccupier);
            }
        }

        public void SetStartingPanel(GameObject currentOccupier, bool forceMove = false)
        {
            this.currentOccupier = currentOccupier;
            animationManager = currentOccupier.GetComponent<Animation_Manager>();
            movementManager = currentOccupier.GetComponent<Movement_Manager>();
            animationManager.SetSortingLayer(sortingLayerNumber);
            movementManager.SetSortingLayer(sortingLayerNumber);
            movementManager.currentPanel = this.gameObject;
            characterManager = currentOccupier.GetComponent<Character_Manager>();
            characterManager.characterModel.inThreatZone = isThreatPanel;
            characterManager.characterModel.inVoidCounter = isVoidCounter;
            characterManager.characterModel.inVoidZone = isVoidZone;
            if (!moved || forceMove)
            {
                panelTransform = GetComponent<RectTransform>();
                if(movementManager.origPosition != new Vector2(panelTransform.position.x, panelTransform.position.y + 6f))
                {
                    currentOccupier.transform.position = movementManager.origPosition = new Vector2(panelTransform.position.x, panelTransform.position.y + 6f);
                    moved = true;
                }
            }
        }

        

        public void VoidZoneMark(){
            //if( type == "All" ){
            //  imageScript.color = new Color(0.9f, 0.1f, 0.1f, 0.8f);
                imageScript.color = voidZoneColor;
                //print ("Void Zone Mark");
                isVoidZone = true;
                if( currentOccupier != null ){
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
            imageScript.color = isThreatPanel ? threatPanelColor : isEnemyPanel ? enemyPanelColor : panelColor;
            isVoidZone = false;
            isVoidCounter = false;
            if(currentOccupier != null){
                currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidZone = false;
                currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidCounter = false;
            }
        }
    
        public void SafePanel(){
        //  imageScript.color = new Color(0.1f,0.9f,0.1f,0.8f);
            imageScript.color = counterZoneColor;
            isVoidZone = false;
            isVoidCounter = true;
            if(currentOccupier != null && currentOccupier.GetComponent<Character_Manager>().characterModel.role.ToString() == "tank" ){
                currentOccupier.GetComponent<Character_Manager>().characterModel.inVoidCounter = true;
            }
        }  
    }
}

 