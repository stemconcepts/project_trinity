using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Enemy_Skill_Manager : Skill_Manager
    {
        public List<enemySkill> enemySkillList = new List<enemySkill>();
        private List<enemySkill> copiedSkillList = new List<enemySkill>();
        public List<enemySkill> phaseSkillList = new List<enemySkill>();
        public bool hasCasted;

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
            //isSkillactive = true;
            //activeSkill = skillModel;
            skillModel.skillActive = true;
            SkillActiveSet(skillModel, true);
            if (skillModel.castTurnTime <= 0)
            {
                isCasting = true;
                Battle_Manager.battleDetailsManager.BattleWarning($"{gameObject.name} casts {skillModel.skillName}", 3f);
                GetTargets(skillModel);
            }
            else
            {
                Battle_Manager.battleDetailsManager.BattleWarning($"{gameObject.name} is casting {skillModel.skillName} in {skillModel.castTurnTime} turns", 3f);
                StartCasting(skillModel);
            }
        }

        public void StartCasting(enemySkill skillModel)
        {
            isCasting = true;
            baseManager.animationManager.inAnimation = true;
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, skillModel.animationCastingType, false);
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, skillModel.animationRepeatCasting, true, 0);
            if (skillModel.hasVoidzone)
            {
                skillModel.ShowVoidPanel(skillModel.voidZoneTypes, skillModel.monsterPanel);
            }
            skillModel.SaveTurnToComplete();
            //Old time based cast
            /*Battle_Manager.taskManager.CallTask(skillModel.castTime, () =>
            {
                if (skillModel.summon)
                {
                    skillModel.SummonCreatures(summonList);
                    SetAnimations(skillModel);
                    //SkillComplete(finalTargets, enemySkillModel: skillModel);
                }
                else
                {
                    GetTargets(skillModel);
                }
            });*/
        }

        private bool ArePreRequisitesMet(enemySkill skill)
        {
            var requirementsMet = true;
            foreach (var p in skill.preRequisites)
            {
                switch (p.preRequisiteType)
                {
                    case PreRequisiteModel.preRequisiteTypeEnum.buffMinions:
                        var x = Battle_Manager.characterSelectManager.enemyCharacters.Where(o => o.characterModel.role == Character_Model.RoleEnum.minion).ToList();
                        requirementsMet = x.Count() >= p.amount;
                        break;
                    case PreRequisiteModel.preRequisiteTypeEnum.summonPanels:
                        var freePanels = 0;
                        foreach (var panel in Battle_Manager.allPanelManagers)
                        {
                            if (panel.currentOccupier == null)
                            {
                                ++freePanels;
                            }
                        }
                        requirementsMet = freePanels >= p.amount;
                        break;
                    case PreRequisiteModel.preRequisiteTypeEnum.spellsOnCooldown:
                        //var skills = ((Enemy_Skill_Manager)baseManager.skillManager).phaseSkillList.Where(o => o.preRequisite.preRequisiteType != PreRequisiteModel.preRequisiteTypeEnum.spellsOnCooldown).ToList();
                        var skills = ((Enemy_Skill_Manager)baseManager.skillManager).phaseSkillList.Where(o => o.preRequisites.Select(pre => pre.preRequisiteType != PreRequisiteModel.preRequisiteTypeEnum.spellsOnCooldown).FirstOrDefault()).ToList();
                        requirementsMet = !skills.Any(o => !o.skillActive);
                        break;
                    default:
                        requirementsMet = true;
                        break;
                }
            }
            return requirementsMet;
        }

        private void ForcedMove(List<Character_Manager> targets, enemySkill skillModel)
        {
            targets.ForEach(o =>
            {
                var movementScript = o.baseManager.movementManager;
                movementScript.ForcedMove(skillModel.forcedMove, skillModel.forcedMoveAmount);
                if (movementScript.isInBackRow)
                {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = o.baseManager,
                        incomingDmg = 50, //skillModel.useModifier ? power + (power * skillModel.modifierAmount) : power,
                        enemySkillModel = skillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = new List<Character_Manager>() { o },
                        hitEffectPositionScript = o.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skillModel.useModifier
                    };
                }
            });
        }

        public void SkillComplete(List<Character_Manager> targets, enemySkill enemySkillModel)
        {
            var power = 0.0f;
            var eM = new EventModel()
            {
                eventName = "OnSkillCast",
                eventCaller = baseManager.characterManager
            };
            Battle_Manager.eventManager.BuildEvent(eM);
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
            //isSkillactive = false;
            currenttarget = null;
            //isCasting = false;
            //activeSkill = null;

            SkillActiveSet(enemySkillModel, false);
            enemySkillModel.ResetSkillOnCurrentTurn(false, () => {
                enemySkillModel.skillActive = true;
                //SkillActiveSet(enemySkillModel, false);
            });
            /*Battle_Manager.taskManager.CallTask(enemySkillModel.skillCooldown, () =>
            {
                SkillActiveSet(enemySkillModel, false);
            });*/
        }

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
            else if (skillModel.enemy)
            {
                finalTargets.Add(baseManager.characterManager.GetTarget().characterManager);
            }
            if (skillModel.hasVoidzone)
            {
                finalTargets.AddRange(enemyPlayers.Where(o => o.characterModel.inVoidZone).ToList());
            }
            finalTargets.Capacity = finalTargets.Count;
            if (!baseManager.statusManager.DoesStatusExist("stun"))
            {
                SetAnimations(skillModel);
                //SkillComplete(finalTargets, enemySkillModel: skillModel);
            }
            else
            {
                finalTargets.Clear();
                isSkillactive = false;
            }
        }

        private void DealHealDmg(enemySkill enemySkillModel, List<Character_Manager> targets, float power)
        {
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
                };
                enemySkillModel.RunExtraEffect(data);
                if (enemySkillModel.doesDamage)
                {
                    var dmgModel = new DamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingDmg = enemySkillModel.useModifier ? power + (power * enemySkillModel.modifierAmount) : power,
                        enemySkillModel = enemySkillModel,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = enemySkillModel.useModifier
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
                            if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                            {
                                target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                            }
                            target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                            target.baseManager.damageManager.calculatedamage(dmgModel);
                        }
                        else
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
                                if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                                {
                                    target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                                }
                                dmgModel.modifiedDamage = true;
                                dmgModel.incomingDmg = dmgModel.incomingDmg * 1.5f;
                                tankData.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                                tankData.baseManager.damageManager.calculatedamage(dmgModel);
                            }
                        }
                    }
                    else
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
                        modifiedDamage = enemySkillModel.useModifier
                    };
                    target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                    target.baseManager.damageManager.calculateHdamage(dmgModel);
                    //currenttarget = null;
                };
                if (target != null)
                {
                    AddStatuses(target, power, enemySkillModel);
                }
            }
            Battle_Manager.ClearAllVoidZones();
        }

        private void AddStatuses(Character_Manager target, float power, enemySkill enemySkill)
        {
            if (target.tag == gameObject.tag)
            {
                var hitAnimation = enemySkill.singleStatusGroupFriendly.Where(o => !string.IsNullOrEmpty(o.hitAnim)).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = enemySkill.singleStatusGroupFriendly.Where(o => !string.IsNullOrEmpty(o.holdAnim)).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                enemySkill.AttachStatus(enemySkill.singleStatusGroupFriendly, target.baseManager, power, enemySkill);
            }
            else
            {
                var hitAnimation = enemySkill.singleStatusGroup.Where(o => !string.IsNullOrEmpty(o.hitAnim)).Select(o => o.hitAnim).FirstOrDefault();
                var hitIdleAnimation = enemySkill.singleStatusGroup.Where(o => !string.IsNullOrEmpty(o.holdAnim)).Select(o => o.holdAnim).FirstOrDefault();
                target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
                enemySkill.AttachStatus(enemySkill.singleStatusGroup, target.baseManager, power, enemySkill);
            }
        }

        enemySkill SkillToRun(List<enemySkill> bossSkillList)
        {
            phaseSkillList.Clear();
            bossSkillList.Capacity = bossSkillList.Count;
            for (int x = 0; x < bossSkillList.Count; x++)
            {
                if (!bossSkillList[x].skillActive && ArePreRequisitesMet(bossSkillList[x]))
                {
                    phaseSkillList.Add(Object.Instantiate(bossSkillList[x]) as enemySkill);
                }
            }
            if (phaseSkillList.Count == 0)
            {
                return null;
            }
            if (phaseSkillList.Count > 0)
            {
                phaseSkillList.ForEach(o => o.newSP = o.skillPower * baseManager.characterManager.characterModel.PAtk);
                phaseSkillList.ForEach(o => o.newMP = o.magicPower * baseManager.characterManager.characterModel.MAtk);
            }
            var randomNumber = Random.Range(0, (phaseSkillList.Count));
            var returnedSkill = phaseSkillList[randomNumber];
            return returnedSkill;
        }

        public void BeginSkillRotation( /*EnemyPhase phase*/ )
        {
            var randomSkill = SkillToRun(copiedSkillList);
            if (!hasCasted && !Battle_Manager.disableActions && (Battle_Manager.turn == Battle_Manager.TurnEnum.EnemyTurn) && baseManager.characterManager.characterModel.isAlive 
                && !isSkillactive && !baseManager.statusManager.DoesStatusExist("stun") && !baseManager.autoAttackManager.isAttacking && randomSkill != null)
            {
                if (!isSkillactive)
                {
                    //if(IsPreRequisiteMet(randomSkill))
                    //{
                    //isSkillactive = true;
                    PrepSkill(randomSkill);
                    Battle_Manager.taskManager.CallTask(10f, () => {
                        BeginSkillRotation();
                    });
                    /*} else {
                        Battle_Manager.taskManager.CallTask(1f, () =>
                        {
                            BeginSkillRotation();
                        });
                    }*/
                }
                else if (isSkillactive)
                {
                    Battle_Manager.taskManager.CallTask(1f, () => {
                        BeginSkillRotation();
                    });
                }
                else
                {
                    Battle_Manager.taskManager.CallTask(1f, () => {
                        BeginSkillRotation();
                    });
                }
            }
            else if (baseManager.characterManager.characterModel.isAlive)
            {
                Battle_Manager.taskManager.CallTask(5f, () => {
                    BeginSkillRotation();
                });
            }
        }

        public void SkillActiveSet(enemySkill skillModel, bool setActive)
        {
            try
            {
                isSkillactive = setActive;
                isCasting = setActive;
                activeSkill = setActive ? skillModel : null;
                /*if (copiedSkillList.Count > 0)
                {
                    copiedSkillList.Where(o => o.skillName == skillModel.skillName).First().skillActive = setActive;
                }*/
                //skillModel.turnToComplete = setActive ? (skillModel.castTurnTime * 2) + Battle_Manager.turnCount : 0;
                //skillModel.skillActive = setActive;
                //isCasting = setActive ? isCasting : false;
                //activeSkill = setActive ? skillModel : null;
            }
            catch (System.Exception e)
            {
                Game_Manager.logger.Log(e.Message);
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
                    baseManager.movementManager.movementSpeed = skillModel.attackMovementSpeed;
                }
                else
                {
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0);
                }
                /*Battle_Manager.taskManager.CallTask(animationDuration, () =>
                {
                    isSkillactive = false;
                });*/
            }
        }

        public void OnEventSkillTrigger(Spine.TrackEntry state, Spine.Event e)
        {
            if ((e.Data.Name == "hit" || e.Data.Name == "triggerEvent") && isSkillactive)
            {
                if ((enemySkill)activeSkill != null)
                {
                    SkillComplete(finalTargets, (enemySkill)activeSkill);
                } 

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

        void Update()
        {
            if (activeSkill != null && isCasting && Battle_Manager.turn == Battle_Manager.TurnEnum.EnemyTurn)
            {
                if (CompleteSkillOnCurrentTurn())
                {
                      isCasting = false;
                    if (activeSkill.summon)
                    {
                        ((enemySkill)activeSkill).SummonCreatures(summonList);
                        SetAnimations((enemySkill)activeSkill);
                    }
                    else
                    {
                        GetTargets((enemySkill)activeSkill);
                    }
                }
            }
        }

        void Start()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if (this.gameObject.tag == "Enemy")
            {
                RefreshSkillList(baseManager.phaseManager.GetPhaseSkills());
            }
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillTrigger;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventSkillComplete;
            }
            Battle_Manager.taskManager.CallTask(5f, () => {
                if (copiedSkillList.Count > 0)
                {
                    BeginSkillRotation();
                }
            });
        }
    }
}