using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class classSkills : ScriptableObject {
	[System.Serializable]
	public class extraEffectCallBack: UnityEvent<Data>{
	}

	[Header("Skill Role:")]
	public ClassEnum Class;
	public enum ClassEnum{
		Guardian,
		Walker,
		Stalker
	}
	[Header("Default Skill Variables:")]
	public string skillName;
	public Sprite skillIcon;
	public int buttonID;
	public bool equipped;
	public bool learned;
	public bool assigned;
	public string displayName;
	public float attackMovementSpeed = 50f;
	public string animationType;
	public string animationCastingType;
	public string animationRepeatCasting;
	public bool loopAnimation;
    [Multiline]
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
    public GameObject hitEffect;
	public bool doesDamage;
	public bool movesToTarget;
	public bool healsDamage;
	[Header("Target choices:")]
	public bool self;
	public bool enemy;
	public bool friendly;
	public bool allFriendly;
	public bool allEnemy;
	public bool targetAndSelf;
	[Header("Extra Effect:")]
	public ExtraEffectEnum ExtraEffect;
	public enum ExtraEffectEnum{
		None,
		Dispel,
		BonusDamage
	}
    public subStatus subStatus;
    [Header("FX Animation:")]
    public GameObject fxObject;
    public fxPosEnum fxPos;
    public enum fxPosEnum{
        center,
        bottom,
        front,
        top
    }

	//public UnityEvent extraEffectMethods;
	//public GameObject extraSkillObject;
	//private extra_skill_effects extraSkillEffects;
	//public extraEffectCallBack extraEffectCallBackMethod;
	//public class extraEffectMethodsCallBack : UnityEvent<extraEffectMethods>{
	//}
	//public extraEffectMethodsCallBack callback;
	//status controller
	//public status targetStatus;

	//New Status System
	[Header("Status Effects:")]
	public List<singleStatus> singleStatusGroup = new List<singleStatus>();
	public bool statusDispellable = true;
	[Header("Friendly Status Effects:")]
	public List<singleStatus> singleStatusGroupFriendly = new List<singleStatus>();
	public bool statusFriendlyDispellable = true;
		//attach status to target
	public void AttachStatus( List<singleStatus> singleStatusGroup, status targetStatus, float power, classSkills classSkill ){
        for( int i = 0; i < singleStatusGroup.Count; i++ ){
          //  singleStatusGroup[i].subStatus = classSkill.subStatus != null ? classSkill.subStatus : null;
			targetStatus.RunStatusFunction( singleStatusGroup[i], power, classSkill.duration );
		}
	}
	public enemySkill onhitSkilltoRun;

	//Run Extra Skill Effects
	public void RunExtraEffect( Data data ){
		switch( ExtraEffect ){
			case ExtraEffectEnum.None:
				break;
			case ExtraEffectEnum.Dispel:
				Dispel(data);
				break;
			case ExtraEffectEnum.BonusDamage:
				BonusDamage(data);
				break;
			}

	}

	//removes x buffs from an enemy
	public void Dispel( Data data ){
		//target = skill_targetting.instance.currentTarget;
		List<GameObject> targets = data.target;
		foreach (var target in targets) {
			var targetStatus = target.GetComponent<status>();
			var targetSpawnUI = target.GetComponent<spawnUI>();
			var debuffPower = data.enemySkill == null ? data.classSkill.skillPower : data.enemySkill.skillPower;

			//print ("running" + target);
            var positionId = 0;
			foreach( statussinglelabel activeStatus in targetStatus.GetAllStatusIfExist( true ) ) {
				if( positionId < debuffPower && activeStatus.buff && activeStatus.dispellable ) {
						targetSpawnUI.RemoveLabel( activeStatus.statusname, activeStatus.buff );
						targetStatus.RunStatusFunction( activeStatus.singleStatus, statusOff:true );
						//singlestatus.ChosenStatusToTurnOff();
						//print ("Successfully Debuffed " + singlestatus.name);
				} else if ( positionId == debuffPower && activeStatus.buff && !activeStatus.dispellable && !activeStatus.singleStatus.active ){
						//print ("Failed to Debuff " + singlestatus.name);
				}
                positionId++;
			} 
		}
	}

	//Does extra damage based on status effect      
	public void BonusDamage( Data data ){ 
		data.modifier = 1f;
		List<GameObject> targets = data.target;
		foreach (var target in targets) {
			var targetStatus = target.GetComponent<status>();
            var statusList = targetStatus.GetAllStatusIfExist( false );
            foreach( var status in statusList ){
                if ( status.singleStatus.subStatus == data.classSkill.subStatus ){
                    data.modifier = data.classSkill.subStatus.modifierAmount;
                }
            }
            /*if( targetStatus.DoesStatusExist("poison")){
				data.modifier = 1.8f;
				//targetStatus.PoisonOn( 4, true, 0 );
			}*/
		}
		//return multiplier;
	}

	void Start(){

	}
}
