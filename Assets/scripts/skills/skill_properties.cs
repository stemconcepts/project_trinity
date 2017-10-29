using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class skill_properties {

	[System.Serializable]
	public class extraEffectCallBack: UnityEvent<Data>{
	}
	[System.Serializable]
	public class voidZoneCallBack: UnityEvent<voidzoneData>{
	}
	[System.Serializable]
	public class counterEventCallBack: UnityEvent<counterEventData>{
	}
	[Header("Default Skill Variables:")]
	public string skillName;
	public Sprite skillIcon;
	public int buttonID;
	public bool equipped;
	public bool assigned;
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
	public float currentCDAmount;
	public bool isSpell;
	public bool isFlat;
	public bool doesDamage;
	public bool movesToTarget;
	public bool healsDamage;
	public targetChoice targetChoices;
	public enum targetChoice{
		Self,
		Enemy,
		Friendly,
		AllFriendly,
		Allenemy
	}
	public bool self;
	public bool enemy;
	public bool friendly;
	public bool allFriendly;
	public bool allEnemy;
	//public UnityEvent extraEffectMethods;
	public extraEffectCallBack extraEffectCallBackMethod;
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
	[Space(20)]

	[Header("Enemy Skill Variables:")]
	public voidZoneType voidZoneTypes;
	public enum voidZoneType{
		All,
		Vline,
		Hline
	}
	public bool bossOnlyP1;
	public bool bossOnlyP2;
	public bool bossOnlyP3;
	public bool hasVoidzone;
	public float eventDuration;
	public voidZoneCallBack voidZoneCallBackMethod;
	public counterEventCallBack counterEventCallBackMethod;

//	targetChoice GetTarget( targetChoice enumSelect ){
//		
//	}

	//skill custom script controller

	GameObject GetSafePanel( GameObject[] panels ){
		//var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
		var randomPanelNumber = Random.Range (0,panels.Length);
		var safePanelScript = panels[randomPanelNumber].GetComponent<movementPanelController>();
		if( safePanelScript.currentOccupier == null ){
			return panels[randomPanelNumber];
		}else {
			return GetSafePanel( panels );
			//return null;
		}
	}

	public void ShowVoidPanel( voidZoneType voidZoneEnumVar ){
		var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
		//var randomPanelNumber = Random.Range (0,allPanels.Length);
		//var safePanelScript = allPanels[randomPanelNumber].GetComponent<movementPanelController>();

		for( int i = 0; allPanels.Length > i; i++ ){
			var panelScript = allPanels[i].GetComponent<movementPanelController>();
			switch( voidZoneEnumVar ){
				case voidZoneType.All:
					panelScript.VoidZoneMark();
				break;
				case voidZoneType.Hline:
					//print("success VoidZone Horizontal Line");
				break;
				case voidZoneType.Vline:
					//print("success VoidZone Vertical Line");
				break;
			}
		}
		switch( voidZoneEnumVar ){
		case voidZoneType.All:
			GetSafePanel( allPanels ).GetComponent<movementPanelController>().SafePanel();
			//safePanelScript.SafePanel();
			break;
		case voidZoneType.Hline:
			//print("success VoidZone Horizontal Line");
			break;
		case voidZoneType.Vline:
			//print("success VoidZone Vertical Line");
			break;
		}
	}

	public void ClearVoidPanel(){
		var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
		for( var i = 0; allPanels.Length > i; i++ ){
			var panelScript = allPanels[i].GetComponent<movementPanelController>();
			panelScript.ClearVoidZone();
		}
	}

}

