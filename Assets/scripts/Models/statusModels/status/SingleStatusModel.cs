using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
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
    	public bool buff;
        [ConditionalHide("buff", true)]
        public float buffpower;
    	public int statusposition;
    	public bool active;
    	public string hitAnim;
    	public string holdAnim;
    	public statusFunction selectedStatusFunction;
    	public enum statusFunction {
    		AttributeChange,
    		StatChange,
    		AddToStat,
    		StatusOn,
    		Tumor,
    		OnHit,
    		OnHitEnemy,
    		Immune
    	}   
    	public statusType statusTypeEnum;
    	public enum statusType {
            Normal,
            Immunity,
    		ValueOverTime
    	}
        [ConditionalHide("statusTypeEnum", (int)statusType.Immunity, true)]
        public List<SingleStatusModel> immunityList;
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
        public elementType element;
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
    }
}
