using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;

public class skill_selection : MonoBehaviour {

	[SpineAnimation]
	SkeletonAnimation skeletonAnimation;

	public enum skillOne{ 
		skillbutton1,
		skillbutton2,
		skillbutton3,
	}

	public enum tankSkills{
		skillbutton1,
		skillbutton2,
		skillbutton3,
	}

	public enum dpsSkills{
		skillbutton1,
		skillbutton2,
		skillbutton3,
	}

	public skillOne selectedSkill;
	public tankSkills selectedTankSkill;
	public dpsSkills selectedDpsSkill;
	public GameObject skillConfirmObject;
	private character_select selectedRole;
	private skill_confirm skillConfirmScript;
	private sortbuttondata sortButtonsScript;
	private skill_cd skillCdScript;
	private character_select characterSelectScript;
	private int skillCost;
	private bool skillReady;
	//public bool skillonCD;
	public GameObject skillObject;
	private Image skillObjectScript;
	private Image buttonObject;
	//Actual skill ID from skillList
	public int skillID;
	public int buttonID;
	public bool skillinprogress = false;
	private Task CoolDownTask;
	private Task GlobalCD;
	private Task waitForFriendlyTargetTask;
	private Task waitForTargetTask;
	private Task waitForTargetTaskNew;
	private gameEffects gameEffectsScript;
    private SkillModel skillInWaiting;

	#region PrepSkill functions
    bool waitingForSelection = false;
    float power; //Set Power
    public void PrepSkillNew( SkillModel classSkill, bool weaponSkill = true ){
        if( CheckSkillAvail( classSkill ) && true ){
            if( !waitingForSelection ){
                SkillActiveSet( classSkill, true ); //Set that skill has being used or waiting to be used
                GetTargets(classSkill, weaponSkill:weaponSkill);
            }
        }
    }

    public void SkillComplete( SkillModel classSkill, List<GameObject> targets, bool weaponSkill = true, GameObject player = null ){
        //send Event to EventManager
        EventManager.BuildEvent( "OnSkillCast", eventCallerVar: player );

        waitingForSelection = false;
        if( classSkill.isFlat ){ //Set Power to spell or skill type
            power = classSkill.isSpell ? classSkill.magicPower : classSkill.skillPower;
        } else {
            power = classSkill.isSpell ? classSkill.newMP : classSkill.newSP;
        }
        Data data = new Data(); 
        data.target = targets;
        data.classSkill = classSkill;
        //if( classSkill.ExtraEffect.ToString() != "None" ){ 
        classSkill.RunExtraEffect(data); 
        //};//Run Extra Effects if there are any
        
        SetAnimations( classSkill ); //Play Animations
        DealHealDmg( classSkill, targets, power * data.modifier, player ); //Deal or Heal Damage to Targets, Also adds Status Effects
        SkillActiveSet( classSkill, false ); //Set that skill is ready to be used again
        StartCoroutine(cooldown( classSkill.skillCooldown, classSkill ));
        CoolDownTask = new Task( cooldownTimer( classSkill.skillCooldown, classSkill, classSkill.Class.ToString(), weaponSkill ) );
    }

    private void SkillActiveSet( SkillModel classSkill, bool setActive, bool skillCancel = false ){
        skillCdScript.skillActive = setActive; //globally sets a skill in progress
        if ( setActive || skillCancel ){ //turn skill active on begin but inactive on cancel
            classSkill.skillActive = setActive;
            skillInWaiting = classSkill;
        }
        if( waitForTargetTaskNew != null ){
            waitForTargetTaskNew.Stop();
            Time.timeScale = 1f;
        } 
       // classSkill.skillConfirm = setActive;
        skillinprogress = setActive;
        if (!setActive || skillCancel ){ 
            finalTargets.Clear(); 
            waitingForSelection = false;
            skillInWaiting = null;
        }
    }

	private List<GameObject> finalTargets = new List<GameObject>();
    private void GetTargets( SkillModel classSkill, bool randomTarget = false, bool weaponSkill = true ){
		var player = GameObject.Find( classSkill.Class.ToString() );
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		if( classSkill.self ){ finalTargets.Add( player ); }
		if( classSkill.allEnemy ){ finalTargets.AddRange( enemies ); }
		if( classSkill.allFriendly ){ finalTargets.AddRange( players ); }
		if( classSkill.friendly || classSkill.enemy ){
			skill_targetting.instance.currentTarget = null;
			print ("select a Target");
			waitForTargetTaskNew = new Task( waitForTargetNew( classSkill, player, weaponSkill ) );
            return;
		} else {
			SkillComplete( classSkill, finalTargets, weaponSkill, player: player );
            return;
		}
	}
    IEnumerator waitForTargetNew( SkillModel classSkill, GameObject player, bool weaponSkill = true )
	{
		waitingForSelection = true;
		gameEffectsScript.SlowMo(0.01f);
		while( skill_targetting.instance.currentTarget == null ) {
            if( player.GetComponent<status>().DoesStatusExist("stun") ){
                SkillActiveSet( classSkill, false, true );
                yield break;
            }
			yield return 0;
        } 
		finalTargets.Add( skill_targetting.instance.currentTarget );
        characterSelectScript.GetClassObject().GetComponent<character_data>().target = skill_targetting.instance.currentTarget.GetComponent<character_data>();
		skill_targetting.instance.currentTarget = null;
		//PrepSkillNew( classSkill, weaponSkill );
		Time.timeScale = 1f;
        SkillComplete( classSkill, finalTargets, weaponSkill, player: player );
	}

	private void SetAnimations( SkillModel classSkill ){
		GameObject target = GameObject.Find( classSkill.Class.ToString() );
		SkeletonAnimation targetAnimData = target.transform.Find("Animations").GetComponent<SkeletonAnimation>();
		animationControl targetAnimControl = target.transform.Find("Animations").GetComponent<animationControl>();
		characterMovementController characterMovementScript = target.GetComponent<characterMovementController>();;
		//call animation variable
		if( classSkill.animationType != null ){
			targetAnimControl.inAnimation = true;
			targetAnimData.state.SetAnimation(0, classSkill.animationType, classSkill.loopAnimation);
			var animationDuration = targetAnimData.state.SetAnimation(0, classSkill.animationType, classSkill.loopAnimation).Animation.duration;
			StartCoroutine( busyAnimation( animationDuration, target ));
			if( classSkill.attackMovementSpeed > 0 ){
				attackMovement MovementScript = target.GetComponent<attackMovement>();
				character_data playerCharData = target.GetComponent<character_data>();
				playerCharData.isAttacking = true;
				MovementScript.StartMovement(classSkill.attackMovementSpeed);
			} else {
				var idleAnim = characterMovementScript.idleAnim;
				targetAnimData.state.AddAnimation(0, idleAnim, true, 0 );
			}
		}
		
	}

	private void DealHealDmg( SkillModel classSkill, List<GameObject> targets, float power, GameObject player ){
		//GameObject dmgSourceObj = GameObject.Find( classSkill.Class.ToString() );
		GameObject dmgSourceObj = player;
        var effectsControllerScript = player.GetComponent<effectsController>();
        var playerCalculateDmg = player.GetComponent<calculateDmg>();
        playerCalculateDmg.dueDmgTargets.RemoveAll(t => t);
        playerCalculateDmg.dueDmgTargets.AddRange(targets);
        foreach (var target in targets) {
			var targetData = target.GetComponent<character_data>();
			var targetCalculateDmgScript = target.GetComponent<calculateDmg>();
			if ( classSkill.fxObject != null ){
				effectsControllerScript.callEffectTarget( target, classSkill.fxObject );
			}
			if( target.tag == "Enemy" && targetData.isAlive ){
				foreach (var status in classSkill.singleStatusGroup) {

					status.debuffable = classSkill.statusDispellable;
				}
				targetData.incomingDmg = power;
                if( classSkill.doesDamage ){ 
                    targetCalculateDmgScript.calculatedamage( classSkill:classSkill, skeletonAnimationVar:dmgSourceObj.transform.Find("Animations").GetComponent<SkeletonAnimation>(), dmgSourceVar: dmgSourceObj, isSpell:classSkill.isSpell ); 
                };
				classSkill.AttachStatus( classSkill.singleStatusGroup, target.GetComponent<status>(), power, classSkill );
            } else if( target.tag == "Player" && targetData.isAlive ){ 
				foreach (var status in classSkill.singleStatusGroupFriendly) {
					status.debuffable = classSkill.statusFriendlyDispellable;
				}
				targetData.incomingHeal = power;
				if( classSkill.healsDamage ){ targetCalculateDmgScript.calculateHdamage(); };
				classSkill.AttachStatus( classSkill.singleStatusGroupFriendly, target.GetComponent<status>(), power, classSkill );
			}
		}
		dmgSourceObj.GetComponent<character_data>().actionPoints -= classSkill.skillCost;
	}

	private bool CheckSkillAvail( SkillModel classSkill ){
		GameObject currentChar = GameObject.Find( classSkill.Class.ToString() );
		var availActionPoints = currentChar.GetComponent<character_data>().actionPoints;
		if ( 
			availActionPoints >= classSkill.skillCost && 
			!classSkill.skillActive && 
			//!classSkill.skillConfirm &&
			!skillCdScript.skillActive )
		{
			return true;
		} else {
			print( classSkill.displayName + " is not available or waiting for target" );
			return false;
		}
	}

    //function to cancel skill
    public void SkillCancel(){
        if( waitForTargetTaskNew != null ){
            waitForTargetTaskNew.Stop();
            Time.timeScale = 1f;
        } 
        if( skillInWaiting != null ){
            SkillActiveSet( skillInWaiting, false, true );
        }
        /*skillInWaiting.skillActive = false;
        skillCdScript.skillActive = false;
        skillinprogress = false;*/
    }

	#endregion

	IEnumerator busyAnimation(float waitTime, GameObject player)
	{
		yield return new WaitForSeconds(waitTime);
		var playerAnimationControl = player.transform.Find("Animations").GetComponent<animationControl>();
		playerAnimationControl.inAnimation = false;
		character_data playerCharData = player.GetComponent<character_data>();
		playerCharData.isAttacking = false;
		//print ("Animation Que clear");
	}

	IEnumerator cooldown(float waitTime, SkillModel classSkill)
	{
		yield return new WaitForSeconds(waitTime);
		classSkill.skillActive = false;
	}

	//Run skill On Key Press 1,2,3
	void KeyPressSkill( string role, int keyNumber ){
        if( role == "Healer" ){
            PrepSkillNew( sortButtonsScript.healerSkillClass[keyNumber] );
        } else if ( role == "Tank" ){
		    PrepSkillNew( sortButtonsScript.tankSkillClass[keyNumber] );
	    } else if ( role == "Dps" ){
            PrepSkillNew( sortButtonsScript.dpsSkillClass[keyNumber] );
        }
    }

	//Check if character is ready to perform Skill
	bool IsCharBusy( GameObject character ){
		return character.transform.Find("Animations").GetComponent<animationControl>().inAnimation;
	}
	
	public void chosenSkill(){
		if ( characterSelectScript.healerSelected == true && !IsCharBusy( GameObject.Find("Walker") ) ){
			switch ( selectedSkill )
			{
			case skillOne.skillbutton1:
				PrepSkillNew( sortButtonsScript.healerSkillClass[0] );
				break;
			case skillOne.skillbutton2:
				PrepSkillNew( sortButtonsScript.healerSkillClass[0] );
				break;
			case skillOne.skillbutton3:
				PrepSkillNew( sortButtonsScript.healerSkillClass[1] );
				break;
			default:
				print ("nothing selected");
				break;
			}
		} else if ( characterSelectScript.tankSelected == true && !IsCharBusy( GameObject.Find("Guardian") ) ){
			switch ( selectedTankSkill )
			{
			case tankSkills.skillbutton1:
				PrepSkillNew( sortButtonsScript.tankSkillClass[0] );
				break;
			case tankSkills.skillbutton2:
				PrepSkillNew( sortButtonsScript.tankSkillClass[0] );
				break;
			case tankSkills.skillbutton3:
				PrepSkillNew( sortButtonsScript.tankSkillClass[1] );
				break;
			default:
				print ("nothing selected");
				break;
			}
		} else if ( characterSelectScript.dpsSelected == true && !IsCharBusy( GameObject.Find("Stalker") ) ){
			//getSkillname();
			//checks dps skills
			switch ( selectedDpsSkill )
			{
			case dpsSkills.skillbutton1:
				PrepSkillNew( sortButtonsScript.dpsSkillClass[0]);
				break;
			case dpsSkills.skillbutton2:
				PrepSkillNew( sortButtonsScript.dpsSkillClass[0]);
				break;
			case dpsSkills.skillbutton3:
				PrepSkillNew( sortButtonsScript.dpsSkillClass[1]);
				break;
			default:
				print ("nothing selected");
				break;
			}
		}
	}

	private Text buttonLabel;
	//timer for cooldown display - takes float amount from fill amount every 1s
	IEnumerator cooldownTimer( float cdtime, SkillModel skillEffectsVar, string roleVar, bool weaponSkill ){
	    var classSkillCDIcon = GameObject.Find("image-classskillIcon").GetComponent<Image>();
        if ( weaponSkill ){
            skillObjectScript.fillAmount = 1f;
        }else{ 
            classSkillCDIcon.fillAmount = 1f;
        };
		while( skillEffectsVar.skillActive == true ){
			yield return new WaitForSeconds(1f);
			skillEffectsVar.currentCDAmount += 1f;
			float timeSpent;
			timeSpent = 1f/cdtime;
			if( roleVar == characterSelectScript.GetClassRole() ){
				if (weaponSkill){skillObjectScript.fillAmount -= timeSpent;}else{ classSkillCDIcon.fillAmount -= timeSpent;};
			}
		}
		skillEffectsVar.currentCDAmount = 0f;
	}

	//Clear CD timer
	public void ClearCD(){
		skillObjectScript.fillAmount = 0f;
	}

	//Run to check if skills are on Cooldown - if so set the fill amount to the remaining time left
	public void CooldownDisplayCheck( float cdtime, List<SkillModel> skillEffectsVar, int buttonIDvar ){
			if( buttonID == buttonIDvar ){
				float timeLeft = 1f/cdtime * skillEffectsVar[skillID].currentCDAmount;
				//print (timeLeft);
				skillObjectScript.fillAmount = 1f - timeLeft;
			}
	}

	void CanAffordSkill(){
		if( selectedRole.healerSelected == true ){
			skillCost = sortButtonsScript.healerSkillCosts[skillID];
			var healPlayer = GameObject.FindGameObjectWithTag("Healer");
			var healPlayerData = healPlayer.transform.parent.GetComponent<character_data>();
			if( !healPlayerData.isAlive || skillCost > healPlayerData.actionPoints || IsCharBusy( GameObject.Find("Walker") )){
				buttonObject.color = new Color(0.9f, 0.2f, 0.2f);
			} else {
				buttonObject.color = new Color(1f, 1f, 1f);
			}
		} else if ( selectedRole.tankSelected == true ) {
			skillCost = sortButtonsScript.tankSkillCosts[skillID];
			var tankPlayer = GameObject.FindGameObjectWithTag("Tank");
			var tankPlayerData = tankPlayer.transform.parent.GetComponent<character_data>();
			if( !tankPlayerData.isAlive || skillCost > tankPlayerData.actionPoints || IsCharBusy( GameObject.Find("Guardian") ) ){
				buttonObject.color = new Color(0.9f, 0.2f, 0.2f);
			} else {
				buttonObject.color = new Color(1f, 1f, 1f);
			}
		} else if ( selectedRole.dpsSelected == true ) {
			skillCost = sortButtonsScript.dpsSkillCosts[skillID];
			var dpsPlayer = GameObject.FindGameObjectWithTag("Dps");
			var dpsPlayerData = dpsPlayer.transform.parent.GetComponent<character_data>();
			if( !dpsPlayerData.isAlive || skillCost > dpsPlayerData.actionPoints || IsCharBusy( GameObject.Find("Stalker") ) ){
				buttonObject.color = new Color(0.9f, 0.2f, 0.2f);
			} else {
				buttonObject.color = new Color(1f, 1f, 1f);
			}
		}
	}

	// Use this for initialization
	void Awake () {
		gameEffectsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<gameEffects>();
		selectedRole = skillConfirmObject.GetComponent<character_select>();
		skillConfirmScript = skillConfirmObject.GetComponent<skill_confirm>();
		skillCdScript = skillConfirmObject.GetComponent<skill_cd>();
		characterSelectScript = skillConfirmObject.GetComponent<character_select>();
		sortButtonsScript = skillConfirmObject.GetComponent<sortbuttondata>();
		buttonObject = GetComponent<Image>();
		skillObjectScript = skillObject.GetComponent<Image>();
	}

	void Start(){
	}

	// Update is called once per frame
	void Update () {
		CanAffordSkill();
		//recieve Key press 1,2,3,4
		if( Input.GetKeyDown( KeyCode.Alpha1 ) && buttonID == 0 && !IsCharBusy( GameObject.Find( characterSelectScript.GetClassRole() ) ) ){
			KeyPressSkill( characterSelectScript.GetClassRoleCaps() , 0 );
		}
		if( Input.GetKeyDown( KeyCode.Alpha2 ) && buttonID == 1 && !IsCharBusy( GameObject.Find( characterSelectScript.GetClassRole() ) ) ){
			KeyPressSkill( characterSelectScript.GetClassRoleCaps() , 1 );
		}
		//Cancel Skill
		if( Input.GetKeyUp( KeyCode.Escape ) ){
			SkillCancel();
		}

	}
}
