using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

namespace AssemblyCSharp
{
        public class SkillsModel : ScriptableObject {
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

        public void AttachStatus( List<StatusModel> singleStatusGroup, Status_Manager targetStatus, float power, SkillModel skill ){
            for( int i = 0; i < singleStatusGroup.Count; i++ ){
                singleStatusGroup[i].power = power;
                singleStatusGroup[i].duration = skill.duration;
                targetStatus.RunStatusFunction( singleStatusGroup[i] );
            }
        }
        public SkillModel onhitSkilltoRun;
    
        //Run Extra Skill Effects
        public void RunExtraEffect( SkillData data ){
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
        public void Dispel( SkillData data ){
            //target = skill_targetting.instance.currentTarget;
            List<Character_Manager> targets = data.target;
            foreach (var target in targets) {
                var targetStatus = target.GetComponent<Status_Manager>();
                var targetSpawnUI = target.GetComponent<Battle_Details_Manager>();
                //var debuffPower = data.skillModel == null ? data.skillModel.skillPower : data.skillModel.skillPower;
                var debuffPower = data.skillModel != null ? data.skillModel.skillPower : 0;

                //print ("running" + target);
                var positionId = 0;
                foreach( StatusLabelModel activeStatus in targetStatus.GetAllStatusIfExist( true ) ) {
                    if( positionId < debuffPower && activeStatus.buff && activeStatus.dispellable ) {
                            targetSpawnUI.RemoveLabel( activeStatus );
                            targetStatus.RunStatusFunction( activeStatus.statusModel );
                            //singlestatus.ChosenStatusToTurnOff();
                            //print ("Successfully Debuffed " + singlestatus.name);
                    } else if ( positionId == debuffPower && activeStatus.buff && !activeStatus.dispellable ){
                            //print ("Failed to Debuff " + singlestatus.name);
                    }
                    positionId++;
                } 
            }
        }
    
        //Does extra damage based on status effect      
        public void BonusDamage( SkillData data ){ 
            data.modifier = 1f;
            List<Character_Manager> targets = data.target;
            foreach (var target in targets) {
                var targetStatus = target.baseManager.statusManager;
                var statusList = targetStatus.GetAllStatusIfExist( false );
                foreach( var status in statusList ){
                    if ( status.statusModel.subStatus == data.skillModel.subStatus ){
                        data.modifier = data.skillModel.subStatus.modifierAmount;
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
}