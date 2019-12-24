using System;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Animation_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public SkeletonAnimation skeletonAnimation;
        public MeshRenderer meshRenderer;
        public bool inAnimation;
        public string idleAnimation;
        public string hopAnimation;
        public string hitAnimation;
        public string toHeavy;
        public string toIdle;
        public string attackAnimation;
        void Awake()
        {
            idleAnimation = "idle";
            hopAnimation = "hop";
            hitAnimation = "hit";
            toHeavy = "toHeavy";
            attackAnimation = gameObject.name == "knight" ? "attack2" : "attack1";
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
        }

        public void SetSortingLayer(int sortingLayer ){
            meshRenderer.sortingOrder = sortingLayer;
        }

        public void SetBusyAnimation( float animationDuration ){
            Battle_Manager.taskManager.CallTask( animationDuration, () => {
                inAnimation = false;
            });
        }

        public void AddStatusAnimation( bool addAnimation, string animationName, string holdAnimation = null ){
            if( addAnimation ){
                if( animationName == "toDeath" ){
                    skeletonAnimation.state.SetAnimation( 0, "toDeath", false);
                } else {
                    if ( !inAnimation )
                    {
                        skeletonAnimation.state.SetAnimation(0, animationName, false);
                    }
                    if (!string.IsNullOrEmpty(holdAnimation))
                    {
                        hitAnimation = animationName;
                        idleAnimation = holdAnimation;
                        skeletonAnimation.state.AddAnimation(0, holdAnimation, true, 0);
                    }
                    /*if ( !string.IsNullOrEmpty(holdAnimation) ){
                        baseManager.damageManager.charDamageModel.hitAnimation = animationName;
                        skeletonAnimation.state.AddAnimation(0, holdAnimation, true, 0);
                        //baseManager.damageManager.charDamageModel.animationHold = animHold;
                        baseManager.damageManager.charDamageModel.holdAnimation = holdAnimation;
                        SetBusyAnimation(skeletonAnimation.state.SetAnimation(0, animationName, false).Animation.duration);
                    }*/
                }
            } else {
                hitAnimation = "hit";
                idleAnimation = "idle";
                skeletonAnimation.state.SetAnimation(0, "stunToIdle", false );
                skeletonAnimation.state.AddAnimation(0, idleAnimation, true, 0);
                //baseManager.damageManager.charDamageModel.hitAnimation = "";       
                //baseManager.damageManager.charDamageModel.animationHold = addStatus;
                var animationDuration = skeletonAnimation.state.SetAnimation(0, "stunToIdle", false).Animation.Duration;
                SetBusyAnimation(animationDuration);
            }
        }

        public void SetHitIdleAnimation(string hitAnimation, string hitIdleAnimation)
        {
            this.hitAnimation = !string.IsNullOrEmpty(hitAnimation) ? hitAnimation : this.hitAnimation;
            this.idleAnimation = !string.IsNullOrEmpty(hitIdleAnimation) ? hitIdleAnimation : this.idleAnimation;
        }
    }
}

