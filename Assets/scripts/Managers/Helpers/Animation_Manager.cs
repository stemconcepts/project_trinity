using System;
using Spine;
using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

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
        public SkeletonAnimationMultiOld skeletonAnimationMulti;
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
            skeletonAnimationMulti = this.transform.Find("Animations").GetComponent<SkeletonAnimationMultiOld>();
        }

        void Update()
        {
            
        }

        public List<MeshRenderer> GetMeshRenderers()
        {
            if (skeletonAnimationMulti != null)
            {
                return skeletonAnimationMulti.GetMeshRenderers();
            } else
            {
                return new List<MeshRenderer>() { meshRenderer };
            }
        }

        public float GetAnimationDuration(string animationName)
        {
            if (skeletonAnimationMulti != null)
            {
                var anim = skeletonAnimationMulti.FindAnimation(animationName);
                return anim != null ? anim.Duration : 2.5f;
            }
            else
            {
                var animS = skeletonAnimation.state.Data.SkeletonData.Animations.Items.ToList().Where(o => o.Name == animationName).FirstOrDefault();
                if (animS != null)
                {
                    return animS.Duration;
                }
            }
            return 2.5f;
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
            if (skeletonAnimation?.state.Data.SkeletonData.FindAnimation("idle2") != null && BattleManager.gameManager.GetChance(5))
            {
                var trackEntry = PlaySetAnimation("idle2", false);
                if (trackEntry != null)
                {
                    PlayAddAnimation("idle", true, trackEntry.Animation.Duration);
                }
            }
            BattleManager.taskManager.CallTask(5f, () =>
            {
                IdleToggle();
            });
        }

        void Start()
        {
            if (skeletonAnimation?.state.Data.SkeletonData.FindAnimation("intro") != null)
            {
                PlaySetAnimation("intro", false);
                PlayAddAnimation("idle", true);
            } else
            {
                PlaySetAnimation("idle", true);
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
                    o.sortingLayerName = "Battle - Characters";
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

        public TrackEntry PlaySetAnimation( string animationName, bool loop = false)
        {
            if (skeletonAnimationMulti != null && skeletonAnimationMulti.FindAnimation(animationName) != null)
            {
                var trackEntry = skeletonAnimationMulti.SetAnimation(animationName, loop);

                return trackEntry;
            } else
            {
                if (skeletonAnimation.state.Data.SkeletonData.Animations.Items.ToList().Any(o => o.Name == animationName))
                {
                    var trackEntry = skeletonAnimation.state.SetAnimation(0, animationName, loop);
                    return trackEntry;
                }
                return null;
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
                if (skeletonAnimation.state.Data.SkeletonData.Animations.Items.ToList().Any(o => o.Name == animationName))
                {
                    var trackEntry = skeletonAnimation.state.AddAnimation(0, animationName, loop, delay);
                    return trackEntry.Animation.Duration;
                }
                return 0;
            }
        }

        public void AddStatusAnimation( bool addAnimation, animationOptionsEnum animationName, animationOptionsEnum holdAnimation = animationOptionsEnum.none)
        {
            if( addAnimation){
                if( animationName == animationOptionsEnum.toDeath)
                {
                    PlaySetAnimation("toDeath", false);
                    skeletonAnimation?.state.SetAnimation( 0, "toDeath", false);
                } else {
                    if ( !inAnimation )
                    {
                        PlaySetAnimation(animationName.ToString(), true);
                    }
                    if (holdAnimation != animationOptionsEnum.none)
                    {
                        hitAnimation = animationName;
                        idleAnimation = holdAnimation;

                        PlayAddAnimation(holdAnimation.ToString(), true);
                    }
                }
            } else {
                hitAnimation = animationOptionsEnum.hit;
                idleAnimation = animationOptionsEnum.idle;
                var trackEntry = PlaySetAnimation(stunToIdle.ToString(), false);
                if(trackEntry != null)
                {
                    PlayAddAnimation(idleAnimation.ToString(), false, trackEntry.Animation.Duration);
                    SetBusyAnimation(trackEntry.Animation.Duration);
                }
            }
        }

        public void SetHitIdleAnimation(animationOptionsEnum hitAnimation, animationOptionsEnum hitIdleAnimation)
        {
            this.hitAnimation = hitAnimation != animationOptionsEnum.none ? hitAnimation : this.hitAnimation;
            this.idleAnimation = hitIdleAnimation != animationOptionsEnum.none ? hitIdleAnimation : this.idleAnimation;
        }
    }
}

