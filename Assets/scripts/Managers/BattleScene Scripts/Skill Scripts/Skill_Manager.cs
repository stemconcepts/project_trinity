using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using static AssemblyCSharp.PhaseManager;

namespace AssemblyCSharp
{
    public class Skill_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        private float multiplier;
        public List<Character_Manager> finalTargets = new List<Character_Manager>();
        public bool isSkillactive;
        public Character_Manager currenttarget;
        //private SkillModel skillInWaiting;
        public enum weaponSlotEnum{
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot;
        public List<SkillModel> primaryWeaponSkills = new List<SkillModel>();
        public List<SkillModel> secondaryWeaponSkills = new List<SkillModel>();
        public SkillModel skillModel;
        public bool waitingForSelection;
        //public DamageModel dmgModel;

        //Enemy specific properties
        //public EnemyPhase enemyPhase;
        public List<enemySkill> enemySkillList = new List<enemySkill>();
        private List<enemySkill> copiedSkillList = new List<enemySkill>();
        public List<enemySkill> phaseSkillList = new List<enemySkill>();

        public void RefreshSkillList(List<enemySkill> skillList)
        {
            copiedSkillList.Clear();
            foreach (var skill in skillList)
            {
                copiedSkillList.Add(Object.Instantiate(skill));
            };
        }

        void Start()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if (this.gameObject.tag == "Enemy")
            {
                RefreshSkillList(baseManager.phaseManager.GetPhaseSkills());
                //enemySkillList = baseManager.phaseManager.GetPhaseSkills();
            }
            /*foreach(var skill in enemySkillList)
            {
                copiedSkillList.Add(Object.Instantiate(skill));
            };*/
            if (gameObject.tag == "Player")
            {
                CalculateSkillPower();
                CalculateMagicPower();
            }
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillHit;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillComplete;
            }
            if (Battle_Manager.characterSelectManager.characterSelected.ToString() == (gameObject.name.ToLower() + "Selected"))
            {
                Battle_Manager.GetBattleInterfaces().ForEach(o => o.SkillSet(baseManager.skillManager));
            }
            Battle_Manager.taskManager.CallTask( 5f, () => {
                if(copiedSkillList.Count > 0 ){
                    BeginSkillRotation();
                }
            });
        }

        public void PrepSkill(SkillModel skillModel, bool weaponSkill = true ){
            if( CheckSkillAvail( skillModel ) ){
                if( !waitingForSelection ){
                    isSkillactive = true;
                    SkillActiveSet( skillModel, true );
                    GetTargets(skillModel, weaponSkill: weaponSkill);
                }
            }
        }

        //for enemies
        public void PrepSkill(enemySkill skillModel)
        {
            SkillActiveSet(skillModel, true);
            if (skillModel.castTime <= 0)
            {
                GetTargets(skillModel);
            }
            else
            {
                StartCasting(skillModel);
            }
        }

        private void ForcedMove(Base_Character_Manager target, enemySkill skillModel)
        {
            var movementScript = target.movementManager;
            movementScript.ForcedMove(skillModel.forcedMove.ToString(), skillModel.forcedMoveAmount);
        }

        public void CalculateSkillPower(){
            for(int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newSP = primaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
                secondaryWeaponSkills[i].newSP = secondaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
            }
            skillModel.newSP = skillModel.skillPower * baseManager.characterManager.characterModel.PAtk;
        }
        
        public void CalculateMagicPower(){
            for(int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newMP = primaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
                secondaryWeaponSkills[i].newMP = secondaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
            }
            skillModel.newMP = skillModel.magicPower * baseManager.characterManager.characterModel.MAtk;
        }
        private void GetTargets( SkillModel skillModel, bool randomTarget = false, bool weaponSkill = true ){
            finalTargets.Clear();
            var player = this.gameObject;
            var enemyPlayers = Battle_Manager.GetCharacterManagers(false);
            var friendlyPlayers = Battle_Manager.GetCharacterManagers(true);
            if( skillModel.self ){ finalTargets.Add( baseManager.characterManager ); }
            if( skillModel.allEnemy ){ finalTargets.AddRange( enemyPlayers ); }
            if( skillModel.allFriendly ){ finalTargets.AddRange( friendlyPlayers ); }
            if( skillModel.friendly || skillModel.enemy ){
                Battle_Manager.taskManager.waitForTargetTask( player, classSkill: skillModel, weaponSkill: weaponSkill, skillAction: () =>
                    {
                        //baseManager.characterManager.characterModel.target = target;
                        Time.timeScale = 1f;
                        SkillComplete(finalTargets, skillModel: skillModel, weaponSkill: weaponSkill);
                    }
                );
                return;
            } else {
                if (!baseManager.statusManager.DoesStatusExist("stun"))
                {
                    finalTargets.Capacity = finalTargets.Count;
                    SkillComplete(finalTargets, weaponSkill: weaponSkill, skillModel: skillModel);
                }
                else
                {
                    isSkillactive = false;
                }
            }
        }

        //for enemies
        private void GetTargets(enemySkill skillModel, bool randomTarget = false)
        {
            finalTargets.Clear();
            var enemyPlayers = Battle_Manager.GetCharacterManagers(true);
            var friendlyPlayers = Battle_Manager.GetCharacterManagers(false).Where(o => o.name != gameObject.name).ToList();
            if (skillModel.self) { finalTargets.Add(baseManager.characterManager); }
            if (skillModel.allFriendly) { finalTargets.AddRange(friendlyPlayers); }
            if (skillModel.allEnemy) { finalTargets.AddRange(enemyPlayers); }
            if (skillModel.friendly)
            {
                finalTargets.Add(baseManager.characterManager.GetFriendlyTarget().characterManager);
            }
            else if(skillModel.enemy)
            {
                finalTargets.Add(baseManager.characterManager.GetTarget().characterManager);
            }
            finalTargets.Capacity = finalTargets.Count;
            if (!baseManager.statusManager.DoesStatusExist("stun"))
            {
                SkillComplete(finalTargets, enemySkillModel: skillModel);
            } else
            {
                isSkillactive = false;
            }
        }

        public void SkillComplete(List<Character_Manager> targets, SkillModel skillModel = null, enemySkill enemySkillModel = null, bool weaponSkill = true){
            var power = 0.0f;
            var eM = new EventModel(){
                eventName = "OnSkillCast",
                eventCaller = baseManager.characterManager
            };
            Battle_Manager.eventManager.BuildEvent( eM );
            if( (skillModel != null && skillModel.isFlat) || (enemySkillModel != null && enemySkillModel.isFlat))
            {
                if (skillModel != null)
                {
                    power = skillModel.isSpell ? skillModel.magicPower : skillModel.skillPower;
                } else
                {
                    power = enemySkillModel.isSpell ? enemySkillModel.magicPower : enemySkillModel.skillPower;
                }
            } else {
                if (skillModel != null)
                {
                    power = skillModel.isSpell ? skillModel.newMP : skillModel.newSP;
                }
                else
                {
                    power = enemySkillModel.isSpell ? enemySkillModel.newMP : enemySkillModel.newSP;
                }
            }
            if (skillModel != null)
            {
                SetAnimations(skillModel);
                DealHealDmg(skillModel, targets, power);
                SkillActiveSet(skillModel, false);
                for (int i = 0; i < Battle_Manager.battleInterfaceManager.Count; i++)
                {
                    var iM = Battle_Manager.battleInterfaceManager[i];
                    if (iM.skill == skillModel)
                    {
                        Battle_Manager.taskManager.skillcoolDownTask(skillModel, iM.skillCDImage);
                    }
                }
            } else
            {
                SetAnimations(enemySkillModel);
                DealHealDmg(enemySkillModel, targets, power);
                Battle_Manager.taskManager.CallTask(enemySkillModel.skillCooldown, () =>
                {
                    SkillActiveSet(enemySkillModel, false);
                });
            }
            currenttarget = null;
        }

        public void StartCasting(enemySkill skillModel)
        {
            Battle_Manager.taskManager.CallTask(skillModel.castTime, () =>
            {
                GetTargets(skillModel);
            });
            //call voidzone if true
            if (skillModel.hasVoidzone)
            {
                /*voidzoneData voidData = new voidzoneData();
                voidData.voidZoneDuration = skillModel.castTime;*/
                skillModel.ShowVoidPanel(skillModel.voidZoneTypes, skillModel.monsterPanel);
            }
            //call animation variable
            baseManager.animationManager.inAnimation = true;
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationCastingType, false);
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.animationRepeatCasting, true, 0);
        }

        private void DealHealDmg( SkillModel skillModel, List<Character_Manager> targets, float power ){
            //baseManager.damageManager.charDamageModel.dueDmgTargets = targets;
            foreach (var status in skillModel.singleStatusGroup) {
                status.dispellable = skillModel.statusDispellable;
            };
            foreach (var target in targets) {
                SkillData data = new SkillData() {
                    target = target,
                    caster = baseManager.characterManager,
                    skillModel = skillModel,
                    //modifier = skillModel.modifierAmount
                };
                skillModel.RunExtraEffect(data);
                if ( skillModel.fxObject != null ){
                    baseManager.effectsManager.callEffectTarget( target, skillModel.fxObject );
                }
                if( skillModel.doesDamage ){ 
                        var dmgModel = new DamageModel(){
                            baseManager = target.baseManager,
                            incomingDmg = power + (power * skillModel.modifierAmount),
                            skillModel = skillModel,
                            dmgSource = baseManager.characterManager,
                            dueDmgTargets = targets,
                            hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                            modifiedDamage = skillModel.modifierAmount > 0f
                        };
                    target.baseManager.damageManager.skillDmgModels.Add(gameObject.name,dmgModel);
                    target.baseManager.damageManager.calculatedamage(dmgModel);
                };
                if( skillModel.healsDamage ){ 
                        var dmgModel = new DamageModel()
                        {
                            baseManager = target.baseManager,
                            incomingHeal = power + (power * skillModel.modifierAmount),
                            skillModel = skillModel,
                            dmgSource = baseManager.characterManager,
                            dueDmgTargets = targets,
                            hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                            modifiedDamage = skillModel.modifierAmount > 0f
                        };
                    target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                    target.baseManager.damageManager.calculateHdamage( dmgModel ); 
                };
                AddStatuses(target, power, skillModel);
            }
            baseManager.characterManager.characterModel.actionPoints -= skillModel.skillCost;
            baseManager.characterManager.UpdateAPAmount();
        }

        private void DealHealDmg(enemySkill enemySkillModel, List<Character_Manager> targets, float power)
        {
            //baseManager.damageManager.charDamageModel.dueDmgTargets = targets;
            foreach (var status in enemySkillModel.singleStatusGroup)
            {
                status.dispellable = enemySkillModel.statusDispellable;
            };
            foreach (var target in targets)
            {
                SkillData data = new SkillData()
                {
                    target = target,
                    caster = baseManager.characterManager,
                    enemySkillModel = enemySkillModel,
                    //modifier = enemySkillModel.modifierAmount == 0 ? 1 : enemySkillModel.modifierAmount
                };
                enemySkillModel.RunExtraEffect(data);
                if (enemySkillModel.doesDamage)
                {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingDmg = power + (power * enemySkillModel.modifierAmount),
                        enemySkillModel = enemySkillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = enemySkillModel.modifierAmount > 0f
                    };
                    if (enemySkillModel.hasVoidzone)
                    {
                        var tankData = targets.Where(o => o.characterModel.role == Character_Model.RoleEnum.tank).First();
                        if (!tankData.characterModel.inVoidCounter)
                        {
                            if (enemySkillModel.fxObject != null)
                            {
                                baseManager.effectsManager.callEffectTarget(target, enemySkillModel.fxObject);
                            }
                            target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                            target.baseManager.damageManager.calculatedamage(dmgModel);
                        } else
                        {
                            if (target.characterModel.role == Character_Model.RoleEnum.tank)
                            {
                                finalTargets = new List<Character_Manager>()
                                {
                                    target
                                };
                                if (enemySkillModel.fxObject != null)
                                {
                                    baseManager.effectsManager.callEffectTarget(target, enemySkillModel.fxObject);
                                }
                                dmgModel.modifiedDamage = true;
                                dmgModel.incomingDmg = power + (power * 0.5f);
                                tankData.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                                tankData.baseManager.damageManager.calculatedamage(dmgModel);
                            }
                        }
                    } else
                    {
                        if (enemySkillModel.fxObject != null)
                        {
                            baseManager.effectsManager.callEffectTarget(target, enemySkillModel.fxObject);
                        }
                        target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                        target.baseManager.damageManager.calculatedamage(dmgModel);
                    }
                };
                if (enemySkillModel.healsDamage)
                {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingHeal = power * (power * enemySkillModel.modifierAmount),
                        enemySkillModel = enemySkillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = enemySkillModel.modifierAmount > 0f
                    };
                    target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                    target.baseManager.damageManager.calculateHdamage(dmgModel);
                };
                AddStatuses(target, power, enemySkillModel);
            }
            Battle_Manager.ClearAllVoidZones();
        }

        private void AddStatuses(Character_Manager target, float power, SkillModel skillModel){
            if (target.characterModel.characterType == baseManager.characterManager.characterModel.characterType){
                var hitAnimation = skillModel.singleStatusGroupFriendly.Where(o => !string.IsNullOrEmpty(o.hitAnim)).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = skillModel.singleStatusGroupFriendly.Where(o => !string.IsNullOrEmpty(o.holdAnim)).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                skillModel.AttachStatus( skillModel.singleStatusGroupFriendly, target.baseManager, power, skillModel );
            } else {
                var hitAnimation = skillModel.singleStatusGroup.Where(o => !string.IsNullOrEmpty(o.hitAnim)).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = skillModel.singleStatusGroup.Where(o => !string.IsNullOrEmpty(o.holdAnim)).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                skillModel.AttachStatus( skillModel.singleStatusGroup, target.baseManager, power, skillModel );
            }        
        }

        private void AddStatuses(Character_Manager target, float power, enemySkill enemySkill)
        {
            if (target.tag == gameObject.tag)
            {
                enemySkill.AttachStatus(enemySkill.singleStatusGroupFriendly, target.baseManager, power, enemySkill);
            }
            else
            {
                enemySkill.AttachStatus(enemySkill.singleStatusGroup, target.baseManager, power, enemySkill);
            }
        }

        private void SetAnimations( SkillModel skillModel ){
            if( skillModel.animationType != null ){
                baseManager.animationManager.inAnimation = true;
                var animationDuration = 0f;
                if (skillModel.castTime > 0)
                {
                    animationDuration = skillModel.castTime;
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationCastingType, false);
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.animationRepeatCasting, true, 0);
                } else
                {
                    animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation).Animation.Duration;
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation);
                }
                baseManager.animationManager.SetBusyAnimation(animationDuration);

                if ( skillModel.attackMovementSpeed > 0 ){
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                } else {
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0 );
                }
                Battle_Manager.taskManager.CallTask(animationDuration, () =>
                {
                    isSkillactive = false;
                });
            }
        }

        private void SetAnimations(enemySkill skillModel)
        {
            if (skillModel.animationType != null)
            {
                baseManager.animationManager.inAnimation = true;
                baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation);
                var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation).Animation.Duration;
                baseManager.animationManager.SetBusyAnimation(animationDuration);
                if (skillModel.attackMovementSpeed > 0)
                {
                    //baseManager.autoAttackManager.isAttacking = true;
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                }
                else
                {
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0);
                }
                Battle_Manager.taskManager.CallTask(animationDuration, () =>
                {
                    isSkillactive = false;
                });
            }
        }

        public void SkillActiveSet( SkillModel skillModel, bool setActive, bool skillCancel = false ){
            if ( setActive || skillCancel ){ //turn skill active on begin but inactive on cancel
                skillModel.skillActive = setActive;
                // skillInWaiting = skillModel;
            }
            if(Battle_Manager.taskManager.taskList.Count > 0 && Battle_Manager.taskManager.taskList.ContainsKey("waitForTarget") ){
                Battle_Manager.taskManager.taskList["waitForTarget"].Stop();
                Time.timeScale = 1f;
            } 
            if (!setActive || skillCancel ){ 
                //finalTargets.Clear(); 
                waitingForSelection = false;
                //skillInWaiting = null;
            }
        }

        //for enemies
        public void SkillActiveSet(enemySkill skillModel, bool setActive)
        {
            if (copiedSkillList.Count > 0)
            {
                copiedSkillList.Where(o => o.skillName == skillModel.skillName).First().skillActive = setActive;
            }
            skillModel.skillActive = setActive;
        }

        private bool CheckSkillAvail( SkillModel skillModel ){
            var availActionPoints = baseManager.characterManager.characterModel.actionPoints;
            if ( availActionPoints >= skillModel.skillCost && !skillModel.skillActive )
            {
                return true;
            } else {
                print( skillModel.skillName + " is not available or waiting for target" );
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

        enemySkill SkillToRun( List<enemySkill> bossSkillList ){
            phaseSkillList.Clear();
            bossSkillList.Capacity = bossSkillList.Count;
            for( int x = 0; x < bossSkillList.Count; x++ ){
                if (!bossSkillList[x].skillActive)
                {
                    phaseSkillList.Add(Object.Instantiate(bossSkillList[x]) as enemySkill);
                }
                /*if( enemyPhase == EnemyPhase.phaseOne && bossSkillList[x].phaseOne && !bossSkillList[x].skillActive ){
                    phaseSkillList.Add( Object.Instantiate(bossSkillList[x]) as enemySkill);
                } else
                if( enemyPhase == EnemyPhase.phaseTwo && bossSkillList[x].phaseTwo && !bossSkillList[x].skillActive  ){
                    phaseSkillList.Add( Object.Instantiate(bossSkillList[x]) as enemySkill);
                } else
                if( enemyPhase == EnemyPhase.phaseThree && bossSkillList[x].phaseThree && !bossSkillList[x].skillActive  ){
                    phaseSkillList.Add( Object.Instantiate(bossSkillList[x]) as enemySkill);
                }*/
            }
            if( phaseSkillList.Count == 0 ){
                return null;
            }
            if( phaseSkillList.Count > 0 ){
                phaseSkillList.ForEach( o => o.newSP = o.skillPower * baseManager.characterManager.characterModel.PAtk );
                phaseSkillList.ForEach( o => o.newMP = o.magicPower * baseManager.characterManager.characterModel.MAtk );
            }
            var randomNumber = Random.Range(0, (phaseSkillList.Count));
            var returnedSkill = phaseSkillList[randomNumber];
            return returnedSkill;
        }
            
        public void BeginSkillRotation( /*EnemyPhase phase*/ ){
            var randomSkill = SkillToRun(copiedSkillList);
            if( !isSkillactive && !baseManager.statusManager.DoesStatusExist( "stun" ) && !baseManager.autoAttackManager.isAttacking && randomSkill != null ){
                    if( !isSkillactive /*&& GetBossPhase(randomSkill) == enemyPhase*/ ){
                        isSkillactive = true;
                        PrepSkill( randomSkill );
                        Battle_Manager.taskManager.CallTask( 10f, () => {
                            BeginSkillRotation();
                        });
                    } else if ( isSkillactive ){
                        Battle_Manager.taskManager.CallTask( 1f, () => {
                            BeginSkillRotation();
                        });
                    } else {
                        Battle_Manager.taskManager.CallTask( 1f, () => {
                            BeginSkillRotation();
                        });
                    } 
            } 
            else {
                Battle_Manager.taskManager.CallTask( 5f, () => {
                    BeginSkillRotation();
                });
            }
        }
        /*public EnemyPhase GetBossPhase(enemySkill enemySkill)
        {
            if (enemySkill.phaseOne)
            {
                return EnemyPhase.phaseOne;
            }
            else if (enemySkill.phaseTwo)
            {
                return EnemyPhase.phaseTwo;
            }
            else if (enemySkill.phaseTwo)
            {
                return EnemyPhase.phaseThree;
            }
            return EnemyPhase.phaseOne;
        }*/

        public void OnEventSkillHit(Spine.TrackEntry state, Spine.Event e)
        {
            if ( (e.Data.Name == "hit" || e.Data.Name == "SFXhit") && isSkillactive)
            {
                foreach (var target in finalTargets)
                {
                    var targetDamageManager = target.baseManager.damageManager;
                    var damageModel = targetDamageManager.skillDmgModels[gameObject.name];
                    if (damageModel != null)
                    {
                        targetDamageManager.TakeDmg(damageModel, e.Data.Name);
                        var eventModel = new EventModel
                        {
                            eventName = "OnDealingDmg",
                            extTarget = target,
                            eventCaller = baseManager.characterManager,
                            extraInfo = damageModel.damageTaken
                        };
                        Battle_Manager.eventManager.BuildEvent(eventModel);
                    }
                }
            }
        }

        public void OnEventSkillComplete(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "endEvent")
            {
                foreach (var target in finalTargets)
                {
                    var targetDamageManager = target.baseManager.damageManager;
                    if (targetDamageManager.skillDmgModels.ContainsKey(gameObject.name))
                    {
                        targetDamageManager.skillDmgModels.Remove(gameObject.name);
                    }
                }
            }
        }
    }
}

