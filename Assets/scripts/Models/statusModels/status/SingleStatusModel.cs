using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.scripts.Models.statusModels;
using static UnityEngine.Rendering.DebugUI;

namespace AssemblyCSharp
{
    [Serializable]
    public class StatusItem
    {
        public int duration;
        public int maxStacks = 20;
        public bool canStack;
        public float power;
        public SingleStatusModel status;
        public bool dispellable = true;
    }

    public enum StatusFunctionEnum
    {
        AttributeChange,
        StatChange,
        AddToStat,
        StatusOn,
        Tumor,
        OnHit,
        OnTakingDamage,
        Immune,
        None
    }

    public enum StatusTypeEnum
    {
        Normal,
        Immunity,
        ValueOverTime
    }

    public class SingleStatusModel : ScriptableObject {
        public string attributeName;
        public StatusNameEnum statusName
        {
            get {
                return (StatusNameEnum)Enum.Parse(typeof(StatusNameEnum), this.name);
            }
        }
        public string displayName;
        [TextArea]
    	public string statusDesc;
    	public Sprite labelIcon;
        public bool canStack;
        [ConditionalHide("canStack", true)]
        public int maxStacks = 20;
    	public bool dispellable;
        public bool isFlat;
        public bool buff;
        [HideInInspector]
    	public int statusposition;
    	public bool active;
    	public animationOptionsEnum hitAnim;
    	public animationOptionsEnum holdAnim;
    	public StatusTypeEnum statusTypeEnum;
        //[ConditionalHide("statusTypeEnum", (int)StatusTypeEnum.Immunity, false)]
        public List<SingleStatusModel> immunityList;
        public subStatus subStatus;

        public elementType element;
        public StatusFunctionEnum selectedStatusFunction;
        [Range(0.0f,1.0f)]
        public float TriggerChance;
        public List<StatusEventTrigger> StatusTriggers = new List<StatusEventTrigger>();
    }

    [Serializable]
    public class StatusEventTrigger
    {
        public List<EventTriggerEnum> Triggers;
        public EffectGrpEnum EffectGrpEnum;
    }
}
