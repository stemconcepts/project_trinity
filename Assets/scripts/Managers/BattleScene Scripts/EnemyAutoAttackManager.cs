using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class EnemyAutoAttackManager : BaseAutoAttackManager
    {
        void Start()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAHit;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAComplete;
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(o =>
                {
                    o.state.Event += OnEventAAHit;
                    o.state.Event += OnEventAAComplete;
                });
            }
        }

        public void StartAutoAttack()
        {
            if (BattleManager.turn == BattleManager.TurnEnum.EnemyTurn && baseManager.characterManager.characterModel.canAutoAttack)
            {
                autoAttackTarget = baseManager.characterManager.GetTarget<CharacterManagerGroup>(true);
                BattleManager.taskManager.ScheduleAction(RunAttackLoopOnNextTurn, 1f);
            }
        }

        bool CanAttack(BaseDamageManager targetDmgManager)
        {
            var turnType = BattleManager.TurnEnum.EnemyTurn;
            return baseManager.characterManager.characterModel.isAlive &&
                baseManager.characterManager.characterModel.canAutoAttack && !isAttacking && !baseManager.statusManager.DoesStatusExist("stun") &&
                !baseManager.animationManager.inAnimation && !baseManager.skillManager.isSkillactive && (BattleManager.turn == turnType);
        }

        public void RunAttackLoop()
        {
            var targetDmgManager = autoAttackTarget ? autoAttackTarget.damageManager : null;
            if (CanAttack(targetDmgManager))
            {
                isAttacking = true;
                if (baseManager.animationManager.attackAnimation != animationOptionsEnum.none && baseManager.animationManager.inAnimation == false)
                {
                    var animationDuration = baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.attackAnimation.ToString(), false);
                    baseManager.movementManager.movementSpeed = 50f;
                    baseManager.animationManager.inAnimation = true;
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = autoAttackTarget as CharacterManagerGroup,
                        incomingDmg = baseManager.characterManager.characterModel.PAtk,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = new List<BaseCharacterManager>(){
                                    autoAttackTarget.characterManager
                                },
                        hitEffectPositionScript = autoAttackTarget.effectsManager.fxCenter.transform
                    };
                    targetDmgManager.autoAttackDmgModels.Add(gameObject.name, dmgModel);
                    hasAttacked = true;
                    if (autoAttackTarget.characterManager.GetChanceToBeHit(baseManager.characterManager.characterModel.accuracy, baseManager.characterManager.characterModel.evasion))
                    {
                        targetDmgManager.calculatedamage(dmgModel);
                    }
                    else
                    {
                        dmgModel.incomingDmg = 0;
                        dmgModel.showDmgNumber = false;
                        dmgModel.isMiss = true;
                        targetDmgManager.calculatedamage(dmgModel);
                    }
                    BattleManager.taskManager.CallTask(animationDuration, () =>
                    {
                        baseManager.animationManager.inAnimation = false;
                    });
                    baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.attackAnimation.ToString(), false);
                    SaveTurnToReset();
                    //ScheduleAction(RunAttackLoopOnNextTurn, 1f);
                    //RunAttackLoopOnNextTurn();
                }
                BattleManager.enemyActionPoints -= 1;
            }
            else if (this != null && baseManager.characterManager.characterModel.canAutoAttack)
            {
                SaveTurnToReset();
                //ScheduleAction(RunAttackLoopOnNextTurn, 1f);
                //RunAttackLoopOnNextTurn();
            }
        }

        public void OnEventAAHit(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "hit" && isAttacking)
            {
                var targetDamageManager = autoAttackTarget.damageManager;
                if (targetDamageManager.autoAttackDmgModels.ContainsKey(gameObject.name))
                {
                    var damageModel = targetDamageManager.autoAttackDmgModels[gameObject.name];
                    if (damageModel.isMiss)
                    {
                        BattleManager.battleDetailsManager.ShowDamageNumber(damageModel, extraInfo: "Miss");
                    }
                    else
                    {
                        targetDamageManager.TakeDmg(damageModel, e.Data.Name);
                    }
                    var eventModel = new EventModel
                    {
                        eventName = "OnDealingDmg",
                        extTarget = autoAttackTarget.characterManager,
                        eventCaller = baseManager.characterManager,
                        extraInfo = damageModel.damageTaken
                    };
                    BattleManager.eventManager.BuildEvent(eventModel);
                    targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                }
                else
                {
                    GameManager.logger.Log("damageModel missing from" + autoAttackTarget.name + " dictionary");
                }
            }
        }

        public void RunAttackLoopOnNextTurn()
        {
            var myTask = new Task(BattleManager.taskManager.CompareTurns(turnToReset, () =>
            {
                if (baseManager.characterManager.characterModel.isAlive)
                {
                    baseManager.animationManager.inAnimation = false;
                    isAttacking = false;
                    turnToComplete = 0;
                    turnToReset = 0;
                    if (this.gameObject && this.gameObject.tag == "Enemy")
                    {
                        autoAttackTarget = baseManager.characterManager.GetTarget<CharacterManagerGroup>(true);
                    }
                    RunAttackLoop();
                }
            }));
        }
    }
}