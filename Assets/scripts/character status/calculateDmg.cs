using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class calculateDmg : MonoBehaviour {
	[SpineAnimation]
	SkeletonAnimation skeletonAnimation;
	animationControl playerAnimationControl;
	// Use this to reference other data in other scripts, First you must get the game object with the scripts you need
	private gameEffects gameEffectsScript;
	private character_data characterScript;
	private characterMovementController characterMovementScript;
	public combatDisplay combatDisplayScript;
	private enemySkillSelection enemySkillScript;
	private status statusScript;
	public float damageTaken;
	public float MdamageTaken;
	public float healAmountTaken;
	private string skillSource;
    public GameObject hitEffect;
    public GameObject customHitFX;
	private Transform hitEffectPositionScript;
	public GameObject hitEffectPosition;
	private GameObject effectObject;
	public GameObject dmgSource;
	public string hitAnimation;
	public string hitAnimNormal = "hit";
	public bool animationHold;
	public string holdAnimation;
    public List<GameObject> dueDmgTargets;
	private soundController soundContScript;
    private classSkills classSkill;
    private enemySkill enemySkill;
	//public singleStatus damageImmune;
	//public singleStatus damageAbsorb;

    public void TakeDmg(string eventName){
        
            GameObject skillHitEffect = customHitFX != null ? customHitFX : hitEffect; 
            gameEffectsScript.ScreenShake( 1f );
            if( gameObject.tag == "Player" && eventName == "hit" ){
                effectObject = (GameObject)Instantiate( skillHitEffect, new Vector2 ( hitEffectPositionScript.transform.position.x , hitEffectPositionScript.transform.position.y ), new Quaternion ( 0, 180, 0, 0 ) );
            } else if( eventName == "hit" ) {
                effectObject = (GameObject)Instantiate( skillHitEffect, new Vector2 ( hitEffectPositionScript.transform.position.x , hitEffectPositionScript.transform.position.y ), hitEffectPositionScript.transform.rotation );
            }

            if( characterScript.absorbPoints > 0 ){
                var absorbedDmg = characterScript.absorbPoints -= damageTaken;
                characterScript.absorbPoints -= damageTaken;
                if( absorbedDmg < 0 ){
                    absorbedDmg -= (absorbedDmg *= 2);
                    characterScript.Health = characterScript.Health - absorbedDmg;
                    combatDisplayScript.getDmg( absorbedDmg, skillSource );
                }
                var absorbAmount = characterScript.absorbPoints - damageTaken;
                combatDisplayScript.getAbsorb( damageTaken, skillSource );
            }else if( characterScript.blockPoints > 0 ){
                characterScript.blockPoints -= 1f;  
                var damageBlocked = damageTaken * 0.25f;
                damageTaken -= damageBlocked;
                characterScript.Health = characterScript.Health - damageTaken;
                combatDisplayScript.getDmg( damageTaken, skillSource, extraInfo: "<size=100><i>(block:" + damageBlocked + ")</i></size>" );
            }else {
                characterScript.Health = characterScript.Health - damageTaken;
                combatDisplayScript.getDmg( damageTaken, skillSource );
            }

            if ( playerAnimationControl.inAnimation == false && characterScript.isAttacking == false ) {
                if( hitAnimation != "" ){
                    skeletonAnimation.state.SetAnimation(0, hitAnimation, false);
                    skeletonAnimation.state.AddAnimation(0, holdAnimation, true, 0);
                    playerAnimationControl.inAnimation = true;
                } else {
                    var hitAnim = gameObject.tag == "Enemy" ? "hit" : hitAnimNormal;
                    var idleAnim = gameObject.tag == "Enemy" ? "idle" : characterMovementScript.idleAnim;
                    skeletonAnimation.state.SetAnimation(0, hitAnim, false );
                    skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0 );
                }
            }
            //if tumor on player
            if ( statusScript.DoesStatusExist( "tumor" ) && dmgSource != null){
                var tumor = statusScript.GetStatusIfExist( "tumor" );
                tumor.buffPower += damageTaken * 0.70f;
            }
            //if thorns on player
            if ( statusScript.DoesStatusExist( "thorns" ) && dmgSource != null ){
                //var thorns = statusScript.GetStatusIfExist( "thorns" );
                var sourceCalDmg = dmgSource.GetComponent<calculateDmg>();
                var sourceCharData = dmgSource.GetComponent<character_data>();
                //SourceDmgCalc.calculatedamage( "Thorns" );
                sourceCharData.incomingDmg = characterScript.thornsDmg;
                sourceCalDmg.calculatedamage( "Thorns", trueDmg: true, dmgSourceVar: this.gameObject );
            }

            //if On hit
            if( statusScript.DoesStatusExist("onHit") && dmgSource != null ){
                var onHitSkill = statusScript.GetStatusIfExist( "onHit" );
                if( characterScript.characterType == "enemy" ){
                    //enemySkillScript.PrepSkill( characterScript.role, 0, onHitSkill.onHitSkillEnemy );
                } else {
                    //this.GetComponent<enemySkillSelection>().PrepSkill( characterScript.role, 0, onHitSkill.onHitSkillPlayer );
                }
            }
            //dueDmg = false;
            customHitFX = null;
            //send Event to EventManager
            EventManager.BuildEvent( "OnTakingDmg", extTargetVar: dmgSource, eventCallerVar: this.gameObject );
    }

	public void OnEventHit(Spine.TrackEntry state, Spine.Event e ){
        if( e.Data.name == "hit" || e.Data.name == "SFXhit"){
            foreach (var target in dueDmgTargets)
            {
                var calculateDmg = target.GetComponent<calculateDmg>();
                calculateDmg.TakeDmg( e.Data.name );
                EventManager.BuildEvent( "OnDealingDmg", extTargetVar: target, eventCallerVar: this.gameObject, extraInfoVar: calculateDmg.damageTaken );
            }
        }
	}

    public void calculatedamage ( string skillSource = "N/A", enemySkill enemySkill = null, classSkills classSkill = null, SkeletonAnimation skeletonAnimationVar = null, GameObject dmgSourceVar = null, bool trueDmg = false, bool isSpell = false ) {
        if( classSkill != null ){
            skillSource = classSkill != null ? classSkill.skillName : skillSource; 
        } else if( enemySkill != null ){
            skillSource = enemySkill != null ? enemySkill.skillName : skillSource; 
        }

        if( characterScript != null ){
            var defences = isSpell ? characterScript.MDef : characterScript.PDef;
    		damageTaken = ( characterScript.incomingDmg - defences ) < 0 ? 0 : characterScript.incomingDmg - defences ;
    		damageTaken = trueDmg ? characterScript.incomingDmg : damageTaken;
    		dmgSource = dmgSourceVar;
    		/*if ( damageTaken > 0 ) {*/
    			//if( characterScript.absorbPoints > 0 ){
    			//	characterScript.absorbPoints -= damageTaken;
    			//	var absorbAmount = characterScript.absorbPoints - damageTaken;
    			//	combatDisplayScript.getAbsorb( absorbAmount, skillSource );
    			//} else 
    			if( statusScript.DoesStatusExist( "damageImmune" ) ){
    				combatDisplayScript.Immune( skillSource );
    			} else {
    				if( skeletonAnimationVar == null ){
    					characterScript.Health = characterScript.Health - damageTaken;
    
    					//if tumor on player
    					if ( statusScript.DoesStatusExist( "tumor" ) ){
    						var tumor = statusScript.GetStatusIfExist( "tumor" );
    						tumor.buffPower += damageTaken * 0.70f;
    					}
    
    					combatDisplayScript.getDmg( damageTaken, skillSource );
    					//dueDmg = false;
    				} else {
    					//dueDmg = true;
                        //dueDmgTarget = null;
                        if( classSkill != null ){
                            customHitFX = classSkill.hitEffect;
                        } else if( enemySkill != null ){
                            customHitFX = enemySkill.hitEffect;
                        }
    					//skeletonAnimationVar.state.Event += OnEventHit;
    				}
    				//characterScript.Health = characterScript.Health - damageTaken;
    				//combatDisplayScript.getDmg( damageTaken, skillSource );
    			}
    		/*} else if ( damageTaken <= 0 ){
    			damageTaken = 0;
    			characterScript.incomingDmg = 0;
    			if( skeletonAnimationVar == null ){
    				combatDisplayScript.getDmg( damageTaken, skillSource );
    			} else {
    				skeletonAnimationVar.state.Event += OnEventHit;
    			}
    		}*/
        }
	}

	public void calculateFlatDmg ( string skillSource, float FdmgTaken = 0 ) {
		if ( FdmgTaken > 0 ) {
			if( characterScript.absorbPoints > 0 ){
				characterScript.absorbPoints -= FdmgTaken;
			} else {
				characterScript.Health = characterScript.Health - FdmgTaken;
				combatDisplayScript.getDmg( FdmgTaken, skillSource );
			}
			characterScript.incomingMDmg = 0;
		} else if ( FdmgTaken <= 0 ){
			characterScript.incomingMDmg = 0;
		}
	}

    public void calculateMdamge ( string skillSource, float? MdmgTaken = null ) {
        MdamageTaken = characterScript.incomingMDmg - characterScript.MDef;
        if ( MdamageTaken > 0 ) {
            if( characterScript.absorbPoints > 0 ){
                characterScript.absorbPoints -= MdamageTaken;
            } else {
                characterScript.Health = characterScript.Health - MdamageTaken;
                combatDisplayScript.getDmg( MdamageTaken, skillSource );
            }
            characterScript.incomingMDmg = 0;
            MdamageTaken = 0;
        } else if ( MdamageTaken <= 0 ){
            MdamageTaken = 0;
            characterScript.incomingMDmg = 0;
        }
    }

    public void calculateHdamage ( float healAmount = 0) {
		healAmountTaken = healAmount == 0 ? characterScript.incomingHeal : healAmount;
		if ( healAmountTaken > 0 ) {
			characterScript.Health += healAmountTaken;
			combatDisplayScript.getHeal( healAmountTaken );
			characterScript.incomingHeal = 0;
			healAmountTaken = 0;
		} else if ( healAmountTaken <= 0 ){
			healAmountTaken = 0;
			characterScript.incomingHeal = 0;
		}
	}

	// Use this for initialization
	void Awake () {
		// After naming the object, get the components within those game objects.
		//hitEffect = GameObject.Find ("hitUp");
		gameEffectsScript = GameObject.Find("Main Camera").GetComponent<gameEffects>();
		characterMovementScript = GetComponent<characterMovementController>();
		hitEffectPositionScript = hitEffectPosition.GetComponent<Transform>();
		characterScript = GetComponent<character_data>();
		combatDisplayScript = GetComponent<combatDisplay>();
		statusScript = GetComponent<status>();
		soundContScript = GetComponent<soundController>();
		if( characterScript.characterType == "enemy" ){
			enemySkillScript = GetComponent<enemySkillSelection>();
		}
		if ( this.transform.Find("Animations") ){
			skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			playerAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
