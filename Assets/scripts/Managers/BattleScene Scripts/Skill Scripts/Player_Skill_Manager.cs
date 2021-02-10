using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
    public class Player_Skill_Manager : Skill_Manager
    {
        public enum weaponSlotEnum{
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot;
        public List<SkillModel> primaryWeaponSkills = new List<SkillModel>();
        public List<SkillModel> secondaryWeaponSkills = new List<SkillModel>();
        public SkillModel skillModel;
        public bool waitingForSelection;
        public int turnsTaken = 0;

        void Start()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if (gameObject.tag == "Player")
            {
                CalculateSkillPower();
                CalculateMagicPower();
            }
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillTrigger;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillComplete;
            }
            if (Battle_Manager.characterSelectManager.characterSelected.ToString() == (gameObject.name.ToLower() + "Selected"))
            {
                Battle_Manager.GetBattleInterfaces().ForEach(o => o.SkillSet((Player_Skill_Manager)baseManager.skillManager));
            }
        }

        public void PrepSkill(SkillModel skillModel, bool weaponSkill = true)
        {
            isSkillactive = true;
            if (CheckSkillAvail(skillModel) && skillModel.castTurnTime <= 0)
            {
                Battle_Manager.battleDetailsManager.BattleWarning($"{gameObject.name} casts {skillModel.skillName}", 3f);
                SkillActiveSet(skillModel, true);
                GetTargets(skillModel, weaponSkill: weaponSkill);
            }
            else if(CheckSkillAvail(skillModel))
            {
                Battle_Manager.battleDetailsManager.BattleWarning($"{gameObject.name} is casting {skillModel.skillName} in {skillModel.castTurnTime} turns", 3f);
                SkillActiveSet(skillModel, true);
                StartCasting(skillModel);
            }
        }

        public void CalculateSkillPower()
        {
            for (int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newSP = primaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
                secondaryWeaponSkills[i].newSP = secondaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
            }
            skillModel.newSP = skillModel.skillPower * baseManager.characterManager.characterModel.PAtk;
        }

        public void CalculateMagicPower()
        {
            for (int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newMP = primaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
                secondaryWeaponSkills[i].newMP = secondaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
            }
            skillModel.newMP = skillModel.magicPower * baseManager.characterManager.characterModel.MAtk;
        }
        private void GetTargets(SkillModel skillModel, bool weaponSkill = true)
        {
            finalTargets.Clear();
            var player = this.gameObject;
            var enemyPlayers = Battle_Manager.GetCharacterManagers(false);
            var friendlyPlayers = Battle_Manager.GetCharacterManagers(true).Where(o => o.name != gameObject.name).ToList(); ;
            if (skillModel.self) { finalTargets.Add(baseManager.characterManager); }
            if (skillModel.allEnemy) { finalTargets.AddRange(enemyPlayers); }
            if (skillModel.allFriendly) { finalTargets.AddRange(friendlyPlayers); }
            if (skillModel.friendly || skillModel.enemy)
            {
                Battle_Manager.taskManager.waitForTargetTask(player, classSkill: skillModel, weaponSkill: weaponSkill, skillAction: () =>
                {
                    //baseManager.characterManager.characterModel.target = target;
                    Time.timeScale = 1f;
                    SetAnimations(skillModel);
                    //SkillComplete(finalTargets, skillModel: skillModel);
                }
                );
                return;
            }
            else
            {
                if (!baseManager.statusManager.DoesStatusExist("stun"))
                {
                    finalTargets.Capacity = finalTargets.Count;
                    SetAnimations(skillModel);
                    //SkillComplete(finalTargets, skillModel: skillModel);
                }
                else
                {
                    SkillActiveSet(skillModel, false);
                    isSkillactive = false;
                }
            }
        }

        public void SkillComplete(List<Character_Manager> targets, SkillModel skillModel)
        {
            var power = 0.0f;
            var eM = new EventModel()
            {
                eventName = "OnSkillCast",
                eventCaller = baseManager.characterManager
            };
            Battle_Manager.eventManager.BuildEvent(eM);
            if (skillModel != null && skillModel.isFlat)
            {
                power = skillModel.isSpell ? skillModel.magicPower : skillModel.skillPower;
            }
            else
            {
                power = skillModel.isSpell ? skillModel.newMP : skillModel.newSP;

            }

            DealHealDmg(skillModel, targets, power);
            ++turnsTaken;
            skillModel.SaveTurnToReset();
            isSkillactive = false;
            currenttarget = null;
            skillModel.ResetSkillOnCurrentTurn(true, () =>
            {
                SkillActiveSet(skillModel, false);
            });

            /*for (int i = 0; i < Battle_Manager.battleInterfaceManager.Count; i++)
            {
                var iM = Battle_Manager.battleInterfaceManager[i];
                if (iM.skill == skillModel)
                {
                    skillModel.SaveTurnToReset();
                    skillModel.ResetSkillOnCurrentTurn(true, () =>
                    {
                        SkillActiveSet(skillModel, false);
                    });
                    //Battle_Manager.taskManager.skillcoolDownTask(skillModel, iM.skillCDImage);
                }
            }*/
        }

        private void DealHealDmg(SkillModel skillModel, List<Character_Manager> targets, float power)
        {
            foreach (var status in skillModel.singleStatusGroup)
            {
                status.dispellable = skillModel.statusDispellable;
            };
            foreach (var target in targets)
            {
                SkillData data = new SkillData()
                {
                    target = target,
                    caster = baseManager.characterManager,
                    skillModel = skillModel
                };
                skillModel.RunExtraEffect(data);
                if (skillModel.fxObject != null)
                {
                    baseManager.effectsManager.callEffectTarget(target, skillModel.fxObject);
                }
                if (skillModel.doesDamage)
                {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingDmg = skillModel.useModifier ? power + (power * skillModel.modifierAmount) : power,
                        skillModel = skillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier
                    };
                    if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                    {
                        target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                    }
                    target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                    target.baseManager.damageManager.calculatedamage(dmgModel);
                };
                if (skillModel.healsDamage)
                {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingHeal = skillModel.useModifier ? power + (power * skillModel.    modifierAmount) : power,
                        skillModel = skillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier
                    };
                    target.baseManager.damageManager.calculateHdamage(dmgModel);
                };
                AddStatuses(target, power, skillModel);
            }
            Battle_Manager.actionPoints -= skillModel.skillCost;
            Battle_Manager.UpdateAPAmount();
        }


        private void AddStatuses(Character_Manager target, float power, SkillModel skillModel)
        {
            if (target.characterModel.characterType == baseManager.characterManager.characterModel.characterType)
            {
                var hitAnimation = skillModel.singleStatusGroupFriendly.Where(o => !string.IsNullOrEmpty(o.hitAnim)).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = skillModel.singleStatusGroupFriendly.Where(o => !string.IsNullOrEmpty(o.holdAnim)).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                skillModel.AttachStatus(skillModel.singleStatusGroupFriendly, target.baseManager, power, skillModel);
            }
            else
            {
                var hitAnimation = skillModel.singleStatusGroup.Where(o => !string.IsNullOrEmpty(o.hitAnim)).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = skillModel.singleStatusGroup.Where(o => !string.IsNullOrEmpty(o.holdAnim)).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                skillModel.AttachStatus(skillModel.singleStatusGroup, target.baseManager, power, skillModel);
            }
        }

        private void SetAnimations(SkillModel skillModel)
        {
            if (skillModel.animationType != null)
            {
                baseManager.animationManager.inAnimation = true;
                var animationDuration = 0f;
                if (skillModel.castTurnTime > 0)
                {
                    animationDuration = skillModel.castTurnTime;
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationCastingType, false);
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.animationRepeatCasting, true, 0);
                }
                else
                {
                    animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation).Animation.Duration;
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationType, skillModel.loopAnimation);
                }
                baseManager.animationManager.SetBusyAnimation(animationDuration);

                if (skillModel.attackMovementSpeed > 0)
                {
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                }
                else
                {
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0);
                }
                /*Battle_Manager.taskManager.CallTask(animationDuration, () =>
                {
                    SkillActiveSet(skillModel, false);
                    //isSkillactive = false;
                });*/
            }
        }

        public void SkillActiveSet(SkillModel skillModel, bool setActive)
        {
            skillModel.skillActive = setActive;
            activeSkill = setActive ? skillModel : null;
            if (Battle_Manager.taskManager.taskList.Count > 0 && Battle_Manager.taskManager.taskList.ContainsKey("waitForTarget"))
            {
                Battle_Manager.taskManager.taskList["waitForTarget"].Stop();
                Time.timeScale = 1f;
            }
            if (!setActive)
            {
                finalTargets.Clear();
            }
        }

        private bool CheckSkillAvail(SkillModel skillModel)
        {
            var availActionPoints = Battle_Manager.actionPoints;
            if (availActionPoints >= skillModel.skillCost && !skillModel.skillActive)
            {
                return true;
            }
            else
            {
                print(skillModel.skillName + " is not available or waiting for target");
                return false;
            }
        }

        public void StartCasting(SkillModel skillModel)
        {
            baseManager.animationManager.inAnimation = true;
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationCastingType, false);
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.animationRepeatCasting, true, 0);
            //New Turn based cast
            if (CompleteSkillOnCurrentTurn() && Battle_Manager.turn == Battle_Manager.TurnEnum.PlayerTurn )
            {
                GetTargets(skillModel);
            }

            //Old time based cast
            /*Battle_Manager.taskManager.CallTask(skillModel.castTurnTime, () =>
            {
                GetTargets(skillModel);
            });*/
        }

        public void OnEventSkillTrigger(Spine.TrackEntry state, Spine.Event e)
        {
            if ((e.Data.Name == "hit" || e.Data.Name == "triggerEvent") && isSkillactive)
            {
                SkillComplete(finalTargets, (SkillModel)activeSkill);
                foreach (var target in finalTargets)
                {
                    var targetDamageManager = target.baseManager.damageManager;
                    var damageModel = targetDamageManager.skillDmgModels.ContainsKey(gameObject.name) ? targetDamageManager.skillDmgModels[gameObject.name] : null;
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
    }
}

