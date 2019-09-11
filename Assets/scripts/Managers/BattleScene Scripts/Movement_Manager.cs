using System;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    public class Movement_Manager : Base_Character_Manager 
    {
        public string idleAnim {get; set;}
        public string hopAnim {get; set;}
        public int origSortingOrder { get; set; }
        public float movementSpeed { get; set; }
        private float moveToHomeSpeed { get; set; }
        private Vector2 currentPosition {get; set;}
        public GameObject posMarkerMin;
        public GameObject posMarker;
        public GameObject dashEffect;
        public GameObject currentPanel { get; set; }
        public Movement_Manager()
        {
            hopAnim = "hop";
            idleAnim = "idle";
            movementSpeed = moveToHomeSpeed = 6f;
        }

        void Start(){
            currentPosition = (Vector2)this.gameObject.transform.position;
            animationManager.GetComponent<SkeletonAnimation>().state.Event += OnEventMove;
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
    
        public void moveToTarget( float movementSpeed, GameObject target ){
            var targetSkeletonAnimation = target.GetComponent<Animation_Manager>().skeletonAnimation;
            var targetSortingOrder = targetSkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder;
            animationManager.gameObject.GetComponent<MeshRenderer>().sortingOrder = targetSortingOrder;
            var targetpos = GetAttackPos( target );
            Battle_Manager.taskManager.MoveForwardTask( 0.009f, targetpos, dashEffect );
        }
    
        public void moveToHome(){
            var origPos = currentPanel.transform.position;
            animationManager.GetComponent<MeshRenderer>().sortingOrder = origSortingOrder;
            //var moveHomeSpeed = moveToHomeSpeed == 0f ? 50f : moveToHomeSpeed;
            var panelPos = currentPanel.transform.position;
            panelPos.y = panelPos.y + 6f;
            Battle_Manager.taskManager.moveBackTask( 0.009f, panelPos, currentPosition );
            //var hopAnim = gameObject.tag == "Enemy" ? "hop" : hopAnim;
            //var idleAnim = gameObject.tag == "Enemy" ? "idle" : idleAnim;
            animationManager.GetComponent<SkeletonAnimation>().state.SetAnimation(0, gameObject.tag == "Enemy" ? "hop" : hopAnim, false );
            animationManager.GetComponent<SkeletonAnimation>().state.AddAnimation(0, gameObject.tag == "Enemy" ? "idle" : idleAnim, true, 0 );
        }

        /*public void StartMovement( float movementSpeedVar ) {
            movementSpeed = movementSpeedVar;
            animationManager.GetComponent<SkeletonAnimation>().state.Event -= OnEventMove;
            animationManager.GetComponent<SkeletonAnimation>().state.Event += OnEventMove;
        }*/
    
        public void OnEventMove(Spine.TrackEntry state, Spine.Event e ){
            if( e.Data.name == "movementStart" ){
                moveToTarget( movementSpeed, characterManager.gameObject );
            } else if( e.Data.name == "movementBack" && characterManager.characterModel.currentPosition != characterManager.characterModel.origPosition ){
                moveToHome();
            }
        }
    }
}

