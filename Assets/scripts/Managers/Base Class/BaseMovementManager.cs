using System;
using UnityEngine;
using Spine.Unity;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace AssemblyCSharp
{
    public class BaseMovementManager : MonoBehaviour 
    {
        public BaseCharacterManagerGroup baseManager;
        public int origSortingOrder;
        public float movementSpeed;
        public float offsetYPosition;
        public float movementCost = 1;
        public Vector2 origPosition;
        public Vector2 currentPosition;
        public GameObject dashEffect;
        public GameObject currentPanel;
        public GameObject positionArrow;
        public bool isInBackRow;
        public bool isInFrontRow;
        public bool isInMiddleRow;

        void Awake() 
        {
            movementCost = 1f;
            movementSpeed = 50f;
        }

        void Update(){
            if (currentPanel)
            {
                CheckPanelPosition();
            }
        }

        public void CheckPanelPosition()
        {
            if (currentPosition != (Vector2)this.gameObject.transform.position)
            {
                currentPosition = this.gameObject.transform.position;
            }
            var panel = currentPanel.GetComponent<PanelsManager>();
            switch (panel.panelNumber)
            {
                case 0:
                    if (!isInFrontRow)
                    {
                        var eventModelFront = new EventModel
                        {
                            eventName = "OnFirstRow",
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelFront);
                    }
                    isInBackRow = false;
                    isInMiddleRow = false;
                    isInFrontRow = true;
                    break;
                case 1:
                    if (!isInMiddleRow)
                    {
                        var eventModelMiddle = new EventModel
                        {
                            eventName = "OnMiddleRow",
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelMiddle);
                    }
                    isInBackRow = false;
                    isInMiddleRow = true;
                    isInFrontRow = false;
                    break;
                case 2:
                    if (!isInBackRow)
                    {
                        var eventModelBack = new EventModel
                        {
                            eventName = "OnLastRow",
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                        };
                        BattleManager.eventManager.BuildEvent(eventModelBack);
                    }
                    isInBackRow = true;
                    isInMiddleRow = false;
                    isInFrontRow = false;
                    break;
                default:
                    isInBackRow = false;
                    isInMiddleRow = false;
                    isInFrontRow = false;
                    break;
            }
        }

        public void ForceMoveOrReposition(GenericSkillModel skill)
        {
            var currentPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
            var currentPanelNum = currentPanel.panelNumber;
            int targetPanelNum = currentPanelNum;
            
            if (!baseManager.statusManager.DoesStatusExist("steadFast") && skill.forcedMove == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + skill.forcedMoveAmount;
            }
            else if (!baseManager.statusManager.DoesStatusExist("steadFast") && skill.forcedMove == GenericSkillModel.moveType.Forward)
            {
                targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum - skill.forcedMoveAmount;
            }

            if (skill.Reposition == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + skill.RepositionAmount;
            }
            else if (skill.Reposition == GenericSkillModel.moveType.Forward)
            {
                targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum - skill.RepositionAmount;
            }
            targetPanelNum = targetPanelNum > 2 ? 2 : targetPanelNum;
            targetPanelNum = targetPanelNum < 0 ? 0 : targetPanelNum;

            var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum).gameObject;
            var panelManager = targetPanel.GetComponent<PanelsManager>();
            if (!panelManager.currentOccupier)
            {
                panelManager.SetOrigPositionInPanel(this);
                if (skill.RepositionAmount > 0 && !skill.movesToTarget)
                {
                    MoveToPanel(panelManager, animationOptionsEnum.hop);
                }
                else if (skill.forcedMoveAmount > 0)
                {
                    MoveToPanel(panelManager, animationOptionsEnum.hit);
                }
            }
        }

        public void SetSortingLayer(int sortingLayer ){
            origSortingOrder = sortingLayer;
        }

        public Vector2 GetAttackPos( GameObject target ){
            Vector2 attackedPos = new Vector2();
            if( target != null ){
                Vector3 size = baseManager.animationManager.GetSpriteBounds().size;
                float xpos = this.tag == "Enemy" ? target.GetComponent<BaseMovementManager>().origPosition.x - (size.x / 1.5f) : target.GetComponent<BaseMovementManager>().origPosition.x + (size.x / 1.5f);
                float ypos = (size.y + offsetYPosition);
                attackedPos.x = xpos;
                attackedPos.y = target.transform.position.y;//target.GetComponent<BaseMovementManager>().currentPanel.transform.position.y /*+ (ypos / 2.2f)*/;
            }
            return attackedPos;
        }
    
        public void moveToTarget( float movementSpeed, BaseCharacterManagerGroup target ){
            baseManager.animationManager.meshRenderer.sortingOrder = target.animationManager.meshRenderer.sortingOrder;
            var targetpos = GetAttackPos( target.gameObject );
            if (dashEffect)
            {
                baseManager.effectsManager.CallEffect(dashEffect, "bottom");
            }
            //BattleManager.taskManager.MoveForwardTask( baseManager, movementSpeed, targetpos, dashEffect );

            baseManager.gameObject.transform.DOMove(targetpos, movementSpeed/100).SetEase(Ease.OutQuad);
        }
    
        public void moveToHome(){
            baseManager.animationManager.meshRenderer.sortingOrder = origSortingOrder;
            float speed = (Math.Abs(origPosition.x - currentPosition.x) + Math.Abs(origPosition.y - currentPosition.y)) / 50;
            baseManager.gameObject.transform.DOMove(origPosition, speed).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    currentPanel.GetComponent<PanelsManager>().SaveCharacterPositionFromPanel();
                });
            baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.hopAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, 0);
        }

        public void MoveToPanel(PanelsManager panelManager, animationOptionsEnum hopAnimation = animationOptionsEnum.none)
        {
            var currentPanelManager = currentPanel.GetComponent<PanelsManager>();
            currentPanelManager.ClearCurrentPanel();
            panelManager.SetStartingPanel(this.gameObject);
            hopAnimation = hopAnimation == animationOptionsEnum.none ? baseManager.animationManager.hopAnimation : hopAnimation;
            float speed = (Math.Abs(origPosition.x - currentPosition.x) + Math.Abs(origPosition.y - currentPosition.y)) / 50;
            baseManager.gameObject.transform.DOMove(origPosition, speed).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    panelManager.SaveCharacterPositionFromPanel(false);
                });
            baseManager.animationManager.PlaySetAnimation(hopAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, 0);
            var eventModel = new EventModel
            {
                eventName = "OnMove",
                extTarget = baseManager.characterManager,
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eventModel);
        }

        public void OnEventMove(Spine.TrackEntry state, Spine.Event e ){
            if( e.Data.Name == "movementStart" ){
                var target = baseManager.autoAttackManager.isAttacking ? baseManager.autoAttackManager.autoAttackTarget : baseManager.skillManager.currenttarget.baseManager;
                moveToTarget( movementSpeed, target );
            } else if( e.Data.Name == "movementBack" ){
                baseManager.autoAttackManager.isAttacking = false;
                if( origPosition != (Vector2)this.gameObject.transform.position ){
                    moveToHome();
                }
            } 
        }
    }
}

