﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
    public class PlayerSkillManager : BaseSkillManager
    {
        public enum weaponSlotEnum {
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot;
        public List<SkillModel> primaryWeaponSkills = new List<SkillModel>();
        public List<SkillModel> secondaryWeaponSkills = new List<SkillModel>();
        public SkillModel skillModel;
        public bool waitingForSelection;
        public int turnsTaken = 0;
        public GenericSkillModel activeSkill;

        void Start()
        {
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            if (gameObject.tag == "Player")
            {
                CalculateSkillPower();
                CalculateMagicPower();
            }
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillTrigger;
                //baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillComplete;
                //baseManager.animationManager.skeletonAnimation.state.Event += Battle_Manager.soundManager.OnEventHit;
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(o => {
                    o.state.Event += OnEventSkillTrigger;
                });
            }
            if (BattleManager.characterSelectManager.characterSelected.ToString() == (gameObject.name.ToLower() + "Selected"))
            {
                BattleManager.GetBattleInterfaces().ForEach(o => o.SkillSet((PlayerSkillManager)baseManager.skillManager));
            }
        }

        public void PrepSkill(SkillModel skillModel, bool weaponSkill = true)
        {
            isSkillactive = true;
            if (CheckSkillAvail(skillModel) && skillModel.castTurnTime <= 0)
            {
                skillModel.skillActive = true;
                BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} casts {skillModel.skillName}", 3f);
                SkillActiveSet(skillModel, true);
                GetTargets(skillModel, weaponSkill: weaponSkill);
            }
            else if(CheckSkillAvail(skillModel))
            {
                skillModel.skillActive = true;
                BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} is casting {skillModel.skillName} in {skillModel.castTurnTime} turns", 3f);
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
            var enemyPlayers = BattleManager.GetEnemyCharacterManagers();
            var friendlyPlayers = BattleManager.GetFriendlyCharacterManagers();
            if (skillModel.self) { finalTargets.Add(baseManager.characterManager as CharacterManager); }
            if (skillModel.allEnemy) {
                finalTargets.AddRange(enemyPlayers);
                currenttarget = enemyPlayers[Random.Range(0, enemyPlayers.Count())];
            }
            if (skillModel.allFriendly) { finalTargets.AddRange(friendlyPlayers); }
            if (skillModel.friendly || skillModel.enemy)
            {
                //Battle_Manager.gameEffectManager.DrawLineFromMouseToPoint(baseManager.movementManager.currentPosition);
                BattleManager.taskManager.waitForTargetTask(player, classSkill: skillModel, weaponSkill: weaponSkill, skillAction: () =>
                {
                    //baseManager.characterManager.characterModel.target = target;
                    Time.timeScale = 1f;
                    SetAnimations(skillModel);
                    //SkillComplete(finalTargets, skillModel: skillModel);
                });
                BattleManager.actionPoints -= skillModel.skillCost;
                BattleManager.UpdateAPAmount();
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
            BattleManager.actionPoints -= skillModel.skillCost;
            BattleManager.UpdateAPAmount();
        }

        public void SkillComplete(List<BaseCharacterManager> targets, SkillModel skillModel)
        {
            var power = 0.0f;
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

            var eM = new EventModel()
            {
                eventName = "OnSkillCast",
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eM);
        }

        private void DealHealDmg(SkillModel skillModel, List<BaseCharacterManager> targets, float power)
        {
            foreach (var status in skillModel.singleStatusGroup)
            {
                status.dispellable = skillModel.statusDispellable;
            };

            foreach (var status in skillModel.singleStatusGroupFriendly)
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
                var didHit = target.GetChanceToBeHit(baseManager.characterManager.characterModel.accuracy, baseManager.characterManager.characterModel.evasion);
                skillModel.RunExtraEffect(data);
                if (skillModel.doesDamage)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingDmg = skillModel.useModifier ? power + (power * skillModel.modifierAmount) : power,
                        skillModel = skillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier
                    };
                    if (skillModel.isSpell || didHit)
                    {
                        if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                        {
                            target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                        }
                        target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                        target.baseManager.damageManager.calculatedamage(dmgModel);
                        /*if (skillModel.singleStatusGroup.Count() > 0)
                        {
                            AddStatuses(target, power, skillModel);
                        }*/
                    } else
                    {
                        dmgModel.incomingDmg = 0;
                        dmgModel.showDmgNumber = false;
                        BattleManager.soundManager.playSound("miss");
                        BattleManager.battleDetailsManager.ShowDamageNumber(dmgModel, extraInfo: "Miss");
                    }
                };
                if (skillModel.healsDamage)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingHeal = skillModel.useModifier ? power + (power * skillModel.modifierAmount) : power,
                        skillModel = skillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier
                    };
                    target.baseManager.damageManager.calculateHdamage(dmgModel);
                };
                if (skillModel.isSpell || didHit || skillModel.healsDamage)
                {
                    if (skillModel.fxObject != null)
                    {
                        baseManager.effectsManager.callEffectTarget(target, skillModel.fxObject);
                    }
                }
                if (skillModel.singleStatusGroupFriendly.Count() > 0 || (skillModel.isSpell || didHit) && skillModel.singleStatusGroup.Count() > 0)
                {
                    AddStatuses(target, power, skillModel);
                }
            }
            if (skillModel.castSound != null)
            {
                BattleManager.soundManager.playSoundUsingAudioSource(skillModel.castSound, baseManager.gameObject.GetComponent<AudioSource>());
            }
            else if (skillModel.isSpell)
            {
                BattleManager.soundManager.playSounds(BattleManager.soundManager.magicChargeSounds);
            }
        }


        private void AddStatuses(BaseCharacterManager target, float power, SkillModel skillModel)
        {
            if (target.characterModel.characterType == baseManager.characterManager.characterModel.characterType)
            {
                var hitAnimation = skillModel.singleStatusGroupFriendly.Where(o => o.hitAnim != animationOptionsEnum.none).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = skillModel.singleStatusGroupFriendly.Where(o => o.holdAnim != animationOptionsEnum.none).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                skillModel.AttachStatus(skillModel.singleStatusGroupFriendly, target.baseManager, power, skillModel);
            }
            else
            {
                var hitAnimation = skillModel.singleStatusGroup.Where(o => o.hitAnim != animationOptionsEnum.none).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = skillModel.singleStatusGroup.Where(o => o.holdAnim != animationOptionsEnum.none).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                skillModel.AttachStatus(skillModel.singleStatusGroup, target.baseManager, power, skillModel);
            }
        }

        private void SetAnimations(SkillModel skillModel)
        {
            if (skillModel.EndAnimation != animationOptionsEnum.none)
            {
                baseManager.animationManager.inAnimation = true;
                var animationDuration = 0f;
                if (skillModel.castTurnTime > 0)
                {
                    animationDuration = skillModel.castTurnTime;

                    baseManager.animationManager.PlaySetAnimation(skillModel.BeginCastingAnimation.ToString(), false);
                    baseManager.animationManager.PlayAddAnimation(skillModel.CastingAnimation.ToString(), true);
                    //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.BeginCastingAnimation.ToString(), false);
                    //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.CastingAnimation.ToString(), true, 0);
                }
                else
                {
                    animationDuration = baseManager.animationManager.PlaySetAnimation(skillModel.EndAnimation.ToString(), skillModel.loopAnimation);

                    //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.EndAnimation.ToString(), skillModel.loopAnimation).Animation.Duration;
                    //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.EndAnimation.ToString(), skillModel.loopAnimation);
                }
                baseManager.animationManager.SetBusyAnimation(animationDuration);

                if (skillModel.attackMovementSpeed > 0)
                {
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                }
                else
                {
                    baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true);
                    //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation.ToString(), true, 0);
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
            activeSkill = setActive ? skillModel : null;
            if (BattleManager.taskManager.taskList.Count > 0 && BattleManager.taskManager.taskList.ContainsKey("waitForTarget"))
            {
                BattleManager.taskManager.taskList["waitForTarget"].Stop();
                BattleManager.taskManager.taskList.Remove("waitForTarget");
                Time.timeScale = 1f;
            }
            if (!setActive)
            {
                finalTargets.Clear();
            }
        }

        private bool CheckSkillAvail(SkillModel skillModel)
        {
            var availActionPoints = BattleManager.actionPoints;
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

            baseManager.animationManager.PlaySetAnimation(skillModel.BeginCastingAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(skillModel.CastingAnimation.ToString(), true);
            //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.BeginCastingAnimation.ToString(), false);
            //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.CastingAnimation.ToString(), true, 0);
            //New Turn based cast
            if (skillModel.CompleteSkillOnCurrentTurn() && BattleManager.turn == BattleManager.TurnEnum.PlayerTurn )
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
                if (activeSkill.Reposition != GenericSkillModel.moveType.None)
                {
                    var charList = new List<BaseCharacterManager>() { baseManager.characterManager };
                    activeSkill.RepositionCharacters(charList, activeSkill);
                    //baseManager.movementManager.ForceMoveOrReposition(skillModel);
                }

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
                        BattleManager.eventManager.BuildEvent(eventModel);
                    }
                }
            }
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
                //currenttarget = null;
            }
            BattleManager.soundManager.OnEventHit(state, e);
        }
    }
}
