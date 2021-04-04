using System;
using UnityEngine;
using Spine.Unity;
using System.Collections;

namespace AssemblyCSharp
{
    public class Movement_Manager : MonoBehaviour 
    {
        public Base_Character_Manager baseManager;
        public int origSortingOrder;
        public float movementSpeed;
        private float moveToHomeSpeed;
        public float movementCost = 1;
        public Vector2 origPosition;
        public Vector2 currentPosition;
        public GameObject posMarkerMin;
        public GameObject posMarker;
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
            moveToHomeSpeed = 80f;
        }

        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if( baseManager.animationManager.skeletonAnimation ){
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventMove;
            }
        }

        void OnRenderObject()
        {
            if( currentPosition != (Vector2)this.gameObject.transform.position){
                currentPosition = this.gameObject.transform.position;
            }
        }

        void Update(){
            CheckPanelPosition();
        }

        public void CheckPanelPosition()
        {
            var panel = currentPanel.GetComponent<Panels_Manager>();
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
                        Battle_Manager.eventManager.BuildEvent(eventModelFront);
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
                        Battle_Manager.eventManager.BuildEvent(eventModelMiddle);
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
                        Battle_Manager.eventManager.BuildEvent(eventModelBack);
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

        public void ForceMoveOrReposition(/*GenericSkillModel.moveType forcedMoveType, int moveAmount = 1, bool reposition = false,*/ GenericSkillModel skill)
        {
            var currentPanel = baseManager.movementManager.currentPanel;
            var currentPanelNum = currentPanel.GetComponent<Panels_Manager>().panelNumber;
            int targetPanelNum = currentPanelNum;
            
            if (skill.forcedMove == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + skill.forcedMoveAmount;
            }
            else if (skill.forcedMove == GenericSkillModel.moveType.Forward)
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
            currentPanel.GetComponent<Panels_Manager>().currentOccupier = null;
            currentPanel.GetComponent<Panels_Manager>().animationManager = null;
            currentPanel.GetComponent<Panels_Manager>().characterManager = null;
            currentPanel.GetComponent<Panels_Manager>().movementManager = null;

            Vector2 panelPos = targetPanel.transform.position;
            panelPos.y = panelPos.y + 6f;
            origPosition = panelPos;

            if (skill.RepositionAmount > 0 && !skill.movesToTarget)
            {
                MoveToPanel(targetPanel, "hop");
                targetPanel.GetComponent<Panels_Manager>().SetStartingPanel(this.gameObject, true);
            } else if(skill.forcedMoveAmount > 0)
            {
                MoveToPanel(targetPanel, "hit");
                targetPanel.GetComponent<Panels_Manager>().SetStartingPanel(this.gameObject, true);
            }
            targetPanel.GetComponent<Panels_Manager>().SetStartingPanel(this.gameObject, false);
        }

        /*public void Reposition(GenericSkillModel.moveType forcedMoveType, int moveAmount = 1)
        {
            var currentPanel = baseManager.movementManager.currentPanel;
            var currentPanelNum = currentPanel.GetComponent<Panels_Manager>().panelNumber;
            int targetPanelNum = currentPanelNum;
            if (forcedMoveType == GenericSkillModel.moveType.Back)
            {
                targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + moveAmount;
            }
            else if (forcedMoveType == GenericSkillModel.moveType.Forward)
            {
                targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum - moveAmount;
            }
            targetPanelNum = targetPanelNum > 2 ? 2 : targetPanelNum;
            targetPanelNum = targetPanelNum < 0 ? 0 : targetPanelNum;
            var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum).gameObject;
            currentPanel.GetComponent<Panels_Manager>().currentOccupier = null;
            currentPanel.GetComponent<Panels_Manager>().animationManager = null;
            currentPanel.GetComponent<Panels_Manager>().characterManager = null;
            currentPanel.GetComponent<Panels_Manager>().movementManager = null;
            //MoveToPanel(targetPanel, "hit");
            targetPanel.GetComponent<Panels_Manager>().SetStartingPanel(this.gameObject, true);
        }*/

        public void SetSortingLayer(int sortingLayer ){
            origSortingOrder = sortingLayer;
        }

        public Vector2 GetAttackPos( GameObject target ){
            Vector2 attackedPos = new Vector2();
            if( target != null ){
                attackedPos.x = baseManager.characterManager.characterModel.role == Character_Model.RoleEnum.minion ? target.GetComponent<Movement_Manager>().posMarkerMin.transform.position.x : target.GetComponent<Movement_Manager>().posMarker.transform.position.x;
                attackedPos.y = baseManager.characterManager.characterModel.role == Character_Model.RoleEnum.minion ? target.GetComponent<Movement_Manager>().posMarkerMin.transform.position.y : target.GetComponent<Movement_Manager>().posMarker.transform.position.y;
            }
            return attackedPos;
        }
    
        public void moveToTarget( float movementSpeed, Base_Character_Manager target ){
            baseManager.animationManager.meshRenderer.sortingOrder = target.animationManager.meshRenderer.sortingOrder;
            var targetpos = GetAttackPos( target.gameObject );
            Battle_Manager.taskManager.MoveForwardTask( baseManager, movementSpeed, targetpos, dashEffect );
        }
    
        public void moveToHome(){
            baseManager.animationManager.meshRenderer.sortingOrder = origSortingOrder;
            Battle_Manager.taskManager.moveBackTask( baseManager, moveToHomeSpeed, origPosition, currentPosition );
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.hopAnimation, false);
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0);
        }

        public void MoveToPanel(GameObject targetPanel, string hopAnimation = "")
        {
            hopAnimation = string.IsNullOrEmpty(hopAnimation) ? baseManager.animationManager.hopAnimation : hopAnimation;
            Vector2 panelPos = targetPanel.transform.position;
            panelPos.y = panelPos.y + 6f;
            origPosition = panelPos;
            //Battle_Manager.taskManager.StartMoveTask(baseManager, 0.009f, panelPos);
            //Battle_Manager.taskManager.MoveForwardTask(baseManager, movementSpeed, panelPos, baseManager.effectsManager.stompEffect);
            Battle_Manager.taskManager.moveBackTask(baseManager, moveToHomeSpeed, origPosition, currentPosition);
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, hopAnimation, false);
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0);

            var eventModel = new EventModel
            {
                eventName = "OnMove",
                extTarget = baseManager.characterManager,
                eventCaller = baseManager.characterManager
            };
            Battle_Manager.eventManager.BuildEvent(eventModel);
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

        //Spawn Move Pointer
        public void OnMouseDown()
        {
            if (!baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking && !baseManager.skillManager.isSkillactive)
            {
                Battle_Manager.HitBoxControl(false);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var distance = Vector2.Distance(transform.position, Camera.main.transform.position);
                Battle_Manager.taskManager.CallTask(0.1f, () =>
                {
                    positionArrow = (GameObject)Instantiate(Battle_Manager.battleDetailsManager.movementArrowObject, ray.GetPoint(distance), Quaternion.identity);
                    var movementArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                    movementArrowManager.originalPanel = baseManager.movementManager.currentPanel.GetComponent<Panels_Manager>();
                    movementArrowManager.distance = distance;
                    movementArrowManager.occupier = baseManager;
                }, "draggingTask_"+baseManager.name);
            }
        }

        public void OnMouseUp()
        {
            if (Battle_Manager.taskManager.taskList.ContainsKey("draggingTask_"+baseManager.name))
            {
                Battle_Manager.taskManager.taskList["draggingTask_" + baseManager.name].Stop();
                Battle_Manager.taskManager.taskList.Remove("draggingTask_" + baseManager.name);
            }
            if (positionArrow && !baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking && !baseManager.skillManager.isSkillactive)
            {
                var positionArrowManager = positionArrow.GetComponent<MovementArrowManager>();
                if (positionArrowManager.hoveredPanel && !positionArrowManager.hoveredPanel.currentOccupier && Battle_Manager.actionPoints >= 1)
                {
                    Battle_Manager.actionPoints -= movementCost;
                    ++((Player_Skill_Manager)baseManager.skillManager).turnsTaken;
                    Battle_Manager.UpdateAPAmount();
                    positionArrowManager.SetPanelandDestroy();
                    MoveToPanel(positionArrowManager.hoveredPanel.gameObject);
                    positionArrowManager.occupier.animationManager.meshRenderer.sortingOrder = origSortingOrder = positionArrowManager.hoveredPanel.sortingLayerNumber;
                    positionArrowManager.occupier.characterManager.characterModel.rowNumber = positionArrowManager.hoveredPanel.sortingLayerNumber;
                }
            }
            Destroy(positionArrow);
            Battle_Manager.HitBoxControl(true);
        }
    }
}

