using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.scripts.Models.skillModels.swapSkills;
using Spine;
using Unity.IO.Archive;
using Assets.scripts.Helpers.Utility;
using DG.Tweening.Core.Easing;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public enum fxPosEnum
    {
        center,
        bottom,
        front,
        top
    }

    public enum CompatibleRow
    {
        All,
        Front,
        Middle,
        Back
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
        [Header("Skill Details")]
        public bool skillConfirm;
        public string skillName
        {
            get
            {
                return LabelConverter.ConvertCamelCaseToWord(this.name);
            }
            set{}
        }
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
        [TextArea]
        public string skillDesc;
        protected string descExtra
        {
            get
            {
                return GenerateDescriptionFromStatus();
            }
        }
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
        public int skillCooldown = 1;

        [Header("Animation")]
        public string skinChange;
        public animationOptionsEnum EndAnimation;
        public animationOptionsEnum BeginCastingAnimation;
        public animationOptionsEnum CastingAnimation;
        public bool loopAnimation;

        [Header("Target Choices")]
        public bool Self;
        public bool enemy;
        public bool friendly;
        public bool allFriendly;
        public bool allEnemy;
        public bool ExcludeSelf;
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
            RunSkill,
            TakeDmgFromCast
        }
        public List<CompatibleRow> compatibleRows = new List<CompatibleRow>();
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

        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.Dispel, false)]
        public StatusNameEnum targetStatusName;

        [ConditionalHide("bonusPrerequisite", (int)BonusPrerequisite.SubStatusExists, false)]
        public subStatus subStatus;

        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.None == 0, false)]
        public float modifierAmount;

        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.TakeDmgFromCast, false)]
        public int DamageFromCastAmount;

        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.TakeDmgFromCast, false)]
        public elementType DamageFromCastElement;

        [ConditionalHide("ExtraEffect", (int)ExtraEffectEnum.TakeDmgFromCast, false)] 
        public bool DamageFromCastIsFlat;

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

        [Header("Self Status Effects:")]
        [Tooltip("Will only affect performing character")]
        public List<StatusItem> SelfStatusGroup = new List<StatusItem>();
        [Header("Enemy Status Effects:")]
        public List<StatusItem> statusGroup = new List<StatusItem>();
        [Header("Friendly Status Effects:")]
        [Tooltip("Will affect all friendly characters unless Exclude Self is ticked")]
        public List<StatusItem> statusGroupFriendly = new List<StatusItem>();

        public string GetExtraDesc()
        {
            return descExtra;
        }

        private string GenerateDescriptionFromStatus()
        {
            var finalDesc = "";

            string DescFromStatuses(List<StatusItem> statuses, string prefix)
            {
                var desc = "";

                statuses.ForEach(value =>
                {
                    var intensity = value.power > 0 ? $" with an intensity of <b><color=#ff7849>{value.power}</color></b>" : "";
                    desc += $"{prefix} the <b>{LabelConverter.ConvertCamelCaseToWord(value.status.statusName.ToString())}</b> status. {value.status.statusDesc}{intensity}.";
                    if (value.canStack)
                    {
                        var stackAmount = value.maxStacks > 0 ? value.maxStacks.ToString() : "infinity";
                        desc += $" Can stack to a max stack of <b><color=#ff7849>{stackAmount}</color></b>";
                    }
                });

                return desc;
            }

            if (SelfStatusGroup.Count > 0)
            {
                finalDesc += DescFromStatuses(SelfStatusGroup, "\nApply to yourself ");
            }

            if (statusGroup.Count > 0)
            {
                finalDesc += DescFromStatuses(statusGroup, "\nApply to enemies ");
            }

            if (statusGroupFriendly.Count > 0)
            {
                finalDesc += DescFromStatuses(statusGroupFriendly, "\nApply to friendly units ");
            }

            if (RepositionAmount > 0)
            {
                finalDesc += $"\nReposition yourself <b>{Reposition}</b> by <b>{RepositionAmount}</b> panel";
            }

            if (forcedMoveAmount > 0)
            {
                finalDesc += $"\nReposition an enemy <b>{forcedMove}</b> by <b>{forcedMoveAmount}</b> panel";
            }

            return finalDesc;
        }

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
                turnDuration = statusItem.duration, //skillModel.turnDuration,
                baseManager = baseManager,
                isFlat = statusItem.status.isFlat,
                dmgTextColor = skillModel.dmgTextColor,
                triggerChance = statusItem.status.TriggerChance
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
                case ExtraEffectEnum.TakeDmgFromCast:
                    TakeDamageFromSkillCast(data);
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

        public void Dispel(SkillData data, StatusNameEnum? targetStatus = null)
        {
            if (targetStatus != null && targetStatus != StatusNameEnum.None)
            {
                var statusToRemove = data.target.baseManager.statusManager.GetStatusIfExist((StatusNameEnum)targetStatus);
                BattleManager.battleDetailsManager.RemoveLabel(statusToRemove);
                statusToRemove.statusModel.turnOff = true;
                data.target.baseManager.statusManager.RunStatusFunction(statusToRemove.statusModel);
                return;
            }

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

        private void TakeDamageFromSkillCast(SkillData skillData)
        {
            if (skillData.skillModel != null)
            {
                skillData
                    .caster
                    .baseManager
                    .damageManager
                    .DoDamage(
                        (int)skillData.skillModel.DamageFromCastAmount,
                        skillData.caster.baseManager.characterManager, 
                        isFlat: skillData.skillModel.DamageFromCastIsFlat);
            } else if (skillData.enemySkillModel != null)
            {
                skillData
                    .caster
                    .baseManager
                    .damageManager
                    .DoDamage(
                        (int)skillData.enemySkillModel.DamageFromCastAmount,
                        skillData.caster.baseManager.characterManager,
                        isFlat: skillData.enemySkillModel.DamageFromCastIsFlat);
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

        //More insight reduces cast time
        public void SaveTurnToComplete(int insight)
        {
            turnToComplete = (BattleManager.turnCount + castTurnTime) - insight;
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

            if (turnToReset == 0)
            {
                if (relevantBIM != null)
                {
                    relevantBIM.skillCDImage.fillAmount = 0;
                }
                action?.Invoke();
                skillActive = false;
                turnToComplete = 0;
                turnToReset = 0;
            } else
            {
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
        }

        public void RepositionCharacters(List<BaseCharacterManager> targets)
        {
            targets.ForEach(o =>
            {
                var movementScript = o.baseManager.movementManager;
                movementScript.Reposition(this);
            });
        }

        public void SetNewPosition(List<BaseCharacterManager> targets)
        {
            targets.ForEach(o =>
            {
                var movementScript = o.baseManager.movementManager;
                movementScript.Reposition(this);
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