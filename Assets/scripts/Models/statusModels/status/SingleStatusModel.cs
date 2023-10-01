using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssemblyCSharp
{
    [Serializable]
    public class StatusItem
    {
        public float power;
        public SingleStatusModel status;
        public bool dispellable = true;
    }

    public class SingleStatusModel : ScriptableObject {
        public string attributeName;
    	public string statusName;
    	public string displayName;
    	public string statusDesc;
    	public Sprite labelIcon;
        public bool canStack;
        [ConditionalHide("canStack", true)]
        public int maxStacks = 20;
    	public bool dispellable;
        public bool isFlat;
        public bool buff;
        [ConditionalHide("buff", true)]
        public float buffpower;
    	public int statusposition;
    	public bool active;
    	public animationOptionsEnum hitAnim;
    	public animationOptionsEnum holdAnim;
    	public statusFunction selectedStatusFunction;
    	public enum statusFunction {
    		AttributeChange,
    		StatChange,
    		AddToStat,
    		StatusOn,
    		Tumor,
    		OnHit,
    		OnTakingDamage,
    		Immune
    	}   
    	public statusType statusTypeEnum;
    	public enum statusType {
            Normal,
            Immunity,
    		ValueOverTime
    	}
        [ConditionalHide("statusTypeEnum", (int)statusType.Immunity, false)]
        public List<SingleStatusModel> immunityList;
        public subStatus subStatus;
        public triggerGrp trigger;
        public elementType element;
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
    }
}
