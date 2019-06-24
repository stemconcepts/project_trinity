using System;

namespace AssemblyCSharp
{
    public class Auto_Attack_Manager: Base_Character_Manager
    {
        public string AAanimation {get; set;}
        public Auto_Attack_Manager()
        {
            AAanimation = "attack1";

        }

        void RunAttackLoop(){   
                if( characterManager.characterModel.isAlive && characterManager.characterModel.canAutoAttack && !characterManager.characterModel.isBusy && !statusManager.DoesStatusExist( "stun" ) ){
                        characterManager.characterModel.isBusy = true;
                        var targetDmgManager = characterManager.characterModel.target.damageManager; 
                        if ( !string.IsNullOrEmpty(AAanimation) && animationManager.inAnimation == false ) 
                        {
                            var animationDuration = animationManager.skeletonAnimation.state.SetAnimation(0, AAanimation, false).Animation.duration;
                            //movementManager.StartMovement(attackMovementSpeed);
                            animationManager.skeletonAnimation.state.SetAnimation(0, AAanimation, false);
                            animationManager.inAnimation = true;
                            var dmgModel = new DamageModel{ 
                                skeletonAnimation = animationManager.skeletonAnimation
                            };
                            damageManager.calculatedamage( dmgModel );
                            Battle_Manager.taskManager.CallTaskBusyAnimation( animationDuration, characterManager.animationManager );
                        }
                }
                else {
                    Battle_Manager.taskManager.CallTaskBusyAnimation( 5f, characterManager.animationManager );
                }
        }
    }
}

