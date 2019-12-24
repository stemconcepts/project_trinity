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
        public int maxStacks = 3;
    	public bool dispellable;
    	public bool buff;
    	public float buffpower;
    	public int statusposition;
    	public bool active;
    	public string hitAnim;
    	public string holdAnim;
        //public bool trueDamage;
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
        public elementType element;
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
    }
}
