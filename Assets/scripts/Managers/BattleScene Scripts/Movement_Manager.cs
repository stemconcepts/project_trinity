using System;
using UnityEngine;
using Spine.Unity;
using System.Collections;

namespace AssemblyCSharp
{
    public class Movement_Manager : MonoBehaviour 
    {
        public Base_Character_Manager baseManager;
        //public string idleAnim;
        //public string hopAnim;
        public int origSortingOrder;
        public float movementSpeed;
        private float moveToHomeSpeed;
        public Vector2 origPosition ;
        public Vector2 currentPosition ;
        public GameObject posMarkerMin;
        public GameObject posMarker;
        public GameObject dashEffect;
        public GameObject currentPanel;
        //public Boolean dragging;
        public GameObject positionArrow;

        void Awake()
        {
            //hopAnim = "hop";
            //idleAnim = "idle";
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
                currentPosition = (Vector2)this.gameObject.transform.position;
            }
        }

        void Update(){
        }

        public void ForcedMove(string direction = "Back", int moveAmount = 1)
        {
            var currentPanel = baseManager.movementManager.currentPanel;
            var currentPanelNum = currentPanel.GetComponent<Panels_Manager>().panelNumber;
            int targetPanelNum = currentPanelNum;
            if (direction == "Back")
            {
                targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum + moveAmount;
            }
            else if (direction == "Forward")
            {
                targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum - moveAmount;
            }
            targetPanelNum = targetPanelNum > 2 ? 2 : targetPanelNum;
            targetPanelNum = targetPanelNum < 0 ? 0 : targetPanelNum;
            var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum).gameObject;
            MoveToPanel(targetPanel, "hit");
        }

        public void SetSortingLayer(int sortingLayer ){
            origSortingOrder = sortingLayer;
        }

        public Vector2 GetAttackPos( GameObject target ){
            Vector2 attackedPos = new Vector2();
            if( target != null ){
                attackedPos.x = gameObject.name.IndexOf("Minion") > -1 ? target.GetComponent<Movement_Manager>().posMarkerMin.transform.position.x : target.GetComponent<Movement_Manager>().posMarker.transform.position.x;
                attackedPos.y = gameObject.name.IndexOf("Minion") > -1 ? target.GetComponent<Movement_Manager>().posMarkerMin.transform.position.y : target.GetComponent<Movement_Manager>().posMarker.transform.position.y;
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
            //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, string.IsNullOrEmpty(hopAnim) ? "hop" : hopAnim, false );
            //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, string.IsNullOrEmpty(idleAnim) ? "idle" : idleAnim, true, 0 );
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
        }

        public void OnEventMove(Spine.TrackEntry state, Spine.Event e ){
            if( e.Data.Name == "movementStart" ){
                moveToTarget( movementSpeed, baseManager.characterManager.characterModel.target );
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
                if (positionArrowManager.hoveredPanel && !positionArrowManager.hoveredPanel.currentOccupier)
                {
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

