using System;
using UnityEngine;

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
            AAanimation = "attack1";
        }

        public void RunAttackLoop(){  
                var targetDmgManager = baseManager.characterManager.characterModel.target ? baseManager.characterManager.characterModel.target.damageManager : null; 
                if( targetDmgManager != null && baseManager.characterManager.characterModel.isAlive && baseManager.characterManager.characterModel.canAutoAttack && !isAttacking && !baseManager.statusManager.DoesStatusExist( "stun" ) ){
                        //baseManager.characterManager.characterModel.isBusy = true;
                        isAttacking = true;
                        if ( !string.IsNullOrEmpty(AAanimation) && baseManager.animationManager.inAnimation == false ) 
                        {
                            var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, AAanimation, false).Animation.duration;
                            //baseManager.movementManager.moveToTarget(attackMovementSpeed, targetDmgManager.gameObject);
                            baseManager.movementManager.movementSpeed = 50f;
                            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, AAanimation, false);
                            baseManager.animationManager.inAnimation = true;
                            var dmgModel = new DamageModel{ 
                                skeletonAnimation = baseManager.animationManager.skeletonAnimation
                            };
                            baseManager.damageManager.calculatedamage( dmgModel );
                            Battle_Manager.taskManager.CallTaskBusyAnimation( animationDuration + baseManager.characterManager.characterModel.ATKspd, baseManager.animationManager, () => {
                                RunAttackLoop();
                            });
                        }
                }
                else {
                    Battle_Manager.taskManager.CallTaskBusyAnimation( baseManager.characterManager.characterModel.ATKspd, baseManager.animationManager, () => {
                        RunAttackLoop();
                    });
                }
                /*Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                    //RunAttackLoop();
                });*/
        }
    }
}

