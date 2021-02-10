using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Auto_Attack_Manager: MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public float attackMovementSpeed = 50f;
        public int attackCoolDown = 1;
        public bool isAttacking = false;
        public bool hasAttacked = false;
        public int turnToReset = 0;
        public int turnToComplete = 0;
        public Base_Character_Manager autoAttackTarget;
        //public DamageModel dmgModel;
        public 
        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            if (baseManager.animationManager.skeletonAnimation)
            {
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAHit;
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventAAComplete;
            }
            if (baseManager.characterManager.characterModel.canAutoAttack)
            {
                Battle_Manager.taskManager.CallTask(5f + baseManager.characterManager.characterModel.Haste, () =>
                {
                    autoAttackTarget = baseManager.characterManager.GetTarget(true);
                    RunAttackLoop();
                });
            }
        }

        public void RunAttackLoop()
        {
            var targetDmgManager = autoAttackTarget ? autoAttackTarget.damageManager : null;
            if (Battle_Manager.turn == Battle_Manager.TurnEnum.EnemyTurn && targetDmgManager != null && baseManager.characterManager.characterModel.isAlive &&
                baseManager.characterManager.characterModel.canAutoAttack && !isAttacking && !baseManager.statusManager.DoesStatusExist("stun") &&
                !baseManager.animationManager.inAnimation && !baseManager.skillManager.isSkillactive)
            {
                isAttacking = true;
                if (!string.IsNullOrEmpty(baseManager.animationManager.attackAnimation) && baseManager.animationManager.inAnimation == false)
                {
                    var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.attackAnimation, false).Animation.Duration;
                    baseManager.movementManager.movementSpeed = 50f;
                    baseManager.animationManager.inAnimation = true;
                    var dmgModel = new DamageModel()
                    {
                        baseManager = autoAttackTarget,
                        incomingDmg = baseManager.characterManager.characterModel.PAtk,
                        dmgSource = baseManager.characterManager,
                        dueDmgTargets = new List<Character_Manager>(){
                                    autoAttackTarget.characterManager
                                },
                        hitEffectPositionScript = autoAttackTarget.effectsManager.fxCenter.transform
                    };
                    targetDmgManager.autoAttackDmgModels.Add(gameObject.name, dmgModel);
                    hasAttacked = true;
                    targetDmgManager.calculatedamage(dmgModel);
                    Battle_Manager.taskManager.CallTask(animationDuration, () =>
                    {
                        baseManager.animationManager.inAnimation = false;
                    });
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.attackAnimation, false);
                    SaveTurnToReset();
                    RunAttackLoopOnNextTurn();
                    /*Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                        if(this.gameObject && this.gameObject.tag == "Enemy"){
                            autoAttackTarget = baseManager.characterManager.GetTarget(true);
                        }
                        RunAttackLoop();
                    });*/
                }
            }
            else if (this != null && baseManager.characterManager.characterModel.canAutoAttack)
            {
                SaveTurnToReset();
                RunAttackLoopOnNextTurn();
                /*Battle_Manager.taskManager.CallTask( baseManager.characterManager.characterModel.ATKspd, () => {
                    baseManager.animationManager.inAnimation = false;
                    if(this.gameObject.tag == "Enemy" ){
                        autoAttackTarget = baseManager.characterManager.GetTarget(true);
                    }
                    RunAttackLoop();
                });*/
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
                    targetDamageManager.TakeDmg(damageModel, e.Data.Name);
                    var eventModel = new EventModel
                    {
                        eventName = "OnDealingDmg",
                        extTarget = autoAttackTarget.characterManager,
                        eventCaller = baseManager.characterManager,
                        extraInfo = damageModel.damageTaken
                    };
                    Battle_Manager.eventManager.BuildEvent(eventModel);
                    targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                } else
                {
                    Game_Manager.logger.Log("damageModel missing from" + autoAttackTarget.name + " dictionary");
                }
            }
        }

        public void OnEventAAComplete(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "endEvent")
            {
                var target = autoAttackTarget;
                if (target != null)
                {
                    var targetDamageManager = target.damageManager;
                    if (targetDamageManager.autoAttackDmgModels.ContainsKey(gameObject.name))
                    {
                        targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                    }
                }
            }
        }

        public void SaveTurnToReset()
        {
            turnToReset = Battle_Manager.turnCount + attackCoolDown;
        }

        public void RunAttackLoopOnNextTurn()
        {
            var myTask = new Task(Battle_Manager.taskManager.CompareTurns(turnToReset, () =>
            {
                if (baseManager.characterManager.characterModel.isAlive)
                {
                    baseManager.animationManager.inAnimation = false;
                    isAttacking = false;
                    turnToComplete = 0;
                    turnToReset = 0;
                    if (this.gameObject && this.gameObject.tag == "Enemy")
                    {
                        autoAttackTarget = baseManager.characterManager.GetTarget(true);
                    }
                    RunAttackLoop();
                }
            }));
        }
    }
}

