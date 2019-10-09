using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class enemySkill_properties {

	[System.Serializable]
	public class extraEffectCallBack: UnityEvent<Data>{
	}
	[System.Serializable]
	public class voidZoneCallBack: UnityEvent<voidzoneData>{
	}
	[System.Serializable]
	public class counterEventCallBack: UnityEvent<counterEventData>{
	}
	public string skillName;
	//public Sprite skillIcon;
	public bool equipped;
	public bool assigned;
	public bool bossOnlyP1;
	public bool bossOnlyP2;
	public bool bossOnlyP3;
	public string displayName;
	public float attackMovementSpeed;
	public string animationType;
	public string animationCastingType;
	public string animationRepeatCasting;
	public bool loopAnimation;
	public string skillDesc;
	public bool skillActive;
	public bool skillConfirm;
	public int skillCost;
	public float skillPower;
	public float magicPower;
	public float duration;
	public float castTime;
	public bool castTimeReady;
	public float skillCooldown;
	public bool isSpell;
	public bool isFlat;
	public bool doesDamage;
	public bool movesToTarget;
	public bool hasVoidzone;
	public enum voidZoneType{
		All,
		Vline,
		Hline
	}
	public float eventDuration;
	public bool healsDamage;
	public bool self;
	public bool enemy;
	public bool friendly;
	public bool allFriendly;
	public bool allEnemy;
	//public UnityEvent extraEffectMethods;
	public extraEffectCallBack extraEffectCallBackMethod;
	public voidZoneCallBack voidZoneCallBackMethod;
	public counterEventCallBack counterEventCallBackMethod;
	//public class extraEffectMethodsCallBack : UnityEvent<extraEffectMethods>{
	//}
	//public extraEffectMethodsCallBack callback;
	//status controller
	public status statusScript;
	public List<statusEffectOne> statusEffectGroup;
	public enum statusEffectOne{
		None,
		Haste,
		Poison,
		ArmorDown,
		ArmorUp,
		Regen,
		Stun,
		AmbitionsEdge,
		Barrier,
		DamageImmune
	}

	//New Status System
	public List<singleStatus> singleStatusGroup = new List<singleStatus>();
		//attach status to target
	public void AttachStatus( List<singleStatus> singleStatusGroup, status targetStatus ){
		for( int i = 0; i < singleStatusGroup.Count; i++ ){
			targetStatus.RunStatusFunction( singleStatusGroup[i], skillPower, duration );
		}
	}
	
	//skill custom script controller
	//public skillCustomEffect skillCustomEffectSelection;

}

