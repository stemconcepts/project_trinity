using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleStatusModel : ScriptableObject {
    public string attributeName;
	public string statusName;
	public string displayName;
	public string statusDesc;
	public Sprite labelIcon;
    public bool canStack;
    public int maxStacks = 3;
	public bool debuffable;
	public bool buff;
	public float buffpower;
	public int statusposition;
	public bool active;
	public string hitAnim;
	public string holdAnim;
	/*public statusFunction selectedStatusFunction;
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
    public List<EffectOnEvent> effectsOnEvent = new List<EffectOnEvent>();*/
}
