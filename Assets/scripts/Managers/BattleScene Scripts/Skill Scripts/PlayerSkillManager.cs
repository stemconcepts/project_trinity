using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core.Easing;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Assets.scripts.Models.statusModels;
using System;
//using UnityEditor.Experimental.GraphView;

namespace AssemblyCSharp
{
    public class PlayerSkillManager : BaseSkillManager
    {
        public enum weaponSlotEnum {
            Main,
            Alt
        };
        public weaponSlotEnum weaponSlot;

        private List<SkillModel> allSkills = new List<SkillModel>();

        public List<SkillModel> primaryWeaponSkills = new List<SkillModel>();
        public List<SkillModel> secondaryWeaponSkills = new List<SkillModel>();
        public SkillModel classSkillModel;
        public bool waitingForSelection;
        public int turnsTaken = 0;
        public GenericSkillModel activeSkill;

        void Start()
        {
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();

            //Calculate ability power
            CalculateSkillPower();
            CalculateMagicPower();

            //Add delegates to animations
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillTrigger;
                baseManager.animationManager.skeletonAnimation.state.Event += MainGameManager.instance.soundManager.OnEventHit;
                baseManager.animationManager.skeletonAnimation.state.Complete += (e) => CleanUpDamageModelsEvent(e, activeSkill);
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(skeletonAnimation => {
                    skeletonAnimation.state.Event += OnEventSkillTrigger;
                    skeletonAnimation.state.Event += MainGameManager.instance.soundManager.OnEventHit;
                    skeletonAnimation.state.Complete += (e) => CleanUpDamageModelsEvent(e, activeSkill);
                });
            }

            //Show skills if character is selected
            if (BattleManager.characterSelectManager.characterSelected.ToString() == (gameObject.name.ToLower() + "Selected"))
            {
                BattleManager.GetBattleInterfaces().ForEach(o => o.SkillSet((PlayerSkillManager)baseManager.skillManager));
            }

            //Save all skills to a list
            SaveAllSkillsToList();

            baseManager.EventManagerV2.AddDelegateToEvent(EventTriggerEnum.OnSkillCast, () => BattleManager.AddToGearSwapPoints(6));
            baseManager.EventManagerV2.AddDelegateToEvent(EventTriggerEnum.OnDealingDmg, () => BattleManager.AddToGearSwapPoints(4));
        }

        private void SaveAllSkillsToList()
        {
            var skills = new[] { primaryWeaponSkills, secondaryWeaponSkills };
            allSkills = skills.SelectMany(list => list).ToList();
        }

        public void PrepSkill(SkillModel skillModel, bool weaponSkill = true)
        {
            skillModel.skillActive = true;
            SkillActiveSet(skillModel, true);
            GetTargets(skillModel, weaponSkill: weaponSkill);

            /*Set trigger for event on spine inComplete
            if (skillModel.Reposition != GenericSkillModel.moveType.None)
            {
                triggerEndEvent = true;
            }*/
        }

        public void CalculateSkillPower()
        {
            for (int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newSP = primaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
                secondaryWeaponSkills[i].newSP = secondaryWeaponSkills[i].skillPower * baseManager.characterManager.characterModel.PAtk;
            }
            if (classSkillModel)
            {
                classSkillModel.newSP = classSkillModel.skillPower * baseManager.characterManager.characterModel.PAtk;
            }
        }

        public void CalculateMagicPower()
        {
            for (int i = 0; i < primaryWeaponSkills.Count; i++)
            {
                primaryWeaponSkills[i].newMP = primaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
                secondaryWeaponSkills[i].newMP = secondaryWeaponSkills[i].magicPower * baseManager.characterManager.characterModel.MAtk;
            }
            if (classSkillModel)
            {
                classSkillModel.newMP = classSkillModel.magicPower * baseManager.characterManager.characterModel.MAtk;
            }
        }

        private void CheckCastingAndUpdatePoints(SkillModel skillModel)
        {
            if (skillModel.castTurnTime > 0 && (skillModel.castTurnTime - baseManager.characterManager.characterModel.insight) > 0)
            {
                StartCasting(skillModel);
            }
            else
            {
                SetAnimations(skillModel);
            }
        }

        private void GetTargets(SkillModel skillModel, bool weaponSkill = true)
        {
            finalTargets.Clear();
            var player = this.gameObject;
            var enemyPlayers = BattleManager.GetEnemyCharacterManagers();
            var friendlyPlayers = BattleManager.GetFriendlyCharacterManagers();

            if (skillModel.Self && !skillModel.ExcludeSelf) { 
                finalTargets.Add(baseManager.characterManager as CharacterManager); }
            if (skillModel.allEnemy) {
                finalTargets.AddRange(enemyPlayers);
                currenttarget = enemyPlayers.Count > 0 ? enemyPlayers[UnityEngine.Random.Range(0, enemyPlayers.Count())] : null;
            }
            if (skillModel.allFriendly) { 
                finalTargets.AddRange(friendlyPlayers);
                if (skillModel.ExcludeSelf)
                {
                    finalTargets = finalTargets
                        .Where(target => target.name != this.name)
                        .ToList();
                }
            }
            if (skillModel.friendly || skillModel.enemy)
            {
                BattleManager.taskManager.waitForTargetTask(player, classSkill: skillModel, weaponSkill: weaponSkill, skillAction: () =>
                {
                    Time.timeScale = 1f;
                    CheckCastingAndUpdatePoints(skillModel);
                    /*if (skillModel.castTurnTime > 0 && (skillModel.castTurnTime - baseManager.characterManager.characterModel.insight) > 0)
                    {
                        StartCasting(skillModel);
                    }
                    else
                    {
                        SetAnimations(skillModel);
                    }*/

                    //Add to turn taken
                    ++turnsTaken;
                    BattleManager.actionPoints -= skillModel.skillCost;
                    BattleManager.UpdateAPAmount();
                });
                
                return;
            }
            else
            {
                if (!baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun))
                {
                    finalTargets.Capacity = finalTargets.Count;
                    CheckCastingAndUpdatePoints(skillModel);
                    /*if (skillModel.castTurnTime > 0 && (skillModel.castTurnTime - baseManager.characterManager.characterModel.insight) > 0)
                    {
                        StartCasting(skillModel);
                    }
                    else
                    {
                        SetAnimations(skillModel);
                    }*/
                }
                else
                {
                    SkillActiveSet(skillModel, false);
                }
            }

            //Add to turn taken
            ++turnsTaken;
            BattleManager.actionPoints -= skillModel.skillCost;
            BattleManager.UpdateAPAmount();
        }

        public void SkillComplete(List<BaseCharacterManager> targets, SkillModel skillModel)
        {
            var power = 0.0f;
            if (skillModel.isFlat)
            {
                power = skillModel.isSpell ? skillModel.magicPower : skillModel.skillPower;
            }
            else
            {
                power = skillModel.isSpell ? skillModel.newMP : skillModel.newSP;

            }

            DealHealDmg(skillModel, targets, power);
            skillModel.SaveTurnToReset();
            //isSkillactive = false;
            currenttarget = null;

            skillModel.ResetSkillOnCurrentTurn(true, () =>
            {
                SkillActiveSet(skillModel, false);
            });

            BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} casts {skillModel.skillName}", 3f);
            
            baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnSkillCast);
            //baseManager.EventManagerV2.AddDelegateToEvent(EventTriggerEnum.OnSkillCast, () => BattleManager.AddToGearSwapPoints(6));
        }

        private void DealHealDmg(SkillModel skillModel, List<BaseCharacterManager> targets, float power)
        {
            //Add status to self if applicable
            AddStatusToSelf(skillModel);

            //Add status to targets if applicable
            foreach (var target in targets)
            {
                SkillData data = new SkillData()
                {
                    target = target,
                    caster = baseManager.characterManager,
                    skillModel = skillModel
                };
                var didHit = skillModel.isSpell ? true : target.GetChanceToBeHit(baseManager.characterManager.characterModel.Accuracy);
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
                        //damageImmidiately = true,
                        //hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier,
                        //customHitFX = skillModel.fxObject
                    };
                    if (didHit)
                    {
                        if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                        {
                            target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                        }
                        target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                        target.baseManager.damageManager.calculatedamage(dmgModel);

                        baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnDealingDmg);
                    } else
                    {
                        dmgModel.incomingDmg = 0;
                        dmgModel.showDmgNumber = false;
                        MainGameManager.instance.soundManager.PlayMissSound();
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
                if ((didHit || skillModel.healsDamage) && skillModel.skillEffects.Count() > 0)
                {
                    skillModel.skillEffects.ForEach(effect =>
                    {
                        if(target != null) MainGameManager.instance.gameEffectManager.CallEffectOnTarget(target.baseManager.effectsManager.GetGameObjectByFXPos(effect.fxPos), effect);
                    });
                }

                //Add Status to target -- might move this call as it seems out of context
                AddStatuses(target, skillModel, didHit);
            }

            if (skillModel.hitSounds.Count > 0)
            {
                MainGameManager.instance.soundManager.playAllSounds(skillModel.hitSounds, 0.3f);
            }
            else if (skillModel.isSpell)
            {
                MainGameManager.instance.soundManager.playAllSounds(MainGameManager.instance.soundManager.hitSounds, 0.3f);
            }
        }

        private void SetAnimations(SkillModel skillModel)
        {
            if (skillModel.EndAnimation != animationOptionsEnum.none)
            {
                baseManager.animationManager.inAnimation = true;
                var trackEntry = baseManager.animationManager.PlaySetAnimation(skillModel.EndAnimation.ToString(), skillModel.loopAnimation);

                if (trackEntry != null)
                {
                    baseManager.animationManager.SetBusyAnimation(trackEntry.Animation.Duration);

                    //Set trigger for event on spine inComplete
                    if (skillModel.Reposition != GenericSkillModel.moveType.None)
                    {
                        triggerEndEvent = true;
                        if (skillModel.movesToTarget)
                        {
                            baseManager.movementManager.SetNewPosition(skillModel);
                        } else
                        {
                            trackEntry.End += (e) => MovecharacterOnComplete(e, skillModel);
                        }
                    }
                }

                if (skillModel.attackMovementSpeed > 0)
                {
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                }
                else
                {
                    baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true);
                }
            }
        }

        public void SkillActiveSet(SkillModel skillModel, bool setActive)
        {
            Debug.Log($"SkillActiveSet: setActive : Value = {setActive}, skillModel : {skillModel != null}, skillName : {skillModel.skillName}");
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
            isCasting = true;
            baseManager.animationManager.inAnimation = true;
            baseManager.animationManager.PlaySetAnimation(skillModel.BeginCastingAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(skillModel.CastingAnimation.ToString(), true);
            if (skillModel.castingSounds.Count > 0)
            {
                MainGameManager.instance.soundManager.playAllSounds(skillModel.castingSounds, 0.3f);
            }
            else if (skillModel.isSpell)
            {
                MainGameManager.instance.soundManager.playAllSounds(MainGameManager.instance.soundManager.magicChargeSounds, 0.3f);
            }
            skillModel.SaveTurnToComplete((int)baseManager.characterManager.characterModel.insight);
            
            BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} is casting {skillModel.skillName} in {skillModel.castTurnTime} turns", 3f);
        }

        public void OnEventSkillTrigger(Spine.TrackEntry spineEntry, Spine.Event spineEvent)
        {
            if (spineEvent.Data.Name == "swing" && activeSkill != null)
            {
                if (activeSkill.swingEffects.Count() > 0)
                {
                    activeSkill.swingEffects.ForEach(effect =>
                    {
                        MainGameManager.instance.gameEffectManager.CallEffectOnTarget(baseManager.effectsManager.GetGameObjectByFXPos(effect.fxPos), effect);
                    });
                }
                else
                {
                    if (defaultSwingEffect && activeSkill.skillEffects.Count() == 0)
                    {
                        var swingEffect = new SkillEffect(1, defaultSwingEffect, fxPosEnum.center);
                        MainGameManager.instance.gameEffectManager.CallEffectOnTarget(baseManager.effectsManager.GetGameObjectByFXPos(swingEffect.fxPos), swingEffect);
                    }
                }
            }

            if ((spineEvent.Data.Name == "hit" || spineEvent.Data.Name == "triggerEvent") && activeSkill.skillActive)
            {
                try
                {
                    SkillComplete(finalTargets, (SkillModel)activeSkill);

                    foreach (var target in finalTargets)
                    {
                        var targetDamageManager = target.baseManager.damageManager;
                        var damageModel = targetDamageManager.skillDmgModels.ContainsKey(gameObject.name) ? targetDamageManager.skillDmgModels[gameObject.name] : null;
                        if (damageModel != null)
                        {
                            targetDamageManager.TakeDmg(damageModel, spineEvent.Data.Name);
                        }
                    }
                }
                catch (System.Exception err)
                {
                    Debug.Log($"Event Name : {spineEvent.Data.Name}, Error : {err.Message}");
                    throw;
                }
            }
        }

        void Update()
        {
            //Checks if player character is casting and if so if the cast should be completed
            if (isCasting && activeSkill != null && !baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun)
                && activeSkill.CompleteSkillOnCurrentTurn() 
                && BattleManager.turn == BattleManager.TurnEnum.PlayerTurn)
            {
                hasCasted = true;
                isCasting = false;
                SetAnimations((SkillModel)activeSkill);
            }
        }
    }
}

