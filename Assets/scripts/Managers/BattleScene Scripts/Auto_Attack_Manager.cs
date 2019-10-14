using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Auto_Attack_Manager: MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public string AAanimation;
        public float attackMovementSpeed = 50f;
        public bool isAttacking = false;
        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
        }
        void Awake()
        {
            AAanimation = string.IsNullOrEmpty(AAanimation) ? "attack1" : AAanimation;
        }

        public void RunAttackLoop(){  
                var targetDmgManager = baseManager.characterManager.characterModel.target ? baseManager.characterManager.characterModel.target.damageManager : null; 
                if( targetDmgManager != null && baseManager.characterManager.characterModel.isAlive && baseManager.characterManager.characterModel.canAutoAttack && !isAttacking && !baseManager.statusManager.DoesStatusExist( "stun" ) ){
                        isAttacking = true;
                        if ( !string.IsNullOrEmpty(AAanimation) && baseManager.animationManager.inAnimation == false ) 
                        {
                            var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, AAanimation, false).Animation.duration;
                            baseManager.movementManager.movementSpeed = 50f;
                            baseManager.animationManager.inAnimation = true;
                            var dmgModel = new DamageModel(baseManager.characterManager.characterModel.target){ 
                                //baseManager = baseManager,
                                incomingDmg = baseManager.characterManager.characterModel.PAtk,
                                dueDmgTargets = new List<GameObject>(){
                                    baseManager.characterManager.characterModel.target.gameObject
                                }
                            };
                            baseManager.damageManager.currentTargetDmgModel = dmgModel;
                            targetDmgManager.calculatedamage( dmgModel );
                            Battle_Manager.taskManager.CallTask( animationDuration, () => {
                                baseManager.animationManager.inAnimation = false;
                            });
                            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, AAanimation, false);
                            Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                                if( this.gameObject.tag == "Enemy" ){
                                    baseManager.characterManager.characterModel.target = baseManager.characterManager.GetTarget();
                                }
                                RunAttackLoop();
                            });
                        }
                }
                else {
                    Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                        baseManager.animationManager.inAnimation = false;
                        if( this.gameObject.tag == "Enemy" ){
                            baseManager.characterManager.characterModel.target = baseManager.characterManager.GetTarget();
                        }
                        RunAttackLoop();
                    });
                }
        }
    }
}

