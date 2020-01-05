using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class SkillData
    {
        //public float modifier = 1f;
        //public List<Character_Manager> targets;
        public Character_Manager target;
        public Character_Manager caster;
        public SkillModel skillModel;
        public enemySkill enemySkillModel;
    }

    public class GenericSkillModel : ScriptableObject
    {
        [Header("Skill Details:")]
        public string skillName;
        public bool skillActive;
        public float skillPower;
        public float newSP;
        public float magicPower;
        public float newMP;
        public float modifierAmount;
        public float duration;
        public float castTime;
        public bool castTimeReady;
        public float skillCooldown;
        public float currentCDAmount;
        public bool isSpell;
        public bool isFlat;
        public GameObject hitEffect;
        public elementType element;
        public bool doesDamage;
        public bool movesToTarget;
        public bool healsDamage;
        [Multiline]
        public string skillDesc;
        public float attackMovementSpeed;
        [Header("Animation:")]
        public string skinChange;
        public string animationType;
        public string animationCastingType;
        public string animationRepeatCasting;
        public bool loopAnimation;
        [Header("Target choices:")]
        public bool self;
        public bool enemy;
        public bool friendly;
        public bool allFriendly;
        public bool allEnemy;
        public bool targetAndSelf;
        public bool summon;
        [Header("Extra Effect:")]
        public ExtraEffectEnum ExtraEffect;
        public enum ExtraEffectEnum
        {
            None,
            Dispel,
            BonusDamage,
            RunSkill
        }
        public SkillModel ExtraSkillToRun;
        public subStatus subStatus;
        [Header("Forced Movement:")]
        public int forcedMoveAmount;
        public forcedMoveType forcedMove;
        public enum forcedMoveType
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

        [Header("Status Effects:")]
        public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        public bool statusFriendlyDispellable = true;

        public void AttachStatus(List<SingleStatusModel> singleStatusGroup, Base_Character_Manager baseManager, float power, SkillModel skillModel)
        {
            for (int i = 0; i < singleStatusGroup.Count; i++)
            {
                var sm = new StatusModel
                {
                    singleStatus = singleStatusGroup[i],
                    power = power,
                    duration = skillModel.duration,
                    baseManager = baseManager
                };
                sm.singleStatus.dispellable = skillModel.statusDispellable;
                baseManager.statusManager.RunStatusFunction(sm);
            }
        }

        public void AttachStatus(List<SingleStatusModel> singleStatusGroup, Base_Character_Manager baseManager, float power, enemySkill skillModel)
        {
            for (int i = 0; i < singleStatusGroup.Count; i++)
            {
                var sm = new StatusModel
                {
                    singleStatus = singleStatusGroup[i],
                    power = power,
                    duration = skillModel.duration,
                    baseManager = baseManager
                };
                sm.singleStatus.dispellable = skillModel.statusDispellable;
                baseManager.statusManager.RunStatusFunction(sm);
            }
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
                    BonusDamageBySubStatus(data);
                    break;
                case ExtraEffectEnum.RunSkill:
                    RunExtraSkill(data);
                    break;
            }
        }

        public void RunExtraSkill(SkillData data)
        {
            var casterSkillSelection = data.caster != null ? data.caster.GetComponent<Skill_Manager>() : null;
            if (data.caster != null)
            {
                casterSkillSelection.PrepSkill(data.skillModel.ExtraSkillToRun);
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
                    Battle_Manager.battleDetailsManager.RemoveLabel(activeStatus);
                    activeStatus.statusModel.turnOff = true;
                    data.target.baseManager.statusManager.RunStatusFunction(activeStatus.statusModel);
                    buffsRemoved++;
                }
                /*else if (positionId == debuffPower && activeStatus.buff && !activeStatus.dispellable && !activeStatus.statusModel.singleStatus.active)
                {
                }*/
            }
        }

        //Does extra damage based on status effect      
        public void BonusDamageBySubStatus(SkillData data)
        {
            var subStatusFound = false;
            var statusList = data.target.baseManager.statusManager.GetAllStatusIfExist(false);
            foreach (var status in statusList)
            {
                if (status.statusModel.singleStatus.subStatus == data.skillModel.subStatus)
                {
                    subStatusFound = true;
                    return;
                }
            }
            if (!subStatusFound)
            {
                    if (data.skillModel != null)
                    {
                        data.skillModel.modifierAmount = 0f;
                    }
                    if (data.enemySkillModel != null)
                    {
                        data.enemySkillModel.modifierAmount = 0f;
                    }
            }
        }
    }
}