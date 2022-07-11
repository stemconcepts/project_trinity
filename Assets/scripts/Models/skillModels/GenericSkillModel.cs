using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class SkillData
    {
        public BaseCharacterManager target;
        public BaseCharacterManager caster;
        public SkillModel skillModel;
        public enemySkill enemySkillModel;
    }

    public class GenericSkillModel : ScriptableObject
    {
        public bool skillConfirm;
        [Header("Skill Details:")]
        public string skillName;
        public bool skillActive;
        public int skillCost;
        public float skillPower;
        public float newSP;
        public float magicPower;
        public float newMP;
        public bool isSpell;
        public bool isFlat;
        public GameObject hitEffect;
        public elementType element;
        public bool doesDamage;
        public bool movesToTarget;
        public bool healsDamage;
        [Multiline]
        public string skillDesc;
        [ConditionalHide("movesToTarget", false, false)]
        public float attackMovementSpeed;
        [Header("Turn Details:")]
        public int turnToComplete;
        public int turnToReset;
        public int castTurnTime;
        public bool castTimeReady;
        public int turnDuration = 4;
        public int skillCooldown = 0;
        [Header("Animation:")]
        public string skinChange;
        public animationOptionsEnum EndAnimation;
        public animationOptionsEnum BeginCastingAnimation;
        public animationOptionsEnum CastingAnimation;
        public bool loopAnimation;
        [Header("Target choices:")]
        public bool self;
        public bool enemy;
        public bool friendly;
        public bool allFriendly;
        public bool allEnemy;
        public bool targetAndSelf;
        public bool summon;
        [Header("Sounds")]
        public AudioClip chargeSound;
        public AudioClip castSound;
        [Header("Extra Effect:")]
        [ConditionalHide(true)]
        public bool useModifier;
        public ExtraEffectEnum ExtraEffect;
        public enum ExtraEffectEnum
        {
            None,
            Dispel,
            BonusDamage,
            RunSkill
        }
        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.BonusDamage, false)]
        public BonusPrerequisite bonusPrerequisite;
        public enum BonusPrerequisite
        {
            InFrontRow,
            InBackRow,
            InMiddleRow,
            SubStatusExists
        }
        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.RunSkill, false)]
        public GenericSkillModel ExtraSkillToRun;
        [ConditionalHide("bonusPrerequisite", (int)BonusPrerequisite.SubStatusExists, false)]
        public subStatus subStatus;
        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.None, true)]
        public float modifierAmount;

        [Header("Reposition:")]
        [ConditionalHide("Reposition", (int)moveType.None == 0, true)]
        public int RepositionAmount;
        public moveType Reposition;

        [Header("Forced Movement:")]
        [ConditionalHide("forcedMove", (int)moveType.None == 0, true)]
        public int forcedMoveAmount;
        public moveType forcedMove;
        public enum moveType
        {
            None,
            Back,
            Forward
        }

        [Header("FX Animation:")]
        public GameObject fxObject;
        public fxPosEnum fxPos;
        public enum fxPosEnum
        {
            center,
            bottom,
            front,
            top
        }
        public DamageColorEnum dmgTextColor;

        [Header("Status Effects:")]
        public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        public bool statusFriendlyDispellable = true;

        public void AttachStatus(List<SingleStatusModel> singleStatusGroup, BaseCharacterManagerGroup baseManager, float power, SkillModel skillModel)
        {
            for (int i = 0; i < singleStatusGroup.Count; i++)
            {
                GenerateStatusModelAndRun(singleStatusGroup[i], baseManager, power, skillModel);
            }
        }

        public void AttachStatus(List<SingleStatusModel> singleStatusGroup, BaseCharacterManagerGroup baseManager, float power, enemySkill skillModel)
        {
            for (int i = 0; i < singleStatusGroup.Count; i++)
            {
                GenerateStatusModelAndRun(singleStatusGroup[i], baseManager, power, skillModel);
            }
        }

        void GenerateStatusModelAndRun(SingleStatusModel singleStatus, BaseCharacterManagerGroup baseManager, float power, GenericSkillModel skillModel)
        {
            var sm = new StatusModel
            {
                singleStatus = singleStatus,
                power = power,
                turnDuration = skillModel.turnDuration,
                baseManager = baseManager,
                isFlat = skillModel.isFlat,
                dmgTextColor = skillModel.dmgTextColor
            };
            sm.singleStatus.dispellable = skillModel.statusDispellable;
            baseManager.statusManager.RunStatusFunction(sm);
        }

        public void RunExtraEffect(SkillData data)
        {
            switch (ExtraEffect)
            {
                case ExtraEffectEnum.None:
                    break;
                case ExtraEffectEnum.Dispel:
                    Dispel(data);
                    break;
                case ExtraEffectEnum.BonusDamage:
                    switch (bonusPrerequisite)
                    {
                        case BonusPrerequisite.SubStatusExists:
                            BonusDamageBySubStatus(data);
                            break;
                        case BonusPrerequisite.InFrontRow:
                            BonusDamageFromPosition(data, bonusPrerequisite);
                            break;
                        case BonusPrerequisite.InMiddleRow:
                            BonusDamageFromPosition(data, bonusPrerequisite);
                            break;
                        case BonusPrerequisite.InBackRow:
                            BonusDamageFromPosition(data, bonusPrerequisite);
                            break;
                    }
                    break;
                case ExtraEffectEnum.RunSkill:
                    RunExtraSkill(data);
                    break;
            }
        }

        public void RunExtraSkill(SkillData data)
        {
            if (data.caster != null)
            {
                var casterSkillSelection = data.caster != null ? data.caster.GetComponent<BaseSkillManager>() : null;
                if (data.skillModel)
                {
                    ((PlayerSkillManager)casterSkillSelection).PrepSkill((SkillModel)data.skillModel.ExtraSkillToRun);
                } else if(data.enemySkillModel)
                {
                    ((EnemySkillManager)casterSkillSelection).PrepSkill((enemySkill)data.enemySkillModel.ExtraSkillToRun);
                }
            }
        }

        public void Dispel(SkillData data)
        {
            var debuffPower = data.skillModel != null ? data.skillModel.skillPower : 0;
            var buffsRemoved = 0;
            foreach (StatusLabelModel activeStatus in data.target.baseManager.statusManager.GetAllStatusIfExist(true))
            {
                if (buffsRemoved < debuffPower && activeStatus.buff && activeStatus.dispellable)
                {
                    BattleManager.battleDetailsManager.RemoveLabel(activeStatus);
                    activeStatus.statusModel.turnOff = true;
                    data.target.baseManager.statusManager.RunStatusFunction(activeStatus.statusModel);
                    buffsRemoved++;
                }
            }
        }
   
        public void BonusDamageBySubStatus(SkillData data)
        {
            var statusList = data.target.baseManager.statusManager.GetAllStatusIfExist(false);
            useModifier = false;
            foreach (var status in statusList)
            {
                if (status.statusModel.singleStatus.subStatus == data.skillModel.subStatus)
                {
                    useModifier = true;
                    return;
                }
            }
        }

        public void BonusDamageFromPosition(SkillData data, BonusPrerequisite positionNeeded)
        {
            useModifier = false;
            if ((data.caster.baseManager.movementManager.isInFrontRow && positionNeeded == BonusPrerequisite.InFrontRow)
                || (data.caster.baseManager.movementManager.isInMiddleRow && positionNeeded == BonusPrerequisite.InMiddleRow)
                || (data.caster.baseManager.movementManager.isInBackRow && positionNeeded == BonusPrerequisite.InBackRow)
                )
            {
                useModifier = true;
                return;
            }
        }

        public void SaveTurnToReset()
        {
            turnToReset = BattleManager.turnCount + (skillCooldown * 2);
        }

        public void SaveTurnToComplete()
        {
            turnToComplete = BattleManager.turnCount + castTurnTime;
        }

        public void ResetSkillOnCurrentTurn(bool player, Action action = null)
        {
            BattleInterfaceManager relevantBIM = null;

            if (player) {
                BattleManager.battleInterfaceManager.ForEach(o =>
                {
                    if (o.skill == (SkillModel)this)
                    {
                        relevantBIM = o;
                        o.skillCDImage.fillAmount = 1;
                    }
                });
            }

            var turn = typeof(enemySkill) == this.GetType() ? BattleManager.TurnEnum.EnemyTurn : BattleManager.TurnEnum.PlayerTurn;
            var myTask = new Task(BattleManager.taskManager.CompareTurnsAndAction(turnToReset, turn, () =>
            {
                if (relevantBIM != null)
                {
                    relevantBIM.skillCDImage.fillAmount = 0;
                }
                action?.Invoke();
                skillActive = false;
                turnToComplete = 0;
                turnToReset = 0;
            }));
        }

        public void RepositionCharacters(List<BaseCharacterManager> targets, GenericSkillModel skillModel)
        {
            targets.ForEach(o =>
            {
                var movementScript = o.baseManager.movementManager;
                movementScript.ForceMoveOrReposition(skillModel);
            });
        }

        public bool CompleteSkillOnCurrentTurn()
        {
            if (BattleManager.turnCount >= turnToComplete)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}