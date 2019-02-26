using System;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    public class Animation_Manager : BasicManager
    {
        [SpineAnimation]
        public SkeletonAnimation skeletonAnimation;
        public MeshRenderer meshRenderer {get; set;}
        public bool inAnimation {get; set;}
        public string idleAnim {get; set;}
        public Damage_Manager damageManager {get; set;}

        public Animation_Manager()
        {
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
            damageManager = GetComponent<Damage_Manager>();
        }

        public void SetSortingLayer(int sortingLayer ){
            meshRenderer.sortingOrder = sortingLayer;
        }

        public void AddStatusAnimation( bool addStatus, string animName, string holdAnim, bool animHold ){
            if( addStatus ){
                //if animation is death, run immediately
                if( animName == "toDeath" ){
                    //set InAnimation to true... to stop "hit" event cancelling the status animation
                    skeletonAnimation.state.SetAnimation( 0, "toDeath", false);
                    //anim Control.inAnimation = true;
                } else {
                    damageManager.charDamageModel.hitAnimation = animName;
                    if( animHold ){
                        damageManager.charDamageModel.animationHold = animHold;
                    } else {
                        damageManager.charDamageModel.holdAnimation = holdAnim;
                    }
                }
            } else {
                skeletonAnimation.state.SetAnimation(0, "stunToIdle", true );
                //set InAnimation to true... to stop "hit" event cancelling the status animation
                damageManager.charDamageModel.hitAnimation = "";       
                damageManager.charDamageModel.animationHold = addStatus;
                //set animation back to idle after recovery animation is complete
                var animationDuration = skeletonAnimation.state.SetAnimation(0, "stunToIdle", false).Animation.duration;
                Battle_Manager.taskManager.CallTaskBusyAnimation( animationDuration, this, skeletonAnimation );
            }
        }
    }
}

