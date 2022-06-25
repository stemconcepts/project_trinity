using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class PanelsManager : MonoBehaviour
    {
        public GameObject currentOccupier;
        private RectTransform panelTransform;
        public Animation_Manager animationManager;
        public BaseCharacterManager characterManager;
        public BaseMovementManager movementManager;
        [Range(0,2)]
        public int sortingLayerNumber;
        [Range(0, 2)]
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
            //HorizontalA,
            //HorizontalB,
            //HorizontalC,
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
            panelTransform = GetComponent<RectTransform>();
            Invoke("SetPosition", 0.5f);
        }

        void SetPosition()
        {
            if (currentOccupier)
            {
                SetStartingPanel(currentOccupier);
                SaveCharacterPositionFromPanel();
            }
        }

        public void ClearCurrentPanel()
        {
            currentOccupier = null;
            animationManager = null;
            characterManager = null;
            movementManager = null;
        }

        public void SetStartingPanel(GameObject currentOccupier)
        {
            this.currentOccupier = currentOccupier;
            animationManager = currentOccupier.GetComponent<Animation_Manager>();
            movementManager = currentOccupier.tag == "Enemy" ? (BaseMovementManager)currentOccupier.GetComponent<EnemyMovementManager>() : (BaseMovementManager)currentOccupier.GetComponent<PlayerMovementManager>();
            animationManager.SetSortingLayer(sortingLayerNumber);
            movementManager.SetSortingLayer(sortingLayerNumber);
            movementManager.currentPanel = this.gameObject;
            characterManager = currentOccupier.tag == "Enemy" ? (BaseCharacterManager)currentOccupier.GetComponent<EnemyCharacterManager>() : (BaseCharacterManager)currentOccupier.GetComponent<CharacterManager>();
            if (characterManager.characterModel.characterType == BaseCharacterModel.CharacterTypeEnum.Player)
            {
                (characterManager.characterModel as CharacterModel).inThreatZone = isThreatPanel;
                (characterManager.characterModel as CharacterModel).inVoidCounter = isVoidCounter;
                characterManager.characterModel.inVoidZone = isVoidZone;
            }
        }

        /// <summary>
        /// Set transform and position reference (origPosition) to character occupying space
        /// Use bool parameter to change only the transform, used for manually moving characters were origPosition is changed externally
        /// </summary>
        /// <param name="changeOrigPosition"></param>
        public void SaveCharacterPositionFromPanel(bool changeOrigPosition = true)
        {
            float spriteSize = movementManager.baseManager.animationManager.GetSpriteBounds().size.y + movementManager.offsetYPosition;
            if (movementManager.origPosition != new Vector2(panelTransform.position.x, panelTransform.position.y + (spriteSize / 2.2f)))
            {
                var newPos = new Vector2(panelTransform.position.x, panelTransform.position.y + (spriteSize / 2.2f));
                currentOccupier.transform.position = newPos;
                if (changeOrigPosition)
                {
                    movementManager.origPosition = newPos;
                }
            }
        }

        public void SetOrigPositionInPanel(BaseMovementManager movementManager)
        {
            float spriteSize = movementManager.baseManager.animationManager.GetSpriteBounds().size.y + movementManager.offsetYPosition;
            var newPos = new Vector2(panelTransform.position.x, panelTransform.position.y + (spriteSize / 2.2f));
            movementManager.origPosition = newPos;
        }

        public void VoidZoneMark(){
                imageScript.color = voidZoneColor;
                isVoidZone = true;
                if( currentOccupier != null ){
                    currentOccupier.GetComponent<CharacterManager>().characterModel.inVoidZone = true;
                }
        }
    
        public void ClearVoidZone(){
            imageScript.color = isThreatPanel ? threatPanelColor : isEnemyPanel ? enemyPanelColor : panelColor;
            isVoidZone = false;
            isVoidCounter = false;
            if(currentOccupier != null){
                currentOccupier.GetComponent<CharacterManager>().characterModel.inVoidZone = false;
                (currentOccupier.GetComponent<CharacterManager>().characterModel as CharacterModel).inVoidCounter = false;
            }
        }
    
        public void SafePanel(){
            imageScript.color = counterZoneColor;
            isVoidZone = false;
            isVoidCounter = true;
            if(currentOccupier != null && currentOccupier.GetComponent<CharacterManager>().characterModel.role.ToString() == "tank" ){
                (currentOccupier.GetComponent<CharacterManager>().characterModel as CharacterModel).inVoidCounter = true;
            }
        }  
    }
}

 