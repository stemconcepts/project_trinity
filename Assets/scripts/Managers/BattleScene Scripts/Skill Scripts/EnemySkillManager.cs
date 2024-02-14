using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Spine.Unity;
using Spine;

namespace AssemblyCSharp
{
    public class EnemySkillManager : BaseSkillManager
    {
        public List<enemySkill> enemySkillList = new List<enemySkill>();
        public List<enemySkill> copiedSkillList = new List<enemySkill>();
        public List<enemySkill> phaseSkillList = new List<enemySkill>();

        public void RefreshSkillList(List<enemySkill> skillList)
        {
            copiedSkillList.Clear();
            if (skillList != null)
            {
                foreach (var skill in skillList)
                {
                    copiedSkillList.Add(Object.Instantiate(skill));
                };
            }
        }

        public void PrepSkill(enemySkill skillModel)
        {
            if (CheckSkillAvail(skillModel) && skillModel.castTurnTime <= 0)
            {
                //isSkillactive = true;
                skillModel.skillActive = true;
                BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} casts {skillModel.skillName}", 3f);
                GetTargets(skillModel);
            }
            else if(CheckSkillAvail(skillModel))
            {
                //isSkillactive = true;
                skillModel.skillActive = true;
                BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} is casting {skillModel.skillName} in {skillModel.castTurnTime} turns", 3f);
                StartCasting(skillModel);
            }
        }

        public void StartCasting(enemySkill skillModel)
        {
            isCasting = true;
            baseManager.animationManager.inAnimation = true;

            baseManager.animationManager.PlaySetAnimation(skillModel.BeginCastingAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(skillModel.CastingAnimation.ToString(), true);
            if (skillModel.castingSounds.Count > 0)
            {
                MainGameManager.instance.soundManager.playAllSounds(skillModel.castingSounds, 0.3f);
            } else if (skillModel.isSpell)
            {
                MainGameManager.instance.soundManager.playAllSounds(MainGameManager.instance.soundManager.magicChargeSounds, 0.3f);
            }
            if (skillModel.hasVoidzone)
            {
                skillModel.ShowVoidPanel(skillModel.voidZoneTypes, skillModel.monsterPanel);
            }
            skillModel.SaveTurnToComplete((int)baseManager.characterManager.characterModel.insight);
        }

        private bool ArePreRequisitesMet(enemySkill skill)
        {
            var requirementsMet = true;
            foreach (var p in skill.preRequisites)
            {
                switch (p.preRequisiteType)
                {
                    case PreRequisiteModel.preRequisiteTypeEnum.buffMinions:
                        var x = BattleManager.characterSelectManager.enemyCharacters.Where(o => o.characterModel.role == RoleEnum.minion).ToList();
                        requirementsMet = x.Count() >= p.amount;
                        break;
                    case PreRequisiteModel.preRequisiteTypeEnum.summonPanels:
                        var freePanels = 0;
                        foreach (var panel in BattleManager.allPanelManagers)
                        {
                            if (panel.currentOccupier == null)
                            {
                                ++freePanels;
                            }
                        }
                        requirementsMet = freePanels >= p.amount;
                        break;
                    case PreRequisiteModel.preRequisiteTypeEnum.spellsOnCooldown:
                        var skills = copiedSkillList.Where(o => o.preRequisites.Count() == 0 || o.preRequisites.Select(pre => pre.preRequisiteType != PreRequisiteModel.preRequisiteTypeEnum.spellsOnCooldown).FirstOrDefault()).ToList();
                        requirementsMet = !skills.Any(o => o.turnToReset == 0);
                        break;
                    default:
                        requirementsMet = true;
                        break;
                }
            }

            return requirementsMet && skill.CanCastFromPosition(skill.compatibleRows, baseManager);
        }

        /*private void ForcedMove(List<BaseCharacterManager> targets, enemySkill skillModel)
        {
            targets.ForEach(o =>
            {
                var movementScript = o.baseManager.movementManager;
                if (movementScript.isInBackRow)
                {
                    o.baseManager.damageManager.DoDamage(3, o, skillModel);
                    o.baseManager.statusManager.MakeStunned(1);
                }
                else
                {
                    movementScript.ForceMove(skillModel);
                }
            });
        }*/

        public void SkillComplete(List<BaseCharacterManager> targets, enemySkill enemySkillModel)
        {
            var power = 0.0f;
            var eM = new EventModel()
            {
                eventName = "OnSkillCast",
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eM);
            if (enemySkillModel != null && enemySkillModel.isFlat)
            {
                power = enemySkillModel.isSpell ? enemySkillModel.magicPower : enemySkillModel.skillPower;
            }
            else
            {
                power = enemySkillModel.isSpell ? enemySkillModel.newMP : enemySkillModel.newSP;
            }

            DealHealDmg(enemySkillModel, targets, power);

            hasCasted = true;
            enemySkillModel.ClearSavedVoidPanels();
            enemySkillModel.SaveTurnToReset();
            currenttarget = null;
            //isSkillactive = false;
            enemySkillModel.skillActive = false;
            enemySkillModel.ResetSkillOnCurrentTurn(false);
        }

        private void GetTargets(enemySkill skillModel, bool randomTarget = false)
        {
            finalTargets.Clear();
            var enemyPlayers = BattleManager.GetFriendlyCharacterManagers();
            var friendlyPlayers = BattleManager.GetEnemyCharacterManagers();
            if (skillModel.self) { 
                finalTargets.Add(baseManager.characterManager); 
            }
            if (skillModel.allFriendly) { 
                finalTargets.AddRange(friendlyPlayers); 
            }
            if (skillModel.allEnemy) { 
                finalTargets.AddRange(enemyPlayers);
                currenttarget = finalTargets[Random.Range(0, finalTargets.Count())];
            }
            if (skillModel.friendly)
            {
                finalTargets.Add((baseManager.characterManager).GetTarget<EnemyCharacterManagerGroup>().characterManager);
            }
            else if (skillModel.enemy)
            {
                finalTargets.Add(baseManager.characterManager.GetTarget<CharacterManagerGroup>().characterManager);
                currenttarget = finalTargets[0];
            }
            if (skillModel.hasVoidzone)
            {
                var p = enemyPlayers.Where(o => o.characterModel.inVoidZone || ((CharacterModel)o.characterModel).inVoidCounter).ToList();
                if(p.Count() > 0)
                {
                    finalTargets.AddRange(enemyPlayers.Where(o => o.characterModel.inVoidZone || ((CharacterModel)o.characterModel).inVoidCounter).ToList());
                    currenttarget = finalTargets[Random.Range(0, finalTargets.Count())];
                }
            }
            finalTargets.Capacity = finalTargets.Count;
            if (!baseManager.statusManager.DoesStatusExist("stun"))
            {
                SetAnimations(skillModel);
            }
            else
            {
                finalTargets.Clear();
            }

            copiedSkillList.ForEach(s =>
            {
                if (s.skillName == skillModel.skillName)
                {
                    s = skillModel;
                }
            });

            BattleManager.enemyActionPoints -= skillModel.skillCost;
        }

        private void DealHealDmg(enemySkill enemySkillModel, List<BaseCharacterManager> targets, float power)
        {
            foreach (var target in targets)
            {
                if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                {
                    target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                }
                SkillData data = new SkillData()
                {
                    target = target,
                    caster = baseManager.characterManager,
                    enemySkillModel = enemySkillModel,
                };

                var didHit = enemySkillModel.isSpell ? true : target.GetChanceToBeHit(baseManager.characterManager.characterModel.Accuracy);

                enemySkillModel.RunExtraEffect(data);
                if (enemySkillModel.doesDamage)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingDmg = enemySkillModel.useModifier ? power + (power * enemySkillModel.modifierAmount) : power,
                        enemySkillModel = enemySkillModel,
                        dmgSource = baseManager.characterManager,
                        textColor = enemySkillModel.dmgTextColor,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = enemySkillModel.useModifier
                    };
                    if (enemySkillModel.hasVoidzone)
                    {
                        var tankData = targets.Where(o => o.characterModel.role == RoleEnum.tank).FirstOrDefault();
                        if (!tankData || !(tankData.characterModel as CharacterModel).inVoidCounter)
                        {
                            target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                            target.baseManager.damageManager.calculatedamage(dmgModel);
                        }
                        else
                        {
                            if (target.characterModel.role == RoleEnum.tank)
                            {
                                finalTargets = new List<BaseCharacterManager>()
                                {
                                    target
                                };
                                dmgModel.modifiedDamage = true;
                                dmgModel.incomingDmg = dmgModel.incomingDmg * 1.5f;
                                tankData.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                                tankData.baseManager.damageManager.calculatedamage(dmgModel);
                            }
                        }
                        BattleManager.ClearAllVoidZones();
                    }
                    else
                    {
                        if (didHit)
                        {
                            target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                            target.baseManager.damageManager.calculatedamage(dmgModel);
                        }
                        else
                        {
                            dmgModel.incomingDmg = 0;
                            dmgModel.showDmgNumber = false;
                            MainGameManager.instance.soundManager.PlayMissSound();
                            BattleManager.battleDetailsManager.ShowDamageNumber(dmgModel, extraInfo: "Miss");
                        }
                    }
                };
                if (enemySkillModel.healsDamage)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingHeal = power * (power * enemySkillModel.modifierAmount),
                        enemySkillModel = enemySkillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = enemySkillModel.useModifier
                    };
                    target.baseManager.damageManager.calculateHdamage(dmgModel);
                };
                if ((didHit || enemySkillModel.healsDamage) && enemySkillModel.skillEffects.Count() > 0)
                {
                    enemySkillModel.skillEffects.ForEach(effect =>
                    {
                        MainGameManager.instance.gameEffectManager.CallEffectOnTarget(target.baseManager.effectsManager.GetGameObjectByFXPos(effect.fxPos), effect);
                    });
                }

                //Add Status to target -- might move this call as it seems out of context
                AddStatuses(target, enemySkillModel, didHit);

                if (enemySkillModel.forcedMoveAmount > 0 && didHit)
                {
                    ForcedMove(new List<BaseCharacterManager> { target }, enemySkillModel);
                }
            }
        }

        enemySkill SkillToRun(List<enemySkill> bossSkillList)
        {
            bossSkillList.Capacity = bossSkillList.Count;
            if (bossSkillList.Count > 0)
            {
                bossSkillList.ForEach(o => o.newSP = o.skillPower * baseManager.characterManager.characterModel.PAtk);
                bossSkillList.ForEach(o => o.newMP = o.magicPower * baseManager.characterManager.characterModel.MAtk);
            }
            var validSkills = bossSkillList.Where(o => o.turnToReset == 0 && ArePreRequisitesMet(o)).ToList();
            if (validSkills.Count() > 0)
            {
                var randomNumber = Random.Range(0, (validSkills.Count));
                var returnedSkill = validSkills[randomNumber];
                return returnedSkill;
            }
            return null;
        }

        public void BeginSkillRotation(enemySkill randomSkill)
        {
            if (BattleManager.turn == BattleManager.TurnEnum.EnemyTurn)
            {
                if (!hasCasted && !BattleManager.disableActions && baseManager.characterManager.characterModel.isAlive
                     && !baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking)
                {
                        PrepSkill(randomSkill);
                }
            }
        }

        private void SetAnimations(enemySkill skillModel)
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
                        }
                        else
                        {
                            trackEntry.End += (e) => MovecharacterOnComplete(e, skillModel);
                        }
                    }
                }
                if (skillModel.attackMovementSpeed > 0)
                {
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;

                    //Add clean up delegate to the end of the hop animation
                    var hopEntry = baseManager.animationManager.skeletonAnimation.state.Tracks
                        .Where(track => track.Animation.Name == animationOptionsEnum.hop.ToString())
                        .FirstOrDefault();
                    if(hopEntry != null)
                    {
                        hopEntry.Complete += (e) => CleanUpDamageModelsEvent(e, skillModel);
                    }
                }
                else
                {
                    if (trackEntry != null)
                    {
                        baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, trackEntry.Animation.Duration);
                        trackEntry.Complete += (e) => CleanUpDamageModelsEvent(e, skillModel);
                    }
                }
            }
        }

        public void OnEventSkillTrigger(Spine.TrackEntry spineEntry, Spine.Event e)
        {
            var activeSkill = copiedSkillList.Where(o => o.skillActive).FirstOrDefault();
            if (e.Data.Name == "swing" && activeSkill)
            {
                if (activeSkill.swingEffects.Count() > 0)
                {
                    activeSkill.swingEffects.ForEach(effect =>
                    {
                        MainGameManager.instance.gameEffectManager.CallEffectOnTarget(baseManager.effectsManager.GetGameObjectByFXPos(effect.fxPos), effect, true);
                    });
                } else
                {
                    if (defaultSwingEffect && activeSkill.skillEffects.Count() == 0)
                    {
                        var swingEffect = new SkillEffect(1, defaultSwingEffect, fxPosEnum.center);
                        MainGameManager.instance.gameEffectManager.CallEffectOnTarget(baseManager.effectsManager.GetGameObjectByFXPos(swingEffect.fxPos), swingEffect, true);
                    }
                }
            }

            if (e.Data.Name == "hit" || e.Data.Name == "triggerEvent")
            {
                copiedSkillList.ForEach(o =>
                {
                    if (o.skillActive)
                    {
                        if (o.hitSounds.Count > 0)
                        {
                            MainGameManager.instance.soundManager.playSound(o.hitSounds, 0.3f);
                        } else if(o.isSpell)
                        {
                            MainGameManager.instance.soundManager.PlayAAHitSound();
                        }
                        SkillComplete(finalTargets, o);
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
                });
            }


            /*if (e.Data.Name == "endEvent")
            {
                foreach (var target in finalTargets)
                {
                    var targetDamageManager = target.baseManager.damageManager;
                    if (targetDamageManager.skillDmgModels.ContainsKey(gameObject.name))
                    {
                        targetDamageManager.skillDmgModels.Remove(gameObject.name);
                    }
                }
            }*/
            //MainGameManager.instance.soundManager.OnEventHit(state, e);
        }

        private bool CheckSkillAvail(enemySkill skillModel)
        {
            if (BattleManager.enemyActionPoints >= skillModel.skillCost && !skillModel.skillActive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Update()
        {
            if (isCasting && BattleManager.turn == BattleManager.TurnEnum.EnemyTurn)
            {
                copiedSkillList.ForEach(o =>
                {
                    if (o.skillActive)
                    {
                        if (o.CompleteSkillOnCurrentTurn())
                        {
                            isCasting = false;
                            if (o.summon)
                            {
                                o.SummonCreatures(summonList);
                                SetAnimations(o);
                            }
                            else
                            {
                                GetTargets(o);
                            }
                        }
                    }
                });
            }
        }

        public void StartSkill()
        {
            var randomSkill = SkillToRun(copiedSkillList);
            if (randomSkill)
            {
                var endAnim = baseManager.animationManager.GetAnimationDuration(randomSkill.EndAnimation.ToString());
                BattleManager.taskManager.ScheduleAction(() => BeginSkillRotation(randomSkill), endAnim + 0.5f);
            }
        }

        void Start()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            RefreshSkillList((baseManager as EnemyCharacterManagerGroup).phaseManager.GetPhaseDetail().phaseSkills);
            if (baseManager.animationManager.skeletonAnimation && baseManager.animationManager.skeletonAnimation.isActiveAndEnabled)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillTrigger;
                baseManager.animationManager.skeletonAnimation.state.Event += MainGameManager.instance.soundManager.OnEventHit;
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(skeletonAnimation => {
                    skeletonAnimation.state.Event += OnEventSkillTrigger;
                    skeletonAnimation.state.Event += MainGameManager.instance.soundManager.OnEventHit;
                });
            }
        }
    }
}