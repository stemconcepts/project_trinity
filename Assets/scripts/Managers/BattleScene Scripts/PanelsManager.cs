using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Unity.IO.Archive;
using System.Collections.Generic;

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
        public float fadeAmount = 0.8f;
        public Color threatPanelColor;
        public Color panelColor;
        public Color enemyPanelColor;
        public Color voidZoneColor;
        public Color counterZoneColor;
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
        public GameObject SelectedEffect;
        public GameObject CastingEffect;

        bool IsBackPanel()
        {
            return (isEnemyPanel && panelNumber == 0) || (!isEnemyPanel && panelNumber == 2);
        }

        bool IsFrontPanel()
        {
            return (isEnemyPanel && panelNumber == 2) || (!isEnemyPanel && panelNumber == 0);
        }

        bool IsMiddlePanel()
        {
            return panelNumber == 1;
        }

        void SetPosition()
        {
            if (currentOccupier)
            {
                SetStartingPanel(currentOccupier);
                SaveCharacterPositionFromPanel();
            }
        }

        public void SetFade(float amount, float duration)
        {
            imageScript.DOFade(amount, duration);
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

             //Assign or remove positional buffs
             AssignPanelPositionBuff();
        }

        /// <summary>
        /// Set transform and position reference (origPosition) to character occupying space
        /// Use bool parameter to change only the transform, used for manually moving characters were origPosition is changed externally
        /// </summary>
        /// <param name="changeOrigPosition"></param>
        public void SaveCharacterPositionFromPanel(bool changeOrigPosition = true)
        {
            float spriteSize = movementManager.offsetYPosition > 0f ? 
                movementManager.baseManager.animationManager.GetSpriteBounds().size.y : movementManager.baseManager.animationManager.GetSpriteBounds().size.y + movementManager.offsetYPosition;

            if (movementManager.origPosition != new Vector2(panelTransform.position.x, panelTransform.position.y + movementManager.offsetYPosition))
            {
                var newPos = new Vector2(panelTransform.position.x, panelTransform.position.y + movementManager.offsetYPosition);
                currentOccupier.transform.position = newPos;
                if (changeOrigPosition)
                {
                    movementManager.origPosition = newPos;
                }
            }
        }

        public void SetOrigPositionInPanel(BaseMovementManager movementManager)
        {
            float spriteSize = movementManager.offsetYPosition > 0f ?
                movementManager.baseManager.animationManager.GetSpriteBounds().size.y : movementManager.baseManager.animationManager.GetSpriteBounds().size.y + movementManager.offsetYPosition;

            var newPos = new Vector2(panelTransform.position.x, panelTransform.position.y + movementManager.offsetYPosition);
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

        private void AssignPanelPositionBuff()
        {
            if (currentOccupier)
            {
                var benefits = characterManager.characterModel.PositionalBenefits;
                foreach (var benefit in benefits)
                {
                    benefit.statusModel.dispellable = false;
                    var statusModel = new StatusModel
                    {
                        singleStatus = benefit.statusModel,
                        power = benefit.statusPower,
                        turnDuration = 0,
                        isFlat = benefit.statusModel.isFlat,
                        baseManager = characterManager.baseManager
                    };
                    switch (benefit.positionBenefit)
                    {
                        case PositionBenefit.Back:
                            if (IsBackPanel())
                            {
                                characterManager.baseManager.statusManager.RunStatusFunction(statusModel);
                            }
                            else
                            {
                                if (characterManager.baseManager.statusManager.GetStatusIfExist(benefit.statusModel.statusName))
                                {
                                    RemovePanelPositionBuff(statusModel);
                                }
                            }
                            break;
                        case PositionBenefit.Middle:
                            if (IsMiddlePanel())
                            {
                                characterManager.baseManager.statusManager.RunStatusFunction(statusModel);
                            }
                            else
                            {
                                if (characterManager.baseManager.statusManager.GetStatusIfExist(benefit.statusModel.statusName))
                                {
                                    RemovePanelPositionBuff(statusModel);
                                }
                            }
                            break;
                        case PositionBenefit.Front:
                            if (IsFrontPanel())
                            {
                                characterManager.baseManager.statusManager.RunStatusFunction(statusModel);
                            }
                            else
                            {
                                if (characterManager.baseManager.statusManager.GetStatusIfExist(benefit.statusModel.statusName))
                                {
                                    RemovePanelPositionBuff(statusModel);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void RemovePanelPositionBuff(StatusModel statusModel)
        {
            if (currentOccupier != null)
            {
                //var status = characterManager.baseManager.statusManager.GetStatusIfExist(statusModel.singleStatus.name);
                //status.statusModel.turnOff = true;
                statusModel.turnOff = true;
                characterManager.baseManager.statusManager.RunStatusFunction(statusModel);
            }
        }

        public List<PanelsManager> GetNeighbours()
        {

            List<voidZoneType> verticalNeighbours = new List<voidZoneType>()
            {
                voidZonesTypes
            };

            List<int> horizontalNeighbours = new List<int>()
            {
                panelNumber
            };

            switch (voidZonesTypes)
            {
                case voidZoneType.VerticalA:
                case voidZoneType.VerticalC:
                    verticalNeighbours.Add(voidZoneType.VerticalB);
                    break;
                case voidZoneType.VerticalB:
                    verticalNeighbours.Add(voidZoneType.VerticalA);
                    verticalNeighbours.Add(voidZoneType.VerticalC);
                    break;
                default:
                    break;
            }

            switch (panelNumber)
            {
                case 0:
                case 2:
                    horizontalNeighbours.Add(1);
                    break;
                case 1:
                    horizontalNeighbours.Add(0);
                    horizontalNeighbours.Add(2);
                    break;
                default:
                    break;
            }

            var result = BattleManager.allPanelManagers
                    .Where(panelManager => horizontalNeighbours.Contains(panelManager.panelNumber) && verticalNeighbours.Contains(panelManager.voidZonesTypes) && panelManager != this).ToList();

            return result;
        }

        public void ShowSelected(bool show)
        {
            SelectedEffect?.SetActive(show);
        }

        public void ShowCasting(bool show)
        {
            CastingEffect?.SetActive(show);
        }

        public void OnMouseEnter()
        {
            //SetFade(0.8f, 0.5f);
        }

        public void OnMouseExit()
        {
            if (!isThreatPanel) {
                //SetFade(fadeAmount, 0.5f);
            }
        }

        private void Update()
        {
            if (!currentOccupier && SelectedEffect.activeSelf)
            {
                ShowSelected(false);
                ShowCasting(false);
            }
        }

        void Start()
        {
            imageScript = GetComponent<Image>();
            voidZoneColor = new Color(0.9f, 0.1f, 0.1f, 1f);
            panelColor = new Color(1f, 1f, 1f, fadeAmount);
            enemyPanelColor = new Color(0.6f, 0.4f, 0.4f, fadeAmount);
            counterZoneColor = new Color(0.1f, 0.9f, 0.1f, 1f);
            threatPanelColor = new Color(0.9f, 0.9f, 0.1f, 0.8f);
            imageScript.color = isVoidZone ? voidZoneColor : isThreatPanel ? threatPanelColor : isEnemyPanel ? enemyPanelColor : panelColor;
            panelTransform = GetComponent<RectTransform>();
            Invoke("SetPosition", 0.5f);
        }
    }
}

 