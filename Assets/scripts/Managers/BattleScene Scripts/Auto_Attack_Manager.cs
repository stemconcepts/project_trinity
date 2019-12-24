using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Auto_Attack_Manager: MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        //public string AAanimation;
        public float attackMovementSpeed = 50f;
        public bool isAttacking = false;
        //public DamageModel dmgModel;
        public 
        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAHit;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAComplete;
            }
        }

        public void RunAttackLoop(){  
                var targetDmgManager = baseManager.characterManager.characterModel.target ? baseManager.characterManager.characterModel.target.damageManager : null; 
                if( targetDmgManager != null && baseManager.characterManager.characterModel.isAlive && baseManager.characterManager.characterModel.canAutoAttack && !isAttacking && !baseManager.statusManager.DoesStatusExist( "stun" ) && !baseManager.animationManager.inAnimation && !baseManager.skillManager.isSkillactive ){
                        isAttacking = true;
                        if ( !string.IsNullOrEmpty(baseManager.animationManager.attackAnimation) && baseManager.animationManager.inAnimation == false ) 
                        {
                            var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.attackAnimation, false).Animation.Duration;
                            baseManager.movementManager.movementSpeed = 50f;
                            baseManager.animationManager.inAnimation = true;
                            var dmgModel = new DamageModel(){ 
                                baseManager = baseManager.characterManager.characterModel.target,
                                incomingDmg = baseManager.characterManager.characterModel.PAtk,
                                dmgSource = baseManager.characterManager,
                                dueDmgTargets = new List<Character_Manager>(){
                                    baseManager.characterManager.characterModel.target.characterManager
                                },
                                hitEffectPositionScript = baseManager.characterManager.characterModel.target.effectsManager.fxCenter.transform
                            };
                            targetDmgManager.autoAttackDmgModels.Add(gameObject.name,dmgModel);
                            targetDmgManager.calculatedamage( dmgModel );
                            Battle_Manager.taskManager.CallTask( animationDuration, () => {
                                baseManager.animationManager.inAnimation = false;
                            });
                            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.attackAnimation, false);
                            Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                                if( this.gameObject.tag == "Enemy" ){
                                    baseManager.characterManager.characterModel.target = baseManager.characterManager.GetTarget(true);
                                }
                                RunAttackLoop();
                            });
                        }
                }
                else {
                    Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                        baseManager.animationManager.inAnimation = false;
                        if( this.gameObject.tag == "Enemy" ){
                            baseManager.characterManager.characterModel.target = baseManager.characterManager.GetTarget(true);
                        }
                        RunAttackLoop();
                    });
                }
        }

        public void OnEventAAHit(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "hit" && isAttacking)
            {
                var target = baseManager.characterManager.characterModel.target;
                var targetDamageManager = target.damageManager;
                if (targetDamageManager.autoAttackDmgModels.ContainsKey(gameObject.name))
                {
                    var damageModel = targetDamageManager.autoAttackDmgModels[gameObject.name];
                    targetDamageManager.TakeDmg(damageModel, e.Data.Name);
                    var eventModel = new EventModel
                    {
                        eventName = "OnDealingDmg",
                        extTarget = target.characterManager,
                        eventCaller = baseManager.characterManager,
                        extraInfo = damageModel.damageTaken
                    };
                    Battle_Manager.eventManager.BuildEvent(eventModel);
                    targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                } else
                {
                    Game_Manager.logger.Log("damageModel missing from" + target.name + " dictionary");
                }
            }
        }

        public void OnEventAAComplete(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "endEvent")
            {
                var target = baseManager.characterManager.characterModel.target;
                var targetDamageManager = target.damageManager;
                if (targetDamageManager.autoAttackDmgModels.ContainsKey(gameObject.name))
                {
                    targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                }
            }
        }
    }
}

