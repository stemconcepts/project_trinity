using System;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    public class Animation_Manager : Base_Character_Manager
    {
        public SkeletonAnimation skeletonAnimation {get; set;}
        public MeshRenderer meshRenderer {get; set;}
        public bool inAnimation {get; set;}
        public string idleAnim {get; set;}

        public Animation_Manager()
        {
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
        }

        public void SetSortingLayer(int sortingLayer ){
            meshRenderer.sortingOrder = sortingLayer;
        }

        public void AddStatusAnimation( bool addStatus, string animName, string holdAnim, bool animHold ){
            if( addStatus ){
                if( animName == "toDeath" ){
                    skeletonAnimation.state.SetAnimation( 0, "toDeath", false);
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
                damageManager.charDamageModel.hitAnimation = "";       
                damageManager.charDamageModel.animationHold = addStatus;
                var animationDuration = skeletonAnimation.state.SetAnimation(0, "stunToIdle", false).Animation.duration;
                Battle_Manager.taskManager.CallTaskBusyAnimation( animationDuration, this );
            }
        }
    }
}

