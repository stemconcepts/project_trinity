using System;
using UnityEngine;
using Spine.Unity;
using System.Collections;

namespace AssemblyCSharp
{
    public class Movement_Manager : MonoBehaviour 
    {
        public Base_Character_Manager baseManager;
        public string idleAnim;
        public string hopAnim;
        public int origSortingOrder;
        public float movementSpeed;
        private float moveToHomeSpeed;
        public Vector2 origPosition ;
        public Vector2 currentPosition ;
        public GameObject posMarkerMin;
        public GameObject posMarker;
        public GameObject dashEffect;
        public GameObject currentPanel;

        void Awake()
        {
            hopAnim = "hop";
            idleAnim = "idle";
            movementSpeed = 50f;
            moveToHomeSpeed = 50f;
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

        IEnumerator moveForward( Vector2 targetPosVar, float movementSpeedVar ){
            baseManager.effectsManager.CallEffect( dashEffect, "bottom" );
            while((Vector2)transform.position != targetPosVar && baseManager.autoAttackManager.isAttacking ){
                float step = movementSpeedVar * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, targetPosVar, step);
                yield return null;
            }
        }
    
        public void moveToHome(){
            baseManager.animationManager.meshRenderer.sortingOrder = origSortingOrder;
            Battle_Manager.taskManager.moveBackTask( baseManager, moveToHomeSpeed, origPosition, currentPosition );
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, string.IsNullOrEmpty(hopAnim) ? "hop" : hopAnim, false );
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, string.IsNullOrEmpty(idleAnim) ? "idle" : idleAnim, true, 0 );
        }
    
        public void OnEventMove(Spine.TrackEntry state, Spine.Event e ){
            if( e.Data.name == "movementStart" ){
                moveToTarget( movementSpeed, baseManager.characterManager.characterModel.target );
            } else if( e.Data.name == "movementBack" ){
                baseManager.autoAttackManager.isAttacking = false;
                if( origPosition != (Vector2)this.gameObject.transform.position ){
                    moveToHome();
                }
            }
        }
    }
}

