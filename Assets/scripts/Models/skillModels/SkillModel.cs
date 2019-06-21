using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class SkillModel : ScriptableObject {
        [Header("Skill Role:")]
        public ClassEnum Class;
        public enum ClassEnum{
            Guardian,
            Walker,
            Stalker
        }
        [Header("Default Skill Variables:")]
        public string skillName;
        public Sprite skillIcon;
        public bool equipped;
        public bool learned;
        public string displayName;
        public float attackMovementSpeed = 50f;
        public string animationType;
        public string animationCastingType;
        public string animationRepeatCasting;
        public bool loopAnimation;
        [Multiline]
        public string skillDesc;
        public bool skillActive;
        public bool skillConfirm;
        public int skillCost;
        public float skillPower;
        public float newSP;
        public float magicPower;
        public float newMP;
        public float duration;
        public float castTime;
        public bool castTimeReady;
        public float skillCooldown;
        public float currentCDAmount;
        public bool isSpell;
        public bool isFlat;
        public GameObject hitEffect;
        public bool doesDamage;
        public bool movesToTarget;
        public bool healsDamage;
        [Header("Target choices:")]
        public bool self;
        public bool enemy;
        public bool friendly;
        public bool allFriendly;
        public bool allEnemy;
        [Header("Extra Effect:")]
        public ExtraEffectEnum ExtraEffect;
        public enum ExtraEffectEnum{
            None,
            Dispel,
            BonusDamage
        }
        public subStatus subStatus;
        [Header("FX Animation:")]
        public GameObject fxObject;
        public fxPosEnum fxPos;
        public enum fxPosEnum{
            center,
            bottom,
            front,
            top
        }
    
        [Header("Status Effects:")]
        public List<StatusModel> singleStatusGroup = new List<StatusModel>();
        public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        public List<StatusModel> singleStatusGroupFriendly = new List<StatusModel>();
        public bool statusFriendlyDispellable = true;

        public void AttachStatus( List<StatusModel> singleStatusGroup, status targetStatus, float power, SkillModel skill ){
            for( int i = 0; i < singleStatusGroup.Count; i++ ){
                targetStatus.RunStatusFunction( singleStatusGroup[i], power, skill.duration );
            }
        }
        public enemySkill onhitSkilltoRun;
    
        //Run Extra Skill Effects
        public void RunExtraEffect( Data data ){
            switch( ExtraEffect ){
                case ExtraEffectEnum.None:
                    break;
                case ExtraEffectEnum.Dispel:
                    Dispel(data);
                    break;
                case ExtraEffectEnum.BonusDamage:
                    BonusDamage(data);
                    break;
                }
    
        }
    
        //removes x buffs from an enemy
        public void Dispel( Data data ){
            //target = skill_targetting.instance.currentTarget;
            List<GameObject> targets = data.target;
            foreach (var target in targets) {
                var targetStatus = target.GetComponent<Status_Manager>();
                var targetSpawnUI = target.GetComponent<spawnUI>();
                var debuffPower = data.enemySkill == null ? data.classSkill.skillPower : data.enemySkill.skillPower;
    
                //print ("running" + target);
                var positionId = 0;
                foreach( statussinglelabel activeStatus in targetStatus.GetAllStatusIfExist( true ) ) {
                    if( positionId < debuffPower && activeStatus.buff && activeStatus.dispellable ) {
                            targetSpawnUI.RemoveLabel( activeStatus.statusname, activeStatus.buff );
                            targetStatus.RunStatusFunction( activeStatus.singleStatus, statusOff:true );
                            //singlestatus.ChosenStatusToTurnOff();
                            //print ("Successfully Debuffed " + singlestatus.name);
                    } else if ( positionId == debuffPower && activeStatus.buff && !activeStatus.dispellable && !activeStatus.singleStatus.active ){
                            //print ("Failed to Debuff " + singlestatus.name);
                    }
                    positionId++;
                } 
            }
        }
    
        //Does extra damage based on status effect      
        public void BonusDamage( Data data ){ 
            data.modifier = 1f;
            List<GameObject> targets = data.target;
            foreach (var target in targets) {
                var targetStatus = target.GetComponent<status>();
                var statusList = targetStatus.GetAllStatusIfExist( false );
                foreach( var status in statusList ){
                    if ( status.singleStatus.subStatus == data.classSkill.subStatus ){
                        data.modifier = data.classSkill.subStatus.modifierAmount;
                    }
                }
                /*if( targetStatus.DoesStatusExist("poison")){
                    data.modifier = 1.8f;
                    //targetStatus.PoisonOn( 4, true, 0 );
                }*/
            }
            //return multiplier;
        }
    }

