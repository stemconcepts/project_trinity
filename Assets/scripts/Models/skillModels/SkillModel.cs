using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class SkillData{
         public float modifier = 1f;
         public List<Character_Manager> target;
         public Character_Manager caster;
         public SkillModel skillModel;
    }
    
    [System.Serializable]
    public class SkillModel : ScriptableObject {
        [Header("Enemy Skill:")]
        public bool enemySkill = true;
    	[Header("Skill Role:")]
        [ConditionalHide("enemySkill", true, true)]
    	public ClassEnum Class;
    	public enum ClassEnum{
    		Guardian,
    		Walker,
    		Stalker
    	}
    	[Header("Default Skill Variables:")]
    	public string skillName;
        [ConditionalHide("enemySkill", true, true)]
    	public Sprite skillIcon;
        [ConditionalHide("enemySkill", true, true)]
    	public int buttonID;
        [ConditionalHide("enemySkill", true, true)]
    	public bool equipped;
        [ConditionalHide("enemySkill", true, true)]
    	public bool learned;
        [ConditionalHide("enemySkill", true, true)]
    	public bool assigned;
    	public string displayName;
    	public float attackMovementSpeed;
    	public string animationType;
    	public string animationCastingType;
    	public string animationRepeatCasting;
    	public bool loopAnimation;
        [Multiline]
    	public string skillDesc;
    	public bool skillActive;
    	public bool skillConfirm;
    	public int skillCost;
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
        public bool summon;
    	[Header("Extra Effect:")]
    	public ExtraEffectEnum ExtraEffect;
    	public enum ExtraEffectEnum{
    		None,
    		Dispel,
    		BonusDamage,
            RunSkill
    	}
        public SkillModel ExtraSkillToRun;
        public subStatus subStatus;
        [Header("Forced Movement:")]
        public int forcedMoveAmount;
        public forcedMoveType forcedMove;
        public enum forcedMoveType{
            None,
            Back,
            Forward
        }
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
        public voidZoneType voidZoneTypes;
        public enum voidZoneType{
            All,
            Vline,
            Hline,
            Random
        }
        [ConditionalHide("enemySkill", true)]
        [Header("Enemy Skill Variables:")]
        public bool monsterPanel = false;
        [ConditionalHide("enemySkill", true)]
        public AssemblyCSharp.Skill_Manager.EnemyPhase bossPhase;
        [ConditionalHide("enemySkill", true)]
        public bool hasVoidzone;
        [ConditionalHide("enemySkill", true)]
        public float eventDuration;
        //public counterEventCallBack counterEventCallBackMethod;
    
    	//New Status System
    	[Header("Status Effects:")]
    	public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
    	public bool statusDispellable = true;
    	[Header("Friendly Status Effects:")]
    	public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
    	public bool statusFriendlyDispellable = true;
    		//attach status to target
    	public void AttachStatus( List<SingleStatusModel> singleStatusGroup, Status_Manager targetStatus, float power, SkillModel skillModel ){
            for( int i = 0; i < singleStatusGroup.Count; i++ ){
                var sm = new StatusModel{
                    singleStatus = singleStatusGroup[i],
                    power = power,
                    duration = skillModel.duration
                };
    			targetStatus.RunStatusFunction( sm );
    		}
    	}
    	public SkillModel onhitSkilltoRun;
    
    	//Run Extra Skill Effects
    	public void RunExtraEffect( SkillData data ){
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
                    RunExtraSkill( data );
                    break;
    		}
    	}

        //Run Extra Skill
        public void RunExtraSkill( SkillData data ){
            var casterSkillSelection = data.caster != null ? data.caster.GetComponent<Skill_Manager>(): null; 
            if( data.caster != null ){
                casterSkillSelection.PrepSkillNew( data.skillModel.ExtraSkillToRun );
               // casterSkillSelection.StartCasting( data.enemySkill.ExtraSkillToRun, data );
            }
        }
    
    	//removes x buffs from an enemy
    	public void Dispel( SkillData data ){
    		//target = skill_targetting.instance.currentTarget;
    		List<Character_Manager> targets = data.target;
    		foreach (var target in targets) {
    			var targetStatus = target.GetComponent<Status_Manager>();
    			var targetSpawnUI = target.GetComponent<Battle_Details_Manager>();
    			//var debuffPower = data.skillModel == null ? data.skillModel.skillPower : data.enemySkill.skillPower;
                var debuffPower = data.skillModel != null ? data.skillModel.skillPower : 0;   

    			//print ("running" + target);
                var positionId = 0;
    			foreach( StatusLabelModel activeStatus in targetStatus.GetAllStatusIfExist( true ) ) {
    				if( positionId < debuffPower && activeStatus.buff && activeStatus.dispellable ) {
    						targetSpawnUI.RemoveLabel( activeStatus );
    						targetStatus.RunStatusFunction( activeStatus.statusModel );
    						//singlestatus.ChosenStatusToTurnOff();
    						//print ("Successfully Debuffed " + singlestatus.name);
    				} else if ( positionId == debuffPower && activeStatus.buff && !activeStatus.dispellable && !activeStatus.statusModel.singleStatus.active ){
    						//print ("Failed to Debuff " + singlestatus.name);
    				}
                    positionId++;
    			} 
    		}
    	}
    
    	//Does extra damage based on status effect      
    	public void BonusDamage( SkillData data ){ 
    		data.modifier = 1f;
    		List<Character_Manager> targets = data.target;
    		foreach (var target in targets) {
    			var targetStatus = target.baseManager.statusManager;
                var statusList = targetStatus.GetAllStatusIfExist( false );
                foreach( var status in statusList ){
                    if ( status.statusModel.subStatus == data.skillModel.subStatus ){
                        data.modifier = data.skillModel.subStatus.modifierAmount;
                    }
                }
                /*if( targetStatus.DoesStatusExist("poison")){
    				data.modifier = 1.8f;
    				//targetStatus.PoisonOn( 4, true, 0 );
    			}*/
    		}
    		//return multiplier;
    	}
    
    	//summon object
        public void SummonCreatures( List<GameObject> targetCreatures ){
            for( int i = 0; i < targetCreatures.Count; i++ ) {
                var creatureData = (GameObject)Instantiate( GameObject.Find( "singleMinionData" ), GameObject.Find( "Panel MinionData" ).transform );
                var panel = GetRandomPanel();
                if( panel ){
                    creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<UI_Display_Text>().On = true;
                    var newCreature = (GameObject)Instantiate( targetCreatures[i], creatureData.transform );
                    panel.GetComponent<Panels_Manager>().currentOccupier = newCreature;
                    creatureData.transform.GetChild(1).GetChild(0).gameObject.name = newCreature.GetComponent<Character_Manager>().characterModel.role.ToString() + i.ToString() + "status";
                    newCreature.gameObject.name = newCreature.GetComponent<Character_Manager>().characterModel.role.ToString() + i.ToString();
                    newCreature.GetComponent<Character_Manager>().currentPanel = panel;
                    newCreature.GetComponent<Character_Manager>().characterModel.role = Character_Model.RoleEnum.minion;
                    creatureData.transform.GetChild(0).GetChild(0).GetComponentInChildren<UI_Display_Text>().SetDataObjects( i );
                    //panel.GetComponent<Panels_Manager>().Start();
                    newCreature.GetComponent<Animation_Manager>().skeletonAnimation.state.SetAnimation(0, "intro", false);
                    newCreature.GetComponent<Animation_Manager>().skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
                    //StartCoroutine( DelayedStart( newCreature ) );
                    globalEffectsController.callEffectTarget( newCreature, fxObject, fxPos.ToString() );
                } else {
                    Debug.Log( "No Panel" );
                }
            }
        }

        GameObject GetSafePanel( GameObject[] panels ){
            //var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
            var randomPanelNumber = Random.Range (0,panels.Length);
            var safePanelScript = panels[randomPanelNumber].GetComponent<Panels_Manager>();
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
                if( !panel.GetComponent<Panels_Manager>().isOccupied ){
                    chosenPanels.Add( panel );
                }
            }   
            var randomPanelNumber = Random.Range (0, chosenPanels.Count );
            return chosenPanels[randomPanelNumber];
        }
    
        public void ShowVoidPanel( voidZoneType voidZoneEnumVar, bool monsterPanels = false ){
            var randomRowNumber = Random.Range (0, 3 );
                switch( voidZoneEnumVar ){
                    case voidZoneType.All:
                        var allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
                        for( int i = 0; allPanels.Length > i; i++ ){
                            var panelScript = allPanels[i].GetComponent<Panels_Manager>();
                            panelScript.VoidZoneMark();
                        }
                        GetSafePanel( allPanels ).GetComponent<Panels_Manager>().SafePanel();
                    break;
                    case voidZoneType.Hline:
                        var HPanel = !monsterPanels ? GameObject.Find("FriendlyMovementPanel") : GameObject.Find("EnemyMovementPanel");
                        var HRow = HPanel.transform.GetChild( randomRowNumber );
                        foreach (Transform panel in HRow.transform )
                        {   
                            var panelScript = panel.GetComponent<Panels_Manager>();
                            panelScript.VoidZoneMark();   
                        }
                    break;
                    case voidZoneType.Vline:
                        
                    break;
                    case voidZoneType.Random:
                        allPanels = !monsterPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
                        GetRandomPanel( allPanels ).GetComponent<Panels_Manager>().VoidZoneMark();
                    break;
                }
        }
    
        public void ClearVoidPanel(){
            var allPanels = GameObject.FindGameObjectsWithTag("movementPanels");
            for( var i = 0; allPanels.Length > i; i++ ){
                var panelScript = allPanels[i].GetComponent<Panels_Manager>();
                panelScript.ClearVoidZone();
            }
        }
    }
}
