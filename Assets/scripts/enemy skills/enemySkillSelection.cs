using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class enemySkillSelection : MonoBehaviour {
	[SpineAnimation]
	SkeletonAnimation skeletonAnimation;
	private animationControl enemyAnimationControl; 
	private effectsController effectsControllerScript;
	public bool casting;
	public bool bossphaseone = true;
	public bool bossphasetwo = false;
	public bool bossphasethree = false;
	private character_data enemyCharData;
	private enemySkills skillScript;
	private enemyskillAI skillAIScript;
	//private skill_selection skillSelectionScript;
	private skill_effects skillEffectsScripts;
	private float currentHealth;
	private float maxHealth;
	public float phaseTwoHealth;
	public float phaseThreeHealth;
	private enemyskill_confirm enemyskillConfirmScript;
	public bool skillinprogress = false;
	private Task CoolDownTask;
    private Task QueueTask;
	//Data data = new Data();
    Data savedData = new Data();
	private soundController soundContScript;
	private calculateDmg savedTargetDmgScript;
	private string savedCurrentSkillName;

	static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}

	//Check what Phase boss is in
	void BossPhaseCheck(){
		currentHealth = enemyCharData.Health;
		if( currentHealth < phaseTwoHealth && bossphaseone ){
			print ("phase 2");
			bossphaseone = false;
			bossphasetwo = true;
		} else
		if ( currentHealth <  phaseThreeHealth && bossphasetwo ){
			print ("phase 3");
			bossphaseone = false;
			bossphasetwo = false;
			bossphasethree = true;
		}
	}

	// Use this for initialization
	void Start () {
		maxHealth = enemyCharData.maxHealth;
		phaseTwoHealth = maxHealth - ( maxHealth/ 3f );
		phaseThreeHealth = maxHealth - ( maxHealth/ 1.5f);
		//SkillChoice ();
	}

	void Awake() {
		skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
		enemyAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
		skillEffectsScripts = GetComponent<skill_effects>();
		effectsControllerScript = GetComponent<effectsController>();
		enemyCharData = GetComponent<character_data>();
		enemyskillConfirmScript = GetComponent<enemyskill_confirm>();
		//enemySkillScript = GetComponent<enemySkills>();
		skillAIScript = GetComponent<enemyskillAI>();
		soundContScript = GetComponent<soundController>();
	}
	
	// Update is called once per frame
	void Update () {
		BossPhaseCheck();
	}

	#region PrepSkill functions
    public void OnEventSFX(Spine.TrackEntry state, Spine.Event e ){
        //var dmgSourceTarget = dmgSource.GetComponent<character_data>().target;
        if( e.Data.name == "SFX" ){
            savedData.enemySkill.SummonCreatures( savedData.enemySkill.summonedObjects );
        }
        skeletonAnimation.state.Event -= OnEventSFX;
    }

	private List<GameObject> finalTargets = new List<GameObject>();
	private List<GameObject> GetTargets( enemySkill enemySkill, bool randomTargetVar = false ){
		var caster = this.gameObject;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> playerList = new List<GameObject>( GameObject.FindGameObjectsWithTag("Player") );
        //add players that are alive to a new list
        var aliveList = new List<GameObject>();
        for (int i = 0; i < playerList.Count; i++)
        {
            var playerData = playerList[i].GetComponent<character_data>();
            if( playerData.isAlive ){
                aliveList.Add( playerList[i] );
            }
        }

		if( enemySkill.self ){ finalTargets.Add( caster ); }
		if( enemySkill.allEnemy ){ finalTargets.AddRange( aliveList ); }
		if( enemySkill.allFriendly ){ finalTargets.AddRange( enemies ); }
		if( enemySkill.friendly || enemySkill.enemy ){
			if ( caster.GetComponent<status>().DoesStatusExist("stun") ){
				//SkillCancel(); Create still cancel for enemies
				return null;
			} else { 
				var randomNumber = enemySkill.enemy ? Random.Range (0, aliveList.Count ) : Random.Range (0, enemies.Length );
				var randomTarget = enemySkill.enemy ? aliveList[randomNumber] : enemies[randomNumber];
                
				finalTargets.Add( randomTarget );
				return finalTargets;
			}
		} else {
			return finalTargets;
		}
	}

	private void SetAnimations( enemySkill enemySkill ){
		GameObject target = this.gameObject;
		SkeletonAnimation targetAnimData = target.transform.Find("Animations").GetComponent<SkeletonAnimation>();
		animationControl targetAnimControl = target.transform.Find("Animations").GetComponent<animationControl>();
		//call animation variable
		if( enemySkill.animationType != null ){
			if( enemySkill.skinChange != "" ){
					targetAnimData.skeleton.SetSkin("phase2");
				}
			targetAnimControl.inAnimation = true;
			targetAnimData.state.SetAnimation(0, enemySkill.animationType, enemySkill.loopAnimation);
			var animationDuration = targetAnimData.state.SetAnimation(0, enemySkill.animationType, enemySkill.loopAnimation).Animation.duration;
			StartCoroutine( busyAnimation( animationDuration, target, enemySkill ));
			if( enemySkill.attackMovementSpeed > 0 ){
				attackMovement MovementScript = target.GetComponent<attackMovement>();
				character_data playerCharData = target.GetComponent<character_data>();
				playerCharData.isAttacking = true;
				MovementScript.StartMovement(enemySkill.attackMovementSpeed);
			} else {
				targetAnimData.state.AddAnimation(0, "idle", true, 0 );
			}
		}
		
	}

	private void DealHealDmg( enemySkill enemySkill, List<GameObject> targets, float power ){
		var tank = GameObject.Find("Guardian");
		var tankData = tank.GetComponent<character_data>();
		var tankCalDmg = tank.GetComponent<calculateDmg>();
		foreach (var target in targets) {
			var targetData = target.GetComponent<character_data>();
			var calculateDmgScript = target.GetComponent<calculateDmg>();
			if ( enemySkill.fxObject != null ){
				effectsControllerScript.callEffectTarget( target, enemySkill.fxObject, enemySkill.fxPos.ToString() );
			}
			if( target.tag == "Player" && targetData.isAlive ){
				foreach (var status in enemySkill.singleStatusGroup) {
					status.debuffable = enemySkill.statusDispellable;
				}
				if( enemySkill.hasVoidzone && !tankData.inVoidCounter ){
					targetData.incomingDmg = power;
					if( enemySkill.doesDamage && targetData.inVoidZone ){ 
						calculateDmgScript.calculatedamage( enemySkill.skillName , this.gameObject.transform.Find("Animations").GetComponent<SkeletonAnimation>() ); };
					if( targetData.inVoidZone ){ enemySkill.AttachStatus( enemySkill.singleStatusGroup, target.GetComponent<status>(), power, enemySkill.duration ); }
				} else if( enemySkill.hasVoidzone && tankData.inVoidCounter ){
					tankData.incomingDmg = ( power * 1.5f);
					tankCalDmg.calculatedamage( enemySkill.displayName, this.gameObject.transform.Find("Animations").GetComponent<SkeletonAnimation>() );
				} else {
					targetData.incomingDmg = power;
					if( enemySkill.doesDamage ){ calculateDmgScript.calculatedamage( enemySkill.skillName , this.gameObject.transform.Find("Animations").GetComponent<SkeletonAnimation>() ); };
					enemySkill.AttachStatus( enemySkill.singleStatusGroup, target.GetComponent<status>(), power, enemySkill.duration ); 
				}
			} else if( target.tag == "Enemy" && targetData.isAlive ){ 
				foreach (var status in enemySkill.singleStatusGroupFriendly) {
					status.debuffable = enemySkill.statusFriendlyDispellable;
				}
				targetData.incomingHeal = power;
				if( enemySkill.healsDamage ){ calculateDmgScript.calculateHdamage(); };
				enemySkill.AttachStatus( enemySkill.singleStatusGroupFriendly, target.GetComponent<status>(), power, enemySkill.duration );
			}
		}
		if( enemySkill.hasVoidzone ){
			enemySkill.ClearVoidPanel();
		}
	}

	private bool CheckSkillAvail( enemySkill enemySkill ){
		if (  
			!enemySkill.skillActive && 
			!enemySkill.skillConfirm )
		{
			return true;
		} else {
			print( enemySkill.displayName + " is not available" );
			return false;
		}
	}

	private void SkillActiveSet( enemySkill enemySkill, bool setActive ){
		if (setActive){ enemySkill.skillActive = setActive; }
		enemySkill.skillConfirm = setActive;
		skillinprogress = setActive;
		if (!setActive ){ targets.RemoveAll( x => x != null );}
	}
	#endregion

	float power; //Set Power
	List<GameObject> targets = new List<GameObject>();
	public void PrepSkillNew( enemySkill enemySkill ){
		print( enemySkill.displayName + " casted" );
        Data data = new Data(); 
		if( CheckSkillAvail( enemySkill ) ){
            targets = targets.Count <= 0 ? GetTargets(enemySkill) : targets;
            data.target = targets;
            data.enemySkill = enemySkill;
            SkillActiveSet( enemySkill, true ); //Set that skill has being used or waiting to be used
			if( enemySkill.castTime <= 0 ){
				/*if ( targets == null ){ return; } //Stop function if There are no targets
				if( enemySkill.isFlat ){ //Set Power to spell or skill type
					power = enemySkill.isSpell ? enemySkill.magicPower : enemySkill.skillPower;
				} else {
					power = enemySkill.isSpell ? enemySkill.newMP : enemySkill.newSP;
				}*/
                SkillComplete( enemySkill, targets, data );
            } else {
                StartCasting( enemySkill, data );
            }
			/*if( !enemySkill.hasVoidzone ) {
                SkillComplete( enemySkill, targets, data );
				DealHealDmg( enemySkill, targets, power ); //Deal or Heal Damage to Targets, Also adds Status Effects
				SetAnimations( enemySkill ); //Play Animations
				if( enemySkill.ExtraEffect.ToString() != "None" ){ enemySkill.RunExtraEffect(data); };//Run Extra Effects if there are any
				SkillActiveSet( enemySkill, false ); //Set that skill is ready to be used again
				StartCoroutine(cooldown( enemySkill.skillCooldown, enemySkill ));
				CoolDownTask = new Task( cooldown( enemySkill.skillCooldown, enemySkill ) );
			} else if ( !enemySkill.castTimeReady && enemySkill.castTime > 0 ) {
                StartCasting( enemySkill, data );
				casting = true;
				StartCoroutine(castTime( enemySkill.castTime, enemySkill ));
				//call voidzone if true
				voidzoneData voidData = new voidzoneData();
				voidData.voidZoneDuration = enemySkill.castTime;
				enemySkill.ShowVoidPanel( enemySkill.voidZoneTypes, enemySkill.monsterPanel );

				//call animation variable
				if( enemySkill.animationType != null ){
					enemyAnimationControl.inAnimation = true;
					skeletonAnimation.state.SetAnimation(0, enemySkill.animationCastingType, false);
					skeletonAnimation.state.AddAnimation(0, enemySkill.animationRepeatCasting, true, 0 );
				}
				print ("casting ability");
			} else if ( enemySkill.castTimeReady ){
                SkillComplete( enemySkill, targets, data );
                Set Data
                Data data = new Data(); 
                data.target = targets;
                data.enemySkill = enemySkill;

				casting = false;
				enemySkill.castTimeReady = false;
				DealHealDmg( enemySkill, targets, power );
				SetAnimations( enemySkill );
                if( enemySkill.ExtraEffect.ToString() != "None" ){ enemySkill.RunExtraEffect(data, this.gameObject); };//Run Extra Effects if there are any
				SkillActiveSet( enemySkill, false ); //Set that skill is ready to be used again
                StartCoroutine(cooldown( enemySkill.skillCooldown, enemySkill ));
                CoolDownTask = new Task( cooldown( enemySkill.skillCooldown, enemySkill ) );
			}*/
		}
	}
    
    public void StartCasting( enemySkill enemySkill, Data data ){
                casting = true;
                StartCoroutine(castTime( enemySkill.castTime, enemySkill, data ));
                //call voidzone if true
                if( enemySkill.hasVoidzone ){
                    voidzoneData voidData = new voidzoneData();
                    voidData.voidZoneDuration = enemySkill.castTime;
                    enemySkill.ShowVoidPanel( enemySkill.voidZoneTypes, enemySkill.monsterPanel );
                }
                //call animation variable
                if( enemySkill.animationType != null ){
                    enemyAnimationControl.inAnimation = true;
                    skeletonAnimation.state.SetAnimation(0, enemySkill.animationCastingType, false);
                    skeletonAnimation.state.AddAnimation(0, enemySkill.animationRepeatCasting, true, 0 );
                }
    }

    public void SkillComplete( enemySkill enemySkill, List<GameObject> targets, Data data ){
                if( enemySkill.isFlat ){ //Set Power to spell or skill type
                    power = enemySkill.isSpell ? enemySkill.magicPower : enemySkill.skillPower;
                } else {
                    power = enemySkill.isSpell ? enemySkill.newMP : enemySkill.newSP;
                }

                if( enemySkill.summon ){
                    savedData.enemySkill = enemySkill;
                   // enemySkill.ShowVoidPanel( enemySkill.voidZoneTypes, enemySkill.monsterPanel );
                    skeletonAnimation.state.Event += OnEventSFX;
                   // enemySkill.SummonCreatures( enemySkill.summonedObjects );
                }

                casting = false;
                enemySkill.castTimeReady = false;
                DealHealDmg( enemySkill, targets, power );
                SetAnimations( enemySkill );
                if( enemySkill.ExtraEffect.ToString() != "None" ){ //Run Extra Effects if there are any
                    //enemySkill.RunExtraEffect(data, this.gameObject); 
                    SkeletonAnimation targetAnimData = gameObject.transform.Find("Animations").GetComponent<SkeletonAnimation>();
                    var animationDuration = targetAnimData.state.SetAnimation(0, enemySkill.animationType, enemySkill.loopAnimation).Animation.duration;
                    QueueTask = new Task( QueueSkill( animationDuration, data, gameObject, enemySkill )  );
                } else {
                    //StartCoroutine(cooldown( enemySkill.skillCooldown, enemySkill ));
                    SkillActiveSet( enemySkill, false );
                }
                CoolDownTask = new Task( cooldown( enemySkill.skillCooldown, enemySkill ) );
    }

    IEnumerator QueueSkill(float waitTime, Data data, GameObject player, enemySkill enemySkill )
    {
        yield return new WaitForSeconds(waitTime);
        enemyAnimationControl.inAnimation = false;
        //Run extra skill if any
        enemySkill.RunExtraEffect( data, player );
    }

	IEnumerator busyAnimation(float waitTime, GameObject player, enemySkill skillEffects )
	{
		yield return new WaitForSeconds(waitTime);
		enemyAnimationControl.inAnimation = false;
		/*Run extra skill if any
		if ( skillEffects.ExtraSkillToRun != null ){
			print( "running " + skillEffects.ExtraSkillToRun.skillName );
			skillEffects.RunExtraEffect( data, player );
		} */
	//	print ("Animation Que clear");
	}

	IEnumerator cooldown(float waitTime, enemySkill enemySkill)
	{
		//var skillEffects = player.transform.parent.GetComponent<enemySkill_effects>().enemySkilllist[skillNumber];
		yield return new WaitForSeconds(waitTime);
		enemySkill.skillActive = false;
		print ("Spell cooldown " + waitTime);
	}

	IEnumerator castTime(float waitTime, enemySkill enemySkill, Data data )
	{
		//var skillEffects = player.transform.parent.GetComponent<enemySkill_effects>().enemySkilllist[skillNumber];
		yield return new WaitForSeconds(waitTime);
		enemySkill.castTimeReady = true;
        SkillComplete( enemySkill, data.target, data );
		//PrepSkillNew( enemySkill );
		print ("castTime done");
	}

}
