using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.scripts.Models.skillModels.swapSkills;
using Spine;

namespace AssemblyCSharp
{
    public enum fxPosEnum
    {
        center,
        bottom,
        front,
        top
    }

    public class SkillData
    {
        public BaseCharacterManager target;
        public BaseCharacterManager caster;
        public SkillModel skillModel;
        public enemySkill enemySkillModel;
        public EyeSkill eyeSkillModel;
    }

    [Serializable]
    public class SkillEffect
    {
        public SkillEffect(float size, GameObject fxObject, fxPosEnum fxPos)
        {
            this.color = new Color(1f, 1f, 1f, 1f);
            this.size = size;
            this.fxObject = fxObject;
            this.fxPos = fxPos;
        }
        public Color color = new Color(1f, 1f, 1f, 1f);
        public float size = 1.0f;
        public GameObject fxObject;
        public fxPosEnum fxPos;
    }

    public class GenericSkillModel : ScriptableObject
    {
        public bool skillConfirm;

        [Header("Skill Details")]
        public string skillName;
        [HideInInspector]
        public bool skillActive;
        public int skillCost;
        public float skillPower;
        [HideInInspector]
        public float newSP;
        public float magicPower;
        [HideInInspector]
        public float newMP;
        public bool isSpell;
        public bool isFlat;
        public elementType element;
        public bool doesDamage;
        public bool movesToTarget;
        public bool healsDamage;
        [Multiline]
        public string skillDesc;
        [ConditionalHide("movesToTarget", false, false)]
        public float attackMovementSpeed;
        [HideInInspector]
        public int turnToComplete;
        [HideInInspector]
        public int turnToReset;

        [Header("Casting/Turn Settings")]
        public int castTurnTime;
        [HideInInspector]
        public bool castTimeReady;
        public int turnDuration = 4;
        public int skillCooldown = 0;

        [Header("Animation")]
        public string skinChange;
        public animationOptionsEnum EndAnimation;
        public animationOptionsEnum BeginCastingAnimation;
        public animationOptionsEnum CastingAnimation;
        public bool loopAnimation;

        [Header("Target Choices")]
        public bool self;
        public bool enemy;
        public bool friendly;
        public bool allFriendly;
        public bool allEnemy;
        public bool targetAndSelf;
        public bool summon;

        [Header("Sounds")]
        public List<AudioClip> castingSounds = new List<AudioClip>();
        public List<AudioClip> hitSounds = new List<AudioClip>();

        [Header("Extra Effect")]
        [HideInInspector]
        public bool useModifier;
        public ExtraEffectEnum ExtraEffect;
        public enum ExtraEffectEnum
        {
            None,
            Dispel,
            BonusDamage,
            RunSkill
        }
        public List<CompatibleRow> compatibleRows;
        public enum CompatibleRow
        {
            All,
            Front,
            Middle,
            Back
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

        [Header("Skill Effects:")]
        public List<SkillEffect> skillEffects = new List<SkillEffect>();
        [Header("Swing Effects:")]
        public List<SkillEffect> swingEffects = new List<SkillEffect>();
        [ConditionalHide("doesDamage")]
        public DamageColorEnum dmgTextColor;

        [Header("Status Effects:")]
        public List<StatusItem> statusGroup = new List<StatusItem>();
        //public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        //public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        public List<StatusItem> statusGroupFriendly = new List<StatusItem>();
        //public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        //public bool statusFriendlyDispellable = true;

        public void AttachStatus(List<StatusItem> statusItems, BaseCharacterManagerGroup baseManager, GenericSkillModel skillModel)
        {
            statusItems.ForEach(statusItem =>
            {
                GenerateStatusModelAndRun(statusItem, baseManager, skillModel);
            });
        }

        public bool CanCastFromPosition(List<CompatibleRow> rows, BaseCharacterManagerGroup baseManager)
        {
            if (rows.Any(o => o == CompatibleRow.All) || rows.Count == 0)
            {
                return true;
            }
            if (baseManager.movementManager.isInFrontRow)
            {
                return rows.Any(o => o == CompatibleRow.Front);
            }
            else if (baseManager.movementManager.isInMiddleRow)
            {
                return rows.Any(o => o == CompatibleRow.Middle);
            }
            else if (baseManager.movementManager.isInBackRow)
            {
                return rows.Any(o => o == CompatibleRow.Back);
            }
            return false;
        }

        void GenerateStatusModelAndRun(StatusItem statusItem, BaseCharacterManagerGroup baseManager, GenericSkillModel skillModel)
        {
            var sm = new StatusModel
            {
                singleStatus = statusItem.status,
                power = statusItem.power,
                turnDuration = skillModel.turnDuration,
                baseManager = baseManager,
                isFlat = statusItem.status.isFlat,
                dmgTextColor = skillModel.dmgTextColor
            };
            sm.singleStatus.dispellable = statusItem.dispellable;
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