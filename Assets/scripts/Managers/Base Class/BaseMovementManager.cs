using System;
using UnityEngine;
using Spine.Unity;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using static UnityEngine.Rendering.DebugUI;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public abstract class BaseMovementManager : MonoBehaviour 
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
        public List<AudioClip> movementSounds;

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

        public abstract void CheckPanelPosition();

        private int GetMoveAmount(int movementAmount)
        {
            switch (baseManager.characterManager.tag)
            {
                case "Enemy":
                    return movementAmount * -1;
                case "Player":
                    return movementAmount * 1;
                default:
                    return movementAmount * 1;
            }
        }

        public void SetNewPosition(GenericSkillModel skill)
        {
            var currentPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
            var currentPanelNum = currentPanel.panelNumber;
            int targetPanelNum = currentPanelNum;

            //Reposition Logic
            if (skill.Reposition == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum + GetMoveAmount(skill.RepositionAmount);
            }
            else if (skill.Reposition == GenericSkillModel.moveType.Forward)
            {
                targetPanelNum = currentPanelNum - GetMoveAmount(skill.RepositionAmount);
            }
            targetPanelNum = targetPanelNum > 2 ? 2 : targetPanelNum;
            targetPanelNum = targetPanelNum < 0 ? 0 : targetPanelNum;

            var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum).gameObject;
            var panelManager = targetPanel.GetComponent<PanelsManager>();

            //Set new panel without moving them
            if (panelManager.currentOccupier == null)
            {
                panelManager.SetOrigPositionInPanel(this);
                var currentPanelManager = currentPanel.GetComponent<PanelsManager>();
                currentPanelManager.ClearCurrentPanel();
                panelManager.SetStartingPanel(this.gameObject);
            }
        }

        public void Reposition(GenericSkillModel skill)
        {
            var currentPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
            var currentPanelNum = currentPanel.panelNumber;
            int targetPanelNum = currentPanelNum;

            //Reposition Logic
            if (skill.Reposition == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum + GetMoveAmount(skill.RepositionAmount);
            }
            else if (skill.Reposition == GenericSkillModel.moveType.Forward)
            {
                targetPanelNum = currentPanelNum - GetMoveAmount(skill.RepositionAmount);
            }

            targetPanelNum = targetPanelNum > 2 ? 2 : targetPanelNum;
            targetPanelNum = targetPanelNum < 0 ? 0 : targetPanelNum;

            var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum).gameObject;

            var panelManager = targetPanel.GetComponent<PanelsManager>();
            if (panelManager.currentOccupier == null)
            {
                panelManager.SetOrigPositionInPanel(this);
                if (skill.RepositionAmount > 0 && !skill.movesToTarget)
                {
                    MoveToPanel(panelManager, animationOptionsEnum.hop);
                }
            }
        }

        public void ForceMove(GenericSkillModel skill)
        {
            var currentPanel = baseManager.movementManager.currentPanel.GetComponent<PanelsManager>();
            var currentPanelNum = currentPanel.panelNumber;
            int targetPanelNum = currentPanelNum;
            
            //Forced Move Logic
            if (!baseManager.statusManager.DoesStatusExist(StatusNameEnum.SteadFast) && skill.forcedMove == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + GetMoveAmount(skill.forcedMoveAmount);
                //targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + skill.forcedMoveAmount;
            }
            else if (!baseManager.statusManager.DoesStatusExist(StatusNameEnum.SteadFast) && skill.forcedMove == GenericSkillModel.moveType.Forward)
            {
                targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum - GetMoveAmount(skill.forcedMoveAmount);
                //targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum - skill.forcedMoveAmount;
            }

            targetPanelNum = targetPanelNum > 2 ? 2 : targetPanelNum;
            targetPanelNum = targetPanelNum < 0 ? 0 : targetPanelNum;

            var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum).gameObject;

            var panelManager = targetPanel.GetComponent<PanelsManager>();
            if (!panelManager.currentOccupier)
            {
                panelManager.SetOrigPositionInPanel(this);
                if (skill.forcedMoveAmount > 0)
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
                float xpos = this.tag == "Enemy" ? target.GetComponent<BaseMovementManager>().origPosition.x - (size.x / 1.2f) : target.GetComponent<BaseMovementManager>().origPosition.x + (size.x / 1.2f);
                float ypos = (size.y + offsetYPosition);
                attackedPos.x = xpos;
                attackedPos.y =  target.transform.position.y; //-(size.y / 1.2f);
            }
            return attackedPos;
        }
    
        public void moveToTarget( float movementSpeed, BaseCharacterManagerGroup target ){
            var meshRenderer = target.animationManager.GetMeshRenderers()[0];
            baseManager.animationManager.SetSortingLayer(meshRenderer.sortingOrder);
            //baseManager.animationManager.meshRenderer.sortingOrder = meshRenderer.sortingOrder;
            var targetpos = GetAttackPos( target.gameObject );
            if (dashEffect)
            {
                baseManager.effectsManager.CallEffect(dashEffect, "bottom");
            }
            baseManager.gameObject.transform.DOMove(targetpos, movementSpeed/100).SetEase(Ease.OutQuad);
            MainGameManager.instance.soundManager.playAllSounds(movementSounds, 0.2f);
        }
    
        public void moveToHome(){
            baseManager.animationManager.SetSortingLayer(origSortingOrder);
            //baseManager.animationManager.meshRenderer.sortingOrder = origSortingOrder;
            float speed = (Math.Abs(origPosition.x - currentPosition.x) + Math.Abs(origPosition.y - currentPosition.y)) / 50;
            baseManager.gameObject.transform.DOMove(origPosition, speed).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    currentPanel.GetComponent<PanelsManager>().SaveCharacterPositionFromPanel();
                });
            baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.hopAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, 0);
            MainGameManager.instance.soundManager.playAllSounds(movementSounds, 0.2f);
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
            /*var eventModel = new EventModel
            {
                eventName = EventTriggerEnum.OnMove.ToString(),
                extTarget = baseManager.characterManager,
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eventModel);*/
            baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnMove);
            MainGameManager.instance.soundManager.playAllSounds(movementSounds, 0.2f);
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

