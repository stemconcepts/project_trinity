using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class SkillData{
            public float modifier = 1f;
            public List<GameObject> target;
            public SkillModel enemySkill;
            public SkillModel classSkill;
    }

    public class Skill_Manager : Base_Character_Manager
    {
        private float multiplier {get; set;}
        public List<GameObject> finalTargets = new List<GameObject>();
        public bool isSkillactive { get; set; }
        public spawnUI spawnUIscript {get; set;}
        public GameObject currenttarget {get; set; }
        private SkillModel skillInWaiting {get; set;}
        public enum weaponSlotEnum{
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot {get; set;}
        public List<SkillModel> primaryWeaponSkills {get; set;}
        public List<SkillModel> secondaryWeaponSkills {get; set;}
        public SkillModel classSkill {get; set;}
        public bool waitingForSelection {get; set;}
        public Skill_Manager()
        {
            primaryWeaponSkills = new List<SkillModel>();
            secondaryWeaponSkills = new List<SkillModel>();
        }
        public void PrepSkillNew( SkillModel classSkill, bool weaponSkill = true ){
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
        private void GetTargets( SkillModel classSkill, bool randomTarget = false, bool weaponSkill = true ){
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

        public void SkillComplete( SkillModel classSkill, List<GameObject> targets, bool weaponSkill = true, GameObject player = null ){
            var power = 0.0f;
            var eM = new EventModel(){
                eventName = "OnSkillCast",
                eventCaller = gameObject
            };
            Battle_Manager.eventManager.BuildEvent( eM );
            if( classSkill.isFlat ){ 
                power = classSkill.isSpell ? classSkill.magicPower : classSkill.skillPower;
            } else {
                power = classSkill.isSpell ? classSkill.newMP : classSkill.newSP;
            }
            SkillData data = new SkillData(){ 
                target = targets,
                classSkill = classSkill
            };
            classSkill.RunExtraEffect(data); 
            SetAnimations( classSkill );
            DealHealDmg( classSkill, targets, power * data.modifier );
            SkillActiveSet( classSkill, false ); //Set that skill is ready to be used again
            for (int i = 0; i < Battle_Manager.battleInterfaceManager.Count; i++)
            {
                var iM = Battle_Manager.battleInterfaceManager[i];
                if( iM.skill == classSkill ) {
                    Battle_Manager.taskManager.skillcoolDownTask( classSkill, iM.skillCDImage );
                }
            }
        }

        private void DealHealDmg( SkillModel classSkill, List<GameObject> targets, float power ){
            var characterManager = gameObject.GetComponent<Character_Manager>();
            characterManager.damageManager.charDamageModel.dueDmgTargets = targets;
            foreach (var target in targets) {
                if ( classSkill.fxObject != null ){
                    characterManager.effectsManager.callEffectTarget( target, classSkill.fxObject );
                }
                if( target.tag == "Enemy" && characterManager.characterModel.isAlive ){
                    foreach (var status in classSkill.singleStatusGroup) {
                        status.dispellable = classSkill.statusDispellable;
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
                    classSkill.AttachStatus( classSkill.singleStatusGroup, target.GetComponent<Status_Manager>(), power, classSkill );
                } else if( target.tag == "Player" && characterManager.characterModel.isAlive ){ 
                    foreach (var status in classSkill.singleStatusGroupFriendly) {
                        status.dispellable = classSkill.statusFriendlyDispellable;
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
                    classSkill.AttachStatus( classSkill.singleStatusGroupFriendly, target.GetComponent<Status_Manager>(), power, classSkill );
                }
            }
            characterManager.characterModel.actionPoints -= classSkill.skillCost;
        }

        private void SetAnimations( SkillModel classSkill ){
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

        public void SkillActiveSet( SkillModel classSkill, bool setActive, bool skillCancel = false ){
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

        private bool CheckSkillAvail( SkillModel classSkill ){
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

