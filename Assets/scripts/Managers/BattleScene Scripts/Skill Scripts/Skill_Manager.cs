using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Skill_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        private float multiplier;
        public List<GameObject> finalTargets = new List<GameObject>();
        public bool isSkillactive;
        //public spawnUI spawnUIscript;
        public GameObject currenttarget;
        public bool usesWeapons;
        private SkillModel skillInWaiting;
        public enum weaponSlotEnum{
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot;
        public List<SkillModel> primaryWeaponSkills = new List<SkillModel>();
        public List<SkillModel> secondaryWeaponSkills = new List<SkillModel>();
        public SkillModel skillModel;
        public bool waitingForSelection;

        //Enemy specific properties
        public EnemyPhase enemyPhase;
        public enum EnemyPhase {
            phaseOne,
            phaseTwo,
            phaseThree
        }
        public List<SkillModel> enemySkillList = new List<SkillModel>();
        public List<SkillModel> phaseSkillList = new List<SkillModel>();

        void Start()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if( enemySkillList.Count > 0 ){
                BeginSkillRotation( enemyPhase );
            }
        }

        public void PrepSkillNew( SkillModel skillModel, bool weaponSkill = true ){
            if( CheckSkillAvail( skillModel ) ){
                if( !waitingForSelection ){
                    SkillActiveSet( skillModel, true );
                    GetTargets(skillModel, weaponSkill:weaponSkill);
                }
            }
        }

        public void CalculateSkillPower(){
            for(int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newSP = primaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
                secondaryWeaponSkills[i].newSP = secondaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
                skillModel.newSP = skillModel.skillPower * baseManager.characterManager.characterModel.PAtk;
            }
        }
        
        public void CalculateMagicPower(){
            for(int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newMP = primaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
                secondaryWeaponSkills[i].newMP = secondaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
                skillModel.newMP = skillModel.magicPower * baseManager.characterManager.characterModel.MAtk;
            }
        }
        private void GetTargets( SkillModel skillModel, bool randomTarget = false, bool weaponSkill = true ){
            var player = this.gameObject;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if( skillModel.self ){ finalTargets.Add( player ); }
            if( skillModel.allEnemy ){ finalTargets.AddRange( enemies ); }
            if( skillModel.allFriendly ){ finalTargets.AddRange( players ); }
            if( skillModel.friendly || skillModel.enemy ){
                Battle_Manager.taskManager.waitForTargetTask( skillModel, player, weaponSkill );
                return;
            } else {
                SkillComplete( skillModel, finalTargets, weaponSkill, player: player );
                return;
            }
        }

        public void SkillComplete( SkillModel skillModel, List<GameObject> targets, bool weaponSkill = true, GameObject player = null ){
            var power = 0.0f;
            var eM = new EventModel(){
                eventName = "OnSkillCast",
                eventCaller = gameObject
            };
            Battle_Manager.eventManager.BuildEvent( eM );
            if( skillModel.isFlat ){ 
                power = skillModel.isSpell ? skillModel.magicPower : skillModel.skillPower;
            } else {
                power = skillModel.isSpell ? skillModel.newMP : skillModel.newSP;
            }
            SkillData data = new SkillData(){ 
                target = targets,
                skillModel = skillModel
            };
            skillModel.RunExtraEffect(data); 
            SetAnimations( skillModel );
            DealHealDmg( skillModel, targets, power * data.modifier );
            SkillActiveSet( skillModel, false ); //Set that skill is ready to be used again
            for (int i = 0; i < Battle_Manager.battleInterfaceManager.Count; i++)
            {
                var iM = Battle_Manager.battleInterfaceManager[i];
                if( iM.skill == skillModel ) {
                    Battle_Manager.taskManager.skillcoolDownTask( skillModel, iM.skillCDImage );
                }
            }
            isSkillactive = false;
        }

        private void DealHealDmg( SkillModel skillModel, List<GameObject> targets, float power ){
            var bm = gameObject.GetComponent<Base_Character_Manager>();
            bm.damageManager.charDamageModel.dueDmgTargets = targets;
            foreach (var target in targets) {
                if ( skillModel.fxObject != null ){
                    bm.effectsManager.callEffectTarget( target, skillModel.fxObject );
                }
                if( target.tag == "Enemy" && bm.characterManager.characterModel.isAlive ){
                    foreach (var status in skillModel.singleStatusGroup) {
                        status.dispellable = skillModel.statusDispellable;
                    };
                    if( skillModel.doesDamage ){ 
                        var dmgModel = new DamageModel(baseManager){
                            incomingDmg = power,
                            skillModel = skillModel,
                            dmgSource = gameObject
                        };
                        bm.damageManager.calculatedamage( dmgModel ); 
                    };
                    skillModel.AttachStatus( skillModel.singleStatusGroup, target.GetComponent<Status_Manager>(), power, skillModel );
                } else if( target.tag == "Player" && bm.characterManager.characterModel.isAlive ){ 
                    foreach (var status in skillModel.singleStatusGroupFriendly) {
                        status.dispellable = skillModel.statusFriendlyDispellable;
                    }
                    if( skillModel.healsDamage ){ 
                        var dmgModel = new DamageModel(baseManager){
                            incomingHeal = power,
                            skillModel = skillModel,
                            dmgSource = gameObject
                        };
                        bm.damageManager.calculateHdamage( dmgModel ); 
                    };
                    skillModel.AttachStatus( skillModel.singleStatusGroupFriendly, target.GetComponent<Status_Manager>(), power, skillModel );
                }
            }
            bm.characterManager.characterModel.actionPoints -= skillModel.skillCost;
        }

        private void SetAnimations( SkillModel skillModel ){
            if( skillModel.animationType != null ){
                baseManager.animationManager.inAnimation = true;
                baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation);
                var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation).Animation.duration;
                baseManager.animationManager.SetBusyAnimation(animationDuration);
                if( skillModel.attackMovementSpeed > 0 ){
                    baseManager.autoAttackManager.isAttacking = true;
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                } else {
                    var idleAnim = baseManager.movementManager.idleAnim;
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0 );
                }
            }
            
        }

        public void SkillActiveSet( SkillModel skillModel, bool setActive, bool skillCancel = false ){
            if ( setActive || skillCancel ){ //turn skill active on begin but inactive on cancel
                skillModel.skillActive = setActive;
                skillInWaiting = skillModel;
            }
            if( !skillModel.enemySkill && Battle_Manager.taskManager.tasks["waitForTarget"] != null ){
                Battle_Manager.taskManager.tasks["waitForTarget"].Stop();
                Time.timeScale = 1f;
            } 
            if (!setActive || skillCancel ){ 
                finalTargets.Clear(); 
                waitingForSelection = false;
                skillInWaiting = null;
            }
        }

        private bool CheckSkillAvail( SkillModel skillModel ){
            var availActionPoints = baseManager.characterManager.characterModel.actionPoints;
            if ( availActionPoints >= skillModel.skillCost && !skillModel.skillActive )
            {
                return true;
            } else {
                print( skillModel.displayName + " is not available or waiting for target" );
                return false;
            }
        }

        //Enemy CD timer
        public void RefreshSkill( SkillModel skillModel ){
            //print ( "Refreshing " + skilllabel);
            if( skillModel.skillActive == false ){
                /*Battle_Manager.taskManager.CallTask( skillModel.skillCooldown, ()=> {
                    
                })*/
            }
        }

        SkillModel SkillToRun( List<SkillModel> bossSkillList ){
            phaseSkillList.Clear();
            bossSkillList.Capacity = bossSkillList.Count;
            for( int x = 0; x < bossSkillList.Count; x++ ){
                if( enemyPhase == EnemyPhase.phaseOne && bossSkillList[x].bossPhase == AssemblyCSharp.Skill_Manager.EnemyPhase.phaseOne && !bossSkillList[x].skillActive ){
                    phaseSkillList.Add( bossSkillList[x] );
                } else
                if( enemyPhase == EnemyPhase.phaseTwo && bossSkillList[x].bossPhase == AssemblyCSharp.Skill_Manager.EnemyPhase.phaseTwo && !bossSkillList[x].skillActive  ){
                    phaseSkillList.Add( bossSkillList[x] );
                } else
                if( enemyPhase == EnemyPhase.phaseThree && bossSkillList[x].bossPhase == AssemblyCSharp.Skill_Manager.EnemyPhase.phaseThree && !bossSkillList[x].skillActive  ){
                    phaseSkillList.Add( bossSkillList[x] );
                }
            }
            if( phaseSkillList.Count == 0 ){
                return null;
            }
            var randomNumber = Random.Range(0, (phaseSkillList.Count));
            var returnedSkill = phaseSkillList[randomNumber];
            return returnedSkill;
        }
            
        public void BeginSkillRotation( EnemyPhase phase ){
            //var boss = GameObject.FindGameObjectWithTag("Boss");
            var bossSkillList = enemySkillList;
            var randomSkill = SkillToRun( bossSkillList );
            if( !true ){
                Battle_Manager.taskManager.CallTask( 5f, () => {
                    BeginSkillRotation( phase );
                });
                return;
            }
                    //print( randomNumber );
            if( !isSkillactive && !baseManager.statusManager.DoesStatusExist( "stun" ) && !baseManager.autoAttackManager.isAttacking && enemyPhase == EnemyPhase.phaseOne && randomSkill != null ){
                    if( !isSkillactive && randomSkill.bossPhase == EnemyPhase.phaseOne ){
                        PrepSkillNew( randomSkill, false );
                        isSkillactive = true;
                        Battle_Manager.taskManager.CallTask( 10f, () => {
                            BeginSkillRotation( phase );
                        });
                    } else if ( isSkillactive ){
                        Battle_Manager.taskManager.CallTask( 1f, () => {
                            BeginSkillRotation( phase );
                        });
                    } else {
                        Battle_Manager.taskManager.CallTask( 1f, () => {
                            BeginSkillRotation( phase );
                        });
                    } 
            } 
            else {
                Battle_Manager.taskManager.CallTask( 5f, () => {
                    BeginSkillRotation( phase );
                });
            }
        }
    }
}

