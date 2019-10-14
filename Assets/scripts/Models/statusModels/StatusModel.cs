using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StatusModel : MonoBehaviour
    {
        public SingleStatusModel singleStatus;
        public Base_Character_Manager baseManager; 
        public SkillModel onHitSkill; 
        public float duration;
        public float power {get; set;}
        public bool dispellable;
        public bool turnOff;
        public bool regenOn;
        public float triggerChance;
        public string targetStat;
        public string stat;
        public int stacks;
        public statusFunction selectedStatusFunction;
        public enum statusFunction
        {
            AttributeChange,
            StatChange,
            AddToStat,
            StatusOn,
            Tumor,
            OnHit,
            OnHitEnemy,
            Immune
        }
        public string attributeName;
        public statusType statusTypeEnum;
        public enum statusType {
            Immunity,
            ValueOverTime,
            Normal
        }
        public subStatus subStatus;
        public triggerGrp trigger;
        public enum triggerGrp {
            None,
            Passive,
            OnTakingDmg,
            OnDealingDmg,
            OnHeal,
            OnMove,
            OnSkillCast
        };
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
    }
}
