using System;
using Spine;
using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public enum animationOptionsEnum
    {
        none,
        intro,
        idle,
        idleHeavy,
        hop,
        hopHeavy,
        hit,
        hitHeavy,
        toHeavy,
        stunned,
        toStunned,
        stunToIdle,
        //Attacks
        attack1,
        attack1Heavy,
        attack2,
        attack3,
        attack4,
        rangeAttack,
        rangeAttackHeavy,
        attackCharge,
        attackChargeWait,
        attackRelease,
        jumpAttack,
        jumpAttackHeavy,
        jumpAttackHeavy2,
        castAoe,
        castSelf,
        castSelf2,
        castTarget,
        crush,
        toDeath,
        death
    };

    [System.Serializable]
    public class Animation_Manager : MonoBehaviour
    {
        public BaseCharacterManagerGroup baseManager;
        public SkeletonAnimation skeletonAnimation;
        public SkeletonAnimationMulti skeletonAnimationMulti;
        public MeshRenderer meshRenderer;
        public bool inAnimation;
        public animationOptionsEnum idleAnimation;
        public animationOptionsEnum hopAnimation;
        public animationOptionsEnum hitAnimation;
        public animationOptionsEnum toHeavy;
        public animationOptionsEnum stunToIdle;
        public animationOptionsEnum attackAnimation;
        private float timeTillNextIdle;

        void Awake()
        {
            idleAnimation = animationOptionsEnum.idle;
            hopAnimation = animationOptionsEnum.hop;
            hitAnimation =  animationOptionsEnum.hit;
            toHeavy = animationOptionsEnum.toHeavy;
            stunToIdle = animationOptionsEnum.stunToIdle;
            attackAnimation = animationOptionsEnum.attack1;
            baseManager = this.gameObject.GetComponent<BaseCharacterManagerGroup>();
            skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
            meshRenderer = transform.Find("Animations").GetComponent<MeshRenderer>();
            skeletonAnimationMulti = this.transform.Find("Animations").GetComponent<SkeletonAnimationMulti>();
        }

        void Update()
        {
            
        }

        public Bounds GetSpriteBounds()
        {
            List<MeshRenderer> mr = skeletonAnimationMulti != null ? skeletonAnimationMulti.GetMeshRenderers() : new List<MeshRenderer>();
            if (mr.Count > 0)
            {
                return mr[0].bounds;
            }
            return meshRenderer.bounds;
        }

        void IdleToggle()
        {
            if (skeletonAnimation.state.Data.SkeletonData.FindAnimation("idle2") != null && BattleManager.gameManager.GetChance(5))
            {
                var duration = PlaySetAnimation("idle2", false);
                PlayAddAnimation("idle", true, duration);
                //skeletonAnimation.state.SetAnimation(0, "intro", false);
                //skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
            }
            BattleManager.taskManager.CallTask(5f, () =>
            {
                IdleToggle();
            });
        }

        void Start()
        {
            if( skeletonAnimation.state.Data.SkeletonData.FindAnimation("intro") != null)
            {
                PlaySetAnimation("intro", false);
                PlayAddAnimation("idle", true);
                //skeletonAnimation.state.SetAnimation(0, "intro", false);
                //skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
            } else
            {
                PlaySetAnimation("idle", true);
                //skeletonAnimation.state.SetAnimation(0, "idle", true);
            }
            IdleToggle();
        }

        public void SetSortingLayer(int sortingLayer ){
            if (skeletonAnimationMulti != null)
            {
                //Set instantiated animation gameobject properties
                skeletonAnimationMulti.GetMeshRenderers().ForEach(o => {
                    o.gameObject.layer = 8; 
                    o.sortingOrder = sortingLayer;
                    o.sortingLayerName = "characters";
                    });
            } else if(meshRenderer != null)
            {
                meshRenderer.sortingOrder = sortingLayer;
            }   
        }

        public void SetBusyAnimation( float animationDuration ){
            BattleManager.taskManager.CallTask( animationDuration, () => {
                inAnimation = false;
            });
        }

        public float PlaySetAnimation( string animationName, bool loop = false)
        {
            if (skeletonAnimationMulti != null && skeletonAnimationMulti.FindAnimation(animationName) != null)
            {
                var trackEntry = skeletonAnimationMulti.SetAnimation(animationName, loop);
                return trackEntry.Animation.Duration;
            } else
            {
                var trackEntry = skeletonAnimation.state.SetAnimation(0, animationName, loop);
                return trackEntry.Animation.Duration;
            }
        }

        public float PlayAddAnimation(string animationName, bool loop = false, float delay = 0)
        {
            if (skeletonAnimationMulti != null && skeletonAnimationMulti.FindAnimation(animationName) != null)
            {
                var trackEntry = skeletonAnimationMulti.AddAnimation(animationName, loop, delay);
                return trackEntry.Animation.Duration;
            }
            else
            {
                var trackEntry = skeletonAnimation.state.AddAnimation(0, animationName, loop, delay);
                return trackEntry.Animation.Duration;
            }
        }

        public void AddStatusAnimation( bool addAnimation, animationOptionsEnum animationName, animationOptionsEnum holdAnimation = animationOptionsEnum.none)
        {
            if( addAnimation ){
                if( animationName == animationOptionsEnum.toDeath)
                {
                    PlaySetAnimation("toDeath", false);
                    skeletonAnimation.state.SetAnimation( 0, "toDeath", false);
                } else {
                    if ( !inAnimation )
                    {
                        PlaySetAnimation(animationName.ToString(), false);
                        //skeletonAnimation.state.SetAnimation(0, animationName.ToString(), false);
                    }
                    if (holdAnimation == animationOptionsEnum.none)
                    {
                        hitAnimation = animationName;
                        idleAnimation = holdAnimation;

                        PlayAddAnimation(holdAnimation.ToString(), true);
                        //skeletonAnimation.state.AddAnimation(0, holdAnimation.ToString(), true, 0);
                    }
                }
            } else {
                hitAnimation = animationOptionsEnum.hit;
                idleAnimation = animationOptionsEnum.idle;
                var animationDuration = PlaySetAnimation(stunToIdle.ToString(), false);
                PlayAddAnimation(idleAnimation.ToString(), false, animationDuration);
                //var animationDuration = skeletonAnimation.state.SetAnimation(0, stunToIdle.ToString(), false).Animation.Duration;
                //skeletonAnimation.state.AddAnimation(0, idleAnimation.ToString(), true, animationDuration);
                SetBusyAnimation(animationDuration);
            }
        }

        public void SetHitIdleAnimation(animationOptionsEnum hitAnimation, animationOptionsEnum hitIdleAnimation)
        {
            this.hitAnimation = hitAnimation != animationOptionsEnum.none ? hitAnimation : this.hitAnimation;
            this.idleAnimation = hitIdleAnimation != animationOptionsEnum.none ? hitIdleAnimation : this.idleAnimation;
        }
    }
}

