using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StatusModel
    {
        public StatusModel()
        {
        } 
        public SingleStatusModel singleStatus;
        public Character_Manager characterManager; 
        public classSkills onHitSkill; 
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
        public List<EffectOnEvent> effectsOnEvent = new List<EffectOnEvent>();
    }
}
