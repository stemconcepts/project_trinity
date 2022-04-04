using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

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
            isSkillactive = true;
            //activeSkill = skillModel;
            skillModel.skillActive = true;
            //SkillActiveSet(skillModel, true);
            if (skillModel.castTurnTime <= 0)
            {
                BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} casts {skillModel.skillName}", 3f);
                GetTargets(skillModel);
            }
            else
            {
                BattleManager.battleDetailsManager.BattleWarning($"{gameObject.name} is casting {skillModel.skillName} in {skillModel.castTurnTime} turns", 3f);
                StartCasting(skillModel);
            }
        }

        public void StartCasting(enemySkill skillModel)
        {
            isCasting = true;
            baseManager.animationManager.inAnimation = true;

            baseManager.animationManager.PlaySetAnimation(skillModel.BeginCastingAnimation.ToString(), false);
            baseManager.animationManager.PlayAddAnimation(skillModel.CastingAnimation.ToString(), false);

            //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.BeginCastingAnimation.ToString(), false);
            //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.CastingAnimation.ToString(), true, 0);
            if (skillModel.chargeSound != null)
            {
                BattleManager.soundManager.playSoundUsingAudioSource(skillModel.chargeSound, baseManager.gameObject.GetComponent<AudioSource>());
            } else if (skillModel.isSpell)
            {
                BattleManager.soundManager.playSounds(BattleManager.soundManager.magicChargeSounds);
            }
            if (skillModel.hasVoidzone)
            {
                skillModel.ShowVoidPanel(skillModel.voidZoneTypes, skillModel.monsterPanel);
            }
            skillModel.SaveTurnToComplete();
        }

        private bool ArePreRequisitesMet(enemySkill skill)
        {
            var requirementsMet = true;
            foreach (var p in skill.preRequisites)
            {
                switch (p.preRequisiteType)
                {
                    case PreRequisiteModel.preRequisiteTypeEnum.buffMinions:
                        var x = BattleManager.characterSelectManager.enemyCharacters.Where(o => o.characterModel.role == CharacterModel.RoleEnum.minion).ToList();
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
            return requirementsMet;
        }

        private void ForcedMove(List<BaseCharacterManager> targets, enemySkill skillModel)
        {
            targets.ForEach(o =>
            {
                var movementScript = o.baseManager.movementManager;
                movementScript.ForceMoveOrReposition(skillModel);
                if (movementScript.isInBackRow)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = o.baseManager,
                        incomingDmg = 50, //skillModel.useModifier ? power + (power * skillModel.modifierAmount) : power,
                        enemySkillModel = skillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = new List<BaseCharacterManager>() { o },
                        hitEffectPositionScript = o.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier
                    };
                    o.baseManager.damageManager.calculatedamage(dmgModel);
                }
            });
        }

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

            if (enemySkillModel.forcedMoveAmount > 0)
            {
                ForcedMove(targets, enemySkillModel);
            }
            hasCasted = true;
            
            enemySkillModel.SaveTurnToReset();
            currenttarget = null;
            isSkillactive = false;
            enemySkillModel.skillActive = false;
            //SkillActiveSet(enemySkillModel, false);
            enemySkillModel.ResetSkillOnCurrentTurn(false, () => {
                //finalTargets.Clear();
                //Debug.Log("final targets cleared");
            });
        }

        private void GetTargets(enemySkill skillModel, bool randomTarget = false)
        {
            finalTargets.Clear();
            var enemyPlayers = BattleManager.GetFriendlyCharacterManagers();
            var friendlyPlayers = BattleManager.GetEnemyCharacterManagers();
            if (skillModel.self) { finalTargets.Add(baseManager.characterManager); }
            if (skillModel.allFriendly) { finalTargets.AddRange(friendlyPlayers); }
            if (skillModel.allEnemy) { 
                finalTargets.AddRange(enemyPlayers);
                currenttarget = finalTargets[Random.Range(0, finalTargets.Count())];
            }
            if (skillModel.friendly)
            {
                //finalTargets.Add(((Enemy_Character_Manager)baseManager.characterManager).GetFriendlyTarget().characterManager);
                finalTargets.Add((baseManager.characterManager).GetTarget<EnemyCharacterManagerGroup>().characterManager);
            }
            else if (skillModel.enemy)
            {
                finalTargets.Add(baseManager.characterManager.GetTarget<CharacterManagerGroup>().characterManager);
                currenttarget = finalTargets[0];
            }
            if (skillModel.hasVoidzone)
            {
                finalTargets.AddRange(enemyPlayers.Where(o => o.characterModel.inVoidZone || ((CharacterModel)o.characterModel).inVoidCounter).ToList());
                currenttarget = finalTargets[Random.Range(0, finalTargets.Count())];
            }
            finalTargets.Capacity = finalTargets.Count;
            if (!baseManager.statusManager.DoesStatusExist("stun"))
            {
                SetAnimations(skillModel);
            }
            else
            {
                finalTargets.Clear();
                isSkillactive = false;
            }
        }

        private void DealHealDmg(enemySkill enemySkillModel, List<BaseCharacterManager> targets, float power)
        {
            foreach (var status in enemySkillModel.singleStatusGroup)
            {
                status.dispellable = enemySkillModel.statusDispellable;
            };
            foreach (var status in enemySkillModel.singleStatusGroupFriendly)
            {
                status.dispellable = enemySkillModel.statusDispellable;
            };
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
                var didHit = target.GetChanceToBeHit(baseManager.characterManager.characterModel.accuracy, baseManager.characterManager.characterModel.evasion);
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
                        var tankData = targets.Where(o => o.characterModel.role == CharacterModel.RoleEnum.tank).First();
                        if (!(tankData.characterModel as CharacterModel).inVoidCounter)
                        {
                            if (enemySkillModel.fxObject != null)
                            {
                                baseManager.effectsManager.callEffectTarget(target, enemySkillModel.fxObject);
                            }
                            /*if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                            {
                                target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                            }*/
                            target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                            target.baseManager.damageManager.calculatedamage(dmgModel);
                        }
                        else
                        {
                            if (target.characterModel.role == CharacterModel.RoleEnum.tank)
                            {
                                finalTargets = new List<BaseCharacterManager>()
                                {
                                    target
                                };
                                if (enemySkillModel.fxObject != null)
                                {
                                    baseManager.effectsManager.callEffectTarget(target, enemySkillModel.fxObject);
                                }
                                /*if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                                {
                                    target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                                }*/
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
                        if (enemySkillModel.isSpell || didHit)
                        {
                            target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                            target.baseManager.damageManager.calculatedamage(dmgModel);
                        }
                        else
                        {
                            dmgModel.incomingDmg = 0;
                            dmgModel.showDmgNumber = false;
                            BattleManager.soundManager.playSound("miss");
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
                if (enemySkillModel.isSpell || didHit || enemySkillModel.healsDamage)
                {
                    if (enemySkillModel.fxObject != null)
                    {
                        baseManager.effectsManager.callEffectTarget(target, enemySkillModel.fxObject);
                    }
                }
                if (enemySkillModel.singleStatusGroupFriendly.Count() > 0 || (enemySkillModel.isSpell || didHit) && enemySkillModel.singleStatusGroup.Count() > 0)
                {
                    AddStatuses(target, power, enemySkillModel);
                }
            }
        }

        private void AddStatuses(BaseCharacterManager target, float power, enemySkill enemySkill)
        {
            if (target.tag == gameObject.tag)
            {
                var hitAnimation = enemySkill.singleStatusGroupFriendly.Where(o => o.hitAnim != animationOptionsEnum.none).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = enemySkill.singleStatusGroupFriendly.Where(o => o.holdAnim != animationOptionsEnum.none).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                enemySkill.AttachStatus(enemySkill.singleStatusGroupFriendly, target.baseManager, power, enemySkill);
            }
            else
            {
                var hitAnimation = enemySkill.singleStatusGroup.Where(o => o.hitAnim != animationOptionsEnum.none).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = enemySkill.singleStatusGroup.Where(o => o.holdAnim != animationOptionsEnum.none).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                enemySkill.AttachStatus(enemySkill.singleStatusGroup, target.baseManager, power, enemySkill);
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

        public void BeginSkillRotation( /*EnemyPhase phase*/ )
        {
            if (!isSkillactive)
            {
                var randomSkill = SkillToRun(copiedSkillList);
                if (!hasCasted && !BattleManager.disableActions && (BattleManager.turn == BattleManager.TurnEnum.EnemyTurn) && baseManager.characterManager.characterModel.isAlive
                     && !baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking && randomSkill != null)
                {
                        PrepSkill(randomSkill);
                        BattleManager.taskManager.CallTask(5f, () => {
                            BeginSkillRotation();
                        });
                }
                else if (baseManager.characterManager.characterModel.isAlive)
                {
                    BattleManager.taskManager.CallTask(3f, () => {
                        BeginSkillRotation();
                    });
                }
            }
            else if (isSkillactive)
            {
                BattleManager.taskManager.CallTask(1f, () => {
                    BeginSkillRotation();
                });
            }
            else
            {
                BattleManager.taskManager.CallTask(1f, () => {
                    BeginSkillRotation();
                });
            }
        }

        /*public void SkillActiveSet(enemySkill skillModel, bool setActive)
        {
            try
            {
                skillModel.skillActive = setActive;
                //isSkillactive = setActive;
                //isCasting = setActive;
                //activeSkill = setActive ? skillModel : null;
                if (!setActive)
                {
                    //finalTargets.Clear();
                }
            }
            catch (System.Exception e)
            {
                GameManager.logger.Log(e.Message);
            }
        }*/

        private void SetAnimations(enemySkill skillModel)
        {
            if (skillModel.EndAnimation != animationOptionsEnum.none)
            {
                baseManager.animationManager.inAnimation = true;

                var animationDuration = baseManager.animationManager.PlaySetAnimation(skillModel.EndAnimation.ToString(), skillModel.loopAnimation);

                //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.EndAnimation.ToString(), skillModel.loopAnimation);
                // = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.EndAnimation.ToString(), skillModel.loopAnimation).Animation.Duration;
                baseManager.animationManager.SetBusyAnimation(animationDuration);
                if (skillModel.attackMovementSpeed > 0)
                {
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                }
                else
                {
                    baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, animationDuration);
                    //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation.ToString(), true, 0);
                }
            }
        }

        public void OnEventSkillTrigger(Spine.TrackEntry state, Spine.Event e)
        {
            if ((e.Data.Name == "hit" || e.Data.Name == "triggerEvent") && isSkillactive)
            {
                copiedSkillList.ForEach(o =>
                {
                    if (o.skillActive)
                    {
                        if (o.castSound != null)
                        {
                            BattleManager.soundManager.playSoundUsingAudioSource(o.castSound, baseManager.gameObject.GetComponent<AudioSource>());
                        } else if(o.isSpell)
                        {
                            BattleManager.soundManager.playSounds(BattleManager.soundManager.magicCastSounds);
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
            BattleManager.soundManager.OnEventHit(state, e);
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

        void Start()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            if (this.gameObject.tag == "Enemy")
            {
                RefreshSkillList((baseManager as EnemyCharacterManagerGroup).phaseManager.GetPhaseDetail().phaseSkills);
            }
            if (baseManager.animationManager.skeletonAnimation && baseManager.animationManager.skeletonAnimation.isActiveAndEnabled)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillTrigger;
            } else if(baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(o => {
                    o.state.Event += OnEventSkillTrigger;
                });
            }
            BattleManager.taskManager.CallTask(5f, () => {
                if (copiedSkillList.Count > 0)
                {
                    BeginSkillRotation();
                }
            });
        }
    }
}