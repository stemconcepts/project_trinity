using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class singleStatus : ScriptableObject {

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
	//public List<string> attributeNameList = new List<string>(); 
	public string attributeName;
	public statusType statusTypeEnum;
	public enum statusType {
		Immunity,
		ValueOverTime,
		Normal
	}
    public subStatus subStatus;
	//public classSkills statusSkillPlayer;
	//public enemySkill statusSkillEnemy;

//	public status statusmethodscript;
//	public statusOff selectedOffMethod;
//	public enum statusOff{ 
//		armorup, 
//		regen,
//		armordown,
//		stun,
//		poison,
//		ambitionsedge,
//		haste,
//		none
//	}

}
