using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class enemySkill : ScriptableObject {
	//[System.Serializable]
	//public class extraEffectCallBack: UnityEvent<Data>{
	//}
	[System.Serializable]
	public class voidZoneCallBack: UnityEvent<voidzoneData>{
	}
	[System.Serializable]
	public class counterEventCallBack: UnityEvent<counterEventData>{
	}

	[Header("Default Skill Variables:")]
	public string skillName;
	public int buttonID;
	public bool equipped;
	public bool assigned;
	public string displayName;
	public float attackMovementSpeed = 50f;
	public string animationType;
	public string animationCastingType;
	public string animationRepeatCasting;
	public string skinChange;
	public bool loopAnimation;
	public string skillDesc;
	public bool skillActive;
	public bool skillConfirm;
	public int skillCost;
	//[System.NonSerialized]
	public float skillPower;
	public float newSP;
	public float magicPower;
	public float newMP;
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
	[Header("Target choices:")]
	public bool self;
	public bool enemy;
	public bool friendly;
	public bool allFriendly;
	public bool allEnemy;
    public bool summon;
	[Header("Summon Creature:")]
	public List<GameObject> summonedObjects = new List<GameObject>();
	[Header("FX Animation:")]
	public GameObject fxObject;
    public fxPosEnum fxPos;
    public enum fxPosEnum{
        center,
        bottom,
        front,
        top
    }
	[Header("Extra Effect:")]
	public ExtraEffectEnum ExtraEffect;
	public enum ExtraEffectEnum{
		None,
		Dispel,
		BonusDamage,
		RunSkill
	}
	[Header("Enemy Skill Variables:")]
	public bool monsterPanel = false;
	public voidZoneType voidZoneTypes;
	public enum voidZoneType{
		All,
		Vline,
		Hline,
		Random
	}
	public bool bossOnlyP1;
	public bool bossOnlyP2;
	public bool bossOnlyP3;
	public bool hasVoidzone;
	public float eventDuration;
	public counterEventCallBack counterEventCallBackMethod;
	public enemySkill ExtraSkillToRun;

	//New Status System
	[Header("Status Effects:")]
	public List<singleStatus> singleStatusGroup = new List<singleStatus>();
	public bool statusDispellable = true;
	[Header("Friendly Status Effects:")]
	public List<singleStatus> singleStatusGroupFriendly = new List<singleStatus>();
	public bool statusFriendlyDispellable = true;
		//attach status to target
	public void AttachStatus( List<singleStatus> singleStatusGroup, status targetStatus, float power, float duration ){
		for( int i = 0; i < singleStatusGroup.Count; i++ ){
			targetStatus.RunStatusFunction( singleStatusGroup[i], power, duration, onHitSkillEnemy:onhitSkilltoRun );
		}
	}
	public enemySkill onhitSkilltoRun;

	//Run Extra Skill Effects
	public void RunExtraEffect( Data data, GameObject caster = null ){
		switch( ExtraEffect ){
			case ExtraEffectEnum.None:
				break;
			case ExtraEffectEnum.Dispel:
				Dispel(data);
				break;
			case ExtraEffectEnum.BonusDamage:
				BonusDamage(data);
				break;
			case ExtraEffectEnum.RunSkill:
				RunExtraSkill( data.enemySkill, caster );
				break;
			}

	}

	//Run Extra Skill
	public void RunExtraSkill( enemySkill enemySkill, GameObject caster ){
		var casterSkillSelection = caster.transform.parent.GetComponent<enemySkillSelection>(); 
		casterSkillSelection.PrepSkillNew( enemySkill );
	}

	//summon object
	public void SummonCreatures( List<GameObject> targetCreatures ){
        for( int i = 0; i < targetCreatures.Count; i++ ) {
            var creatureData = (GameObject)Instantiate( GameObject.Find( "singleMinionData" ), GameObject.Find( "Panel MinionData" ).transform );
            var panel = GetRandomPanel();
            if( panel ){
                creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<dataDisplaytwo>().On = true;
                var newCreature = (GameObject)Instantiate( targetCreatures[i], creatureData.transform );
                panel.GetComponent<movementPanelController>().currentOccupier = newCreature;
                creatureData.transform.GetChild(1).GetChild(0).gameObject.name = newCreature.GetComponent<character_data>().role + i.ToString() + "status";
                newCreature.gameObject.name = newCreature.GetComponent<character_data>().role + i.ToString();
                newCreature.GetComponent<character_data>().currentPanel = panel;
                newCreature.GetComponent<character_data>().role = newCreature.GetComponent<character_data>().role + i.ToString();
                creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<dataDisplaytwo>().SetDataObjects( i );
                panel.GetComponent<movementPanelController>().Start();
                newCreature.transform.Find("Animations").GetComponent<SkeletonAnimation>().state.SetAnimation(0, "intro", false);
                newCreature.transform.Find("Animations").GetComponent<SkeletonAnimation>().state.AddAnimation(0, "idle", true, 0 );
                //StartCoroutine( DelayedStart( newCreature ) );
                globalEffectsController.callEffectTarget( newCreature, fxObject, fxPos.ToString() );
            } else {
                Debug.Log( "No Panel" );
            }
        }
	}

	//removes x buffs from an enemy
	public void Dispel( Data data ){
		//target = skill_targetting.instance.currentTarget;
		List<GameObject> targets = data.target;
		foreach (var target in targets) {
			var targetStatus = target.GetComponent<status>();
			var targetSpawnUI = target.GetComponent<spawnUI>();
			//print ("running" + target);
			foreach( singleStatus singlestatus in targetStatus.statusListSO ) {
			if( singlestatus.statusposition <= 1 && singlestatus.statusposition >= 0 && singlestatus.buff && singlestatus.debuffable && singlestatus.active ) {
					targetSpawnUI.RemoveLabel( singlestatus.name, singlestatus.buff );
					targetStatus.RunStatusFunction( singlestatus, statusOff:true );
					//singlestatus.ChosenStatusToTurnOff();
						//print ("Successfully Debuffed " + singlestatus.name);
			} else if ( singlestatus.statusposition <= 2 && singlestatus.statusposition >= 0 && singlestatus.buff && !singlestatus.debuffable && !singlestatus.active ){
					//print ("Failed to Debuff " + singlestatus.name);
				}
			} 
		}
	}

	//Does extra damage based on status effect
	public void BonusDamage( Data data ){ 
		List<GameObject> targets = data.target;
		foreach (var target in targets) {
			//target = skill_targetting.instance.currentTarget;
			var targetStatus = target.GetComponent<status>();
			//var multiplier;
			//checks if poison is on the target
			if( targetStatus.DoesStatusExist("poison") ){
				data.modifier = 1.8f;
				//targetStatus.PoisonOn( 4, true, 0 );
			} else {
				data.modifier = 1f;
			}
		}
		//return multiplier;
	}

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

	GameObject GetRandomPanel( GameObject[] panels = null ){
        if( panels == null ){
            panels = !monsterPanel ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
		}
        var chosenPanels = new List<GameObject>();
        foreach (var panel in panels) {
            if( !panel.GetComponent<movementPanelController>().isOccupied ){
                chosenPanels.Add( panel );
            }
        }   
        var randomPanelNumber = Random.Range (0, chosenPanels.Count );
		return chosenPanels[randomPanelNumber];
	}

	public void ShowVoidPanel( voidZoneType voidZoneEnumVar, bool monsterPanels = false ){
		var allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
		//var randomPanelNumber = Random.Range (0,allPanels.Length);
		//var safePanelScript = allPanels[randomPanelNumber].GetComponent<movementPanelController>();

		//for( int i = 0; allPanels.Length > i; i++ ){
		//	var panelScript = allPanels[i].GetComponent<movementPanelController>();
			switch( voidZoneEnumVar ){
				case voidZoneType.All:
					for( int i = 0; allPanels.Length > i; i++ ){
						var panelScript = allPanels[i].GetComponent<movementPanelController>();
						panelScript.VoidZoneMark();
					}
				break;
				case voidZoneType.Hline:
					//print("success VoidZone Horizontal Line");
				break;
				case voidZoneType.Vline:
					//print("success VoidZone Vertical Line");
				break;
				case voidZoneType.Random:
                    GetRandomPanel( allPanels ).GetComponent<movementPanelController>().VoidZoneMark();
                break;
			}
		//}
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

	void Start(){
        
	}
}
