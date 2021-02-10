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
        public string stunToIdle;
        public string attackAnimation;
        void Awake()
        {
            idleAnimation = "idle";
            hopAnimation = "hop";
            hitAnimation = "hit";
            toHeavy = "toHeavy";
            stunToIdle = "stunToIdle";
            attackAnimation = "attack1";
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
        }

        void Start()
        {
            if( skeletonAnimation.state.Data.SkeletonData.FindAnimation("intro") != null)
            {
                skeletonAnimation.state.SetAnimation(0, "intro", false);
                skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
            } else
            {
                skeletonAnimation.state.SetAnimation(0, "idle", true);
            }
        }

        public void SetSortingLayer(int sortingLayer ){
            meshRenderer.sortingOrder = sortingLayer;
        }

        public void SetBusyAnimation( float animationDuration ){
            Battle_Manager.taskManager.CallTask( animationDuration, () => {
                inAnimation = false;
            });
        }

        public float PlayAnimation( string animationName, bool loop = false)
        {
            skeletonAnimation.state.SetAnimation(0, animationName, loop);
            return skeletonAnimation.state.SetAnimation(0, animationName, loop).Animation.Duration;
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
                }
            } else {
                hitAnimation = "hit";
                idleAnimation = "idle";
                //skeletonAnimation.state.SetAnimation(0, stunToIdle, false );
                //baseManager.damageManager.charDamageModel.hitAnimation = "";       
                //baseManager.damageManager.charDamageModel.animationHold = addStatus;
                var animationDuration = skeletonAnimation.state.SetAnimation(0, stunToIdle, false).Animation.Duration;
                skeletonAnimation.state.AddAnimation(0, idleAnimation, true, animationDuration);
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

