using System.Collections.Generic;

namespace AssemblyCSharp
{ 
    public class PlayerAutoAttackManager : BaseAutoAttackManager
    {
        void Start(){
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAHit;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAComplete;
            }
            else if (baseManager.animationManager.skeletonAnimationMulti)
            {
                baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(o => {
                    o.state.Event += OnEventAAHit;
                    o.state.Event += OnEventAAComplete;
                });
            }
            StartAutoAttack();
        }

        void StartAutoAttack() 
        {
            if (BattleManager.turn == BattleManager.TurnEnum.EnemyTurn && (baseManager as CharacterManagerGroup).characterManager.characterModel.canAutoAttack)
            {
                BattleManager.taskManager.CallTask(5f, () =>
                {
                    autoAttackTarget = (baseManager as EnemyCharacterManagerGroup).characterManager.GetTarget<EnemyCharacterManagerGroup>(true);
                    RunAttackLoopOnNextTurn();
                });
            }
        }

        public void RunAttackLoop()
        {
            var turnType = BattleManager.TurnEnum.PlayerTurn;
            var targetDmgManager = autoAttackTarget ? autoAttackTarget.damageManager : null;
            if (targetDmgManager != null && (baseManager as CharacterManagerGroup).characterManager.characterModel.isAlive &&
                (baseManager as CharacterManagerGroup).characterManager.characterModel.canAutoAttack && !isAttacking && !baseManager.statusManager.DoesStatusExist("stun") &&
                !baseManager.animationManager.inAnimation /*&& !baseManager.skillManager.isSkillactive*/ && (BattleManager.turn == turnType))
            {
                isAttacking = true;
                if (baseManager.animationManager.attackAnimation != animationOptionsEnum.none && baseManager.animationManager.inAnimation == false)
                {
                    var trackEntry = baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.attackAnimation.ToString(), false);
                    //var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.attackAnimation.ToString(), false).Animation.Duration;
                    baseManager.movementManager.movementSpeed = 50f;
                    baseManager.animationManager.inAnimation = true;
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = autoAttackTarget,
                        incomingDmg = (baseManager as CharacterManagerGroup).characterManager.characterModel.PAtk,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = new List<BaseCharacterManager>(){
                                    (autoAttackTarget as EnemyCharacterManagerGroup).characterManager
                                },
                        hitEffectPositionScript = autoAttackTarget.effectsManager.fxCenter.transform
                    };
                    targetDmgManager.autoAttackDmgModels.Add(gameObject.name, dmgModel);
                    hasAttacked = true;
                    if ((autoAttackTarget as EnemyCharacterManagerGroup).characterManager.GetChanceToBeHit((baseManager as CharacterManagerGroup).characterManager.characterModel.Accuracy))
                    {
                        targetDmgManager.calculatedamage(dmgModel);
                    } else
                    {
                        dmgModel.incomingDmg = 0;
                        dmgModel.showDmgNumber = false;
                        dmgModel.isMiss = true;
                        targetDmgManager.calculatedamage(dmgModel);
                    }

                    if (trackEntry != null)
                    {
                        BattleManager.taskManager.CallTask(trackEntry.Animation.Duration, () =>
                        {
                            baseManager.animationManager.inAnimation = false;
                        });
                    }
                    
                    baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.attackAnimation.ToString(), false);
                    //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.attackAnimation.ToString(), false);
                    SaveTurnToReset();
                    RunAttackLoopOnNextTurn();
                }
            }
            else if (this != null && (baseManager as CharacterManagerGroup).characterManager.characterModel.canAutoAttack)
            {
                SaveTurnToReset();
                RunAttackLoopOnNextTurn();
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
                    } else
                    {
                        targetDamageManager.TakeDmg<PlayerDamageModel>(damageModel as PlayerDamageModel, e.Data.Name);
                    }
                    var eventModel = new EventModel
                    {
                        eventName = "OnDealingDmg",
                        extTarget = (autoAttackTarget as EnemyCharacterManagerGroup).characterManager,
                        eventCaller = (baseManager as CharacterManagerGroup).characterManager,
                        extraInfo = damageModel.damageTaken
                    };
                    BattleManager.eventManager.BuildEvent(eventModel);
                    targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                } else
                {
                    GameManager.logger.Log("damageModel missing from" + autoAttackTarget.name + " dictionary");
                }
            }
        }

        public void RunAttackLoopOnNextTurn()
        {
            //var myTask = new Task(BattleManager.taskManager.CompareTurns(turnToReset, () =>
            //{
                if ((baseManager as CharacterManagerGroup).characterManager.characterModel.isAlive)
                {
                    baseManager.animationManager.inAnimation = false;
                    isAttacking = false;
                    turnToComplete = 0;
                    turnToReset = 0;
                    if (this.gameObject && this.gameObject.tag == "Enemy")
                    {
                        autoAttackTarget = (baseManager as CharacterManagerGroup).characterManager.GetTarget<CharacterManagerGroup>(true);
                        //autoAttackTarget = baseManager.characterManager.GetTarget(true);
                    }
                    RunAttackLoop();
                }
            //}));
            //BattleManager.taskManager.taskList.Add("PlayerAutoAttackLoop", myTask);
        }
    }
}

