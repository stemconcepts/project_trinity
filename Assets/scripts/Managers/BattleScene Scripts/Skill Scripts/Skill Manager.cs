using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Skill_Manager : Base_Character_Manager
    {
        private float multiplier {get; set;}
        public List<GameObject> finalTargets = new List<GameObject>();
        public bool isSkillactive { get; set; }
        public spawnUI spawnUIscript {get; set;}
        public GameObject currenttarget {get; set; }
        private classSkills skillInWaiting {get; set;}
        public enum weaponSlotEnum{
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot {get; set;}
        public List<classSkills> primaryWeaponSkills {get; set;} = new List<classSkills>();
        public List<classSkills> secondaryWeaponSkills {get; set;} = new List<classSkills>();
        public classSkills classSkill {get; set;}
        public bool waitingForSelection {get; set;}
        public Skill_Manager()
        {
        }

        public void PrepSkillNew( classSkills classSkill, bool weaponSkill = true ){
            if( CheckSkillAvail( classSkill ) ){
                if( !waitingForSelection ){
                    SkillActiveSet( classSkill, true );
                    GetTargets(classSkill, weaponSkill:weaponSkill);
                }
            }
        }

        public void CalculateSkillPower(){
            for(int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newSP = primaryWeaponSkills[i].skillPower * characterManager.characterModel.PAtk;
                secondaryWeaponSkills[i].newSP = secondaryWeaponSkills[i].skillPower * characterManager.characterModel.PAtk;
                classSkill.newSP = classSkill.skillPower * characterManager.characterModel.PAtk;
            }
        }
        
        public void CalculateMagicPower(){
            for(int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newMP = primaryWeaponSkills[i].magicPower * characterManager.characterModel.MAtk;
                secondaryWeaponSkills[i].newMP = secondaryWeaponSkills[i].magicPower * characterManager.characterModel.MAtk;
                classSkill.newMP = classSkill.magicPower * characterManager.characterModel.MAtk;
            }
        }
        private void GetTargets( classSkills classSkill, bool randomTarget = false, bool weaponSkill = true ){
            var player = this.gameObject;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if( classSkill.self ){ finalTargets.Add( player ); }
            if( classSkill.allEnemy ){ finalTargets.AddRange( enemies ); }
            if( classSkill.allFriendly ){ finalTargets.AddRange( players ); }
            if( classSkill.friendly || classSkill.enemy ){
                Battle_Manager.taskManager.waitForTargetTask( classSkill, player, weaponSkill );
                return;
            } else {
                SkillComplete( classSkill, finalTargets, weaponSkill, player: player );
                return;
            }
        }

        public void SkillComplete( classSkills classSkill, List<GameObject> targets, bool weaponSkill = true ){
            var power = 0;
            Event_Manager.BuildEvent( "OnSkillCast", eventCallerVar: gameObject );
            if( classSkill.isFlat ){ 
                power = classSkill.isSpell ? classSkill.magicPower : classSkill.skillPower;
            } else {
                power = classSkill.isSpell ? classSkill.newMP : classSkill.newSP;
            }
            Data data = new Data(); 
            data.target = targets;
            data.classSkill = classSkill;
            classSkill.RunExtraEffect(data); 
            SetAnimations( classSkill );
            DealHealDmg( classSkill, targets, power * data.modifier );
            SkillActiveSet( classSkill, false ); //Set that skill is ready to be used again
            Battle_Manager.taskManager.skillcoolDownTask( classSkill );
        }

        private void DealHealDmg( classSkills classSkill, List<GameObject> targets, float power ){
            var characterManager = gameObject.GetComponent<Character_Manager>();
            characterManager.damageManager.charDamageModel.dueDmgTargets = targets;
            foreach (var target in targets) {
                var targetCalculateDmgScript = target.GetComponent<calculateDmg>();
                if ( classSkill.fxObject != null ){
                    characterManager.effectsManager.callEffectTarget( target, classSkill.fxObject );
                }
                if( target.tag == "Enemy" && characterManager.characterModel.isAlive ){
                    foreach (var status in classSkill.singleStatusGroup) {
                        status.debuffable = classSkill.statusDispellable;
                    };
                    if( classSkill.doesDamage ){ 
                        var dmgModel = new DamageModel{
                            incomingDmg = power,
                            classSkill = classSkill,
                            dmgSource = gameObject,
                            skeletonAnimation = characterManager.animationManager.skeletonAnimation
                        };
                        characterManager.damageManager.calculatedamage( dmgModel ); 
                    };
                    classSkill.AttachStatus( classSkill.singleStatusGroup, target.GetComponent<status>(), power, classSkill );
                } else if( target.tag == "Player" && characterManager.characterModel.isAlive ){ 
                    foreach (var status in classSkill.singleStatusGroupFriendly) {
                        status.debuffable = classSkill.statusFriendlyDispellable;
                    }
                    if( classSkill.healsDamage ){ 
                        var dmgModel = new DamageModel{
                            incomingHeal = power,
                            classSkill = classSkill,
                            dmgSource = gameObject,
                            skeletonAnimation = characterManager.animationManager.skeletonAnimation
                        };
                        characterManager.damageManager.calculateHdamage( dmgModel ); 
                    };
                    classSkill.AttachStatus( classSkill.singleStatusGroupFriendly, target.GetComponent<status>(), power, classSkill );
                }
            }
            characterManager.characterModel.actionPoints -= classSkill.skillCost;
        }

        private void SetAnimations( classSkills classSkill ){
            if( classSkill.animationType != null ){
                characterManager.animationManager.inAnimation = true;
                characterManager.animationManager.skeletonAnimation.state.SetAnimation(0, classSkill.animationType, classSkill.loopAnimation);
                var animationDuration = characterManager.animationManager.skeletonAnimation.state.SetAnimation(0, classSkill.animationType, classSkill.loopAnimation).Animation.duration;
                Battle_Manager.taskManager.CallTaskBusyAnimation( animationDuration, characterManager.animationManager );
                if( classSkill.attackMovementSpeed > 0 ){
                    characterManager.characterModel.isAttacking = true;
                    characterManager.movementManager.movementSpeed = classSkill.attackMovementSpeed;
                } else {
                    var idleAnim = characterManager.movementManager.idleAnim;
                    characterManager.animationManager.skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0 );
                }
            }
            
        }

        public void SkillActiveSet( classSkills classSkill, bool setActive, bool skillCancel = false ){
            if ( setActive || skillCancel ){ //turn skill active on begin but inactive on cancel
                classSkill.skillActive = setActive;
                skillInWaiting = classSkill;
            }
            if( Battle_Manager.taskManager.tasks["waitForTarget"] != null ){
                Battle_Manager.taskManager.tasks["waitForTarget"].Stop();
                Time.timeScale = 1f;
            } 
            if (!setActive || skillCancel ){ 
                finalTargets.Clear(); 
                waitingForSelection = false;
                skillInWaiting = null;
            }
        }

        private bool CheckSkillAvail( classSkills classSkill ){
            var availActionPoints = characterManager.characterModel.actionPoints;
            if ( availActionPoints >= classSkill.skillCost && !classSkill.skillActive )
            {
                return true;
            } else {
                print( classSkill.displayName + " is not available or waiting for target" );
                return false;
            }
        }
    }
}

