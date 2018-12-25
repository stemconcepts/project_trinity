using System;

namespace AssemblyCSharp
{
    public class StatusModel
    {
        public singleStatus singleStatus { get; set; } 
        public classSkills onHitSkill { get; set; } 
        public float duration { get; set; }
        public float power {get; set;}
        public bool dispellable { get; set; }
        public bool turnOff { get; set; }
        public float triggerChance { get; set; }
        public string targetStat { get; set; }
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
        public StatusModel()
        {
        }
    }
}