using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StatusModel
    {
        public StatusModel()
        {
        } 
        public SingleStatusModel singleStatus { get; set; }
        public Character_Manager characterManager { get; set; } 
        public classSkills onHitSkill { get; set; } 
        public float duration { get; set; }
        public float power {get; set;}
        public bool dispellable { get; set; }
        public bool turnOff { get; set; }
        public bool regenOn { get; set; }
        public float triggerChance { get; set; }
        public string targetStat { get; set; }
        public string stat { get; set; }
        public int stacks {get; set;}
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
