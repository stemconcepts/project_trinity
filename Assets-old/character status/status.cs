using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class StatusDetail {
    public singleStatus singleStatus { get; set; } 
    public classSkills onHitSkill { get; set; } 
    public float duration { get; set; }
    public float power {get; set;}
    public bool dispellable { get; set; }
    public bool turnOff { get; set; }
    public float triggerChance {get; set;}
}

public class status : MonoBehaviour {
	public GameObject globalObject;
    private character_data characterScript;
    private calculateDmg calculateDMGScript;
    private button_clicks buttonClickScript;
    private spawnUI spawnUIscript;
    private skill_effects skillEffectsScript;
    public Task tickTimer;
    public Task durationTimer;
    [Header("Immunity List:")]
    public List<singleStatus> immunityList = new List<singleStatus>();
    [Header("Status List:")]
    public List<singleStatus> statusListSO = new List<singleStatus>();
    public List<statussinglelabel> currentStatusList;
    public List<subStatus> currentSubStatusList;

    public void OnHit( singleStatus singleStatus, classSkills onHitSkill, float duration = 0, bool dispellable = true, bool turnOff = false ){
        /*if( !turnOff && !DoesStatusExist( singleStatus.statusName ) ){ 
            spawnUIscript.ShowLabel( singleStatus );
            var onHitStatus = GetStatusIfExist( singleStatus.statusName );
            onHitStatus.onHitSkillPlayer = onHitSkill;
            StartCoroutine( DurationTimer( duration, singleStatus, ()=> {
                ForceStatusOff( singleStatus );
            } ) );
        } else 
        if( turnOff ){
            ForceStatusOff( singleStatus );
        }*/
    }	

	public void OnTakeDmg( StatusDetail statusDetail ){
        if( !statusDetail.turnOff && !DoesStatusExist( statusDetail.singleStatus.statusName ) ){ 
            spawnUIscript.ShowLabel( statusDetail.singleStatus );
			var attrValue = characterScript.GetAttributeValue( statusDetail.singleStatus.attributeName );
            var stat = attrValue;
            var effect = ScriptableObject.CreateInstance<EffectOnEvent>();
            effect.power = stat * 0.25f;
            effect.duration = statusDetail.duration;
            effect.trigger = "OnTakingDmg";
            effect.triggerChance = 1; //comes from status
            effect.ready = true;
            effect.effect = EffectOnEvent.effectGrp.Status;
            effect.affectSelf = true; //comes from status
            effect.owner = gameObject;
            effect.coolDown = 0;
            EventManager.EventAction += effect.RunEffect;
			StartCoroutine( DurationTimer( statusDetail.duration, statusDetail.singleStatus, ()=> {
				ForceStatusOff( statusDetail.singleStatus );
			} ) );
		} else 
		if( statusDetail.turnOff ){
			ForceStatusOff( statusDetail.singleStatus );
		}
	}

	//Attribute mutations - Use for Attribute buffs and debuff
    public void AttributeChange( StatusDetail statusDetail ){
        var currentStat = characterScript.GetAttributeValue( statusDetail.singleStatus.attributeName );
        var originalStat = characterScript.GetAttributeValue( "original" + statusDetail.singleStatus.attributeName );
        float buffedStat;
        if( statusDetail.singleStatus.statusName == "thorns" ){
            buffedStat = (statusDetail.power / 20) + originalStat;
        } else {
            buffedStat = (statusDetail.power / 100) * originalStat;
        }
        float newStat;
        if( !statusDetail.turnOff && !DoesStatusExist( statusDetail.singleStatus.statusName ) ){ 
            spawnUIscript.ShowLabel( statusDetail.singleStatus );
            if ( statusDetail.singleStatus.buff ){
                newStat = statusDetail.singleStatus.attributeName != "ATKspd" ? currentStat + buffedStat : currentStat - buffedStat;
            } else {
                newStat = statusDetail.singleStatus.attributeName != "ATKspd" ? currentStat - buffedStat : currentStat + buffedStat;
            } 

            //set New stat to character
            characterScript.SetAttribute( statusDetail.singleStatus.attributeName, newStat);

            //Remove status 
            StartCoroutine( DurationTimer( statusDetail.duration, statusDetail.singleStatus, ()=> {
                    characterScript.SetAttribute( statusDetail.singleStatus.attributeName, originalStat);
                })
            );
            print ( characterScript.objectName + " " + statusDetail.singleStatus.attributeName + " is now " + newStat );         
        } else 
        if( statusDetail.turnOff ){
            ForceStatusOff( statusDetail.singleStatus, ()=> {
                characterScript.SetAttribute( statusDetail.singleStatus.attributeName, originalStat);
            });
        }
    }

	//Tumor set Timer then do damage
	public void Tumor( singleStatus singleStatus, float duration, bool dispellable = true, bool turnOff = false ){
		if( !turnOff && !DoesStatusExist( singleStatus.statusName ) ){ 
			spawnUIscript.ShowLabel( singleStatus );
			//statuscount += 1;
			//buffposition += 1;
			StartCoroutine( DurationTimer( duration, singleStatus, ()=> {
				characterScript.incomingMDmg = GetStatusIfExist( singleStatus.statusName ).buffPower;
				calculateDMGScript.calculateMdamge( singleStatus.statusName );
				//print( "tumor damage" + " " + GetStatusIfExist( singleStatus.statusName ).buffPower );
			} ) );
		} else 
		if( turnOff ){
			ForceStatusOff( singleStatus, ()=> {
				characterScript.incomingMDmg = GetStatusIfExist( singleStatus.statusName ).buffPower;
				calculateDMGScript.calculateMdamge( singleStatus.statusName );
				//print( "tumor damage" + " " + GetStatusIfExist( singleStatus.statusName ).buffPower );
			});
		}
	}

	//set Immunity
	public void SetImmunity( singleStatus singleStatus, float duration, bool dispellable = true, bool turnOff = false ){
		if( !turnOff && !DoesStatusExist( singleStatus.statusName ) ){ 
			immunityList.Add( GetStatus( singleStatus.attributeName ) );
			StartCoroutine( DurationTimer( duration, singleStatus, ()=> {
				ForceStatusOff( singleStatus );
				immunityList.Remove( GetStatus( singleStatus.attributeName ) );
			} ) );
		} else 
		if( turnOff ){
			ForceStatusOff( singleStatus );
			immunityList.Remove( GetStatus( singleStatus.attributeName ) );
			if( singleStatus.hitAnim != "" ){ 
				AddStatusAnimation( false, singleStatus.hitAnim, singleStatus.holdAnim, false );
			}
		}
	}

	//check immunity
	public bool CheckImmunity( string statusName ){
		for (int i = 0; i < immunityList.Count; i++) {
			if( immunityList[i].name == statusName ){
				return true;
			}
		}
		return false;
	}

	//Turn Status On and Change stat permanently
	public void StatusOn( singleStatus singleStatus, float duration, bool dispellable = true, bool turnOff = false ){
		if( !turnOff && !DoesStatusExist( singleStatus.statusName ) && !CheckImmunity( singleStatus.attributeName ) ){ 
			spawnUIscript.ShowLabel( singleStatus );
			//statuscount += 1;
			//buffposition += 1;
			StartCoroutine( DurationTimer( duration, singleStatus, ()=> {
				ForceStatusOff( singleStatus );
			} ) );
			//change aniamtion if there is one
			if( singleStatus.hitAnim != "" ){
				AddStatusAnimation( true, singleStatus.hitAnim, singleStatus.holdAnim, false );
			}
		} else 
		if( turnOff ){
			ForceStatusOff( singleStatus );
			if( singleStatus.hitAnim != "" ){
				AddStatusAnimation( false, singleStatus.hitAnim, singleStatus.holdAnim, false );
			}
		}
	}

	//Add to stat permanently
	public void AddToStatus( singleStatus singleStatus, float duration, float power = 0, bool dispellable = true, bool turnOff = false, string targetStat = "" ){
        var attributeName = targetStat == "" ? singleStatus.attributeName : targetStat;
        if( !turnOff && !DoesStatusExist( singleStatus.statusName ) ){ 
			spawnUIscript.ShowLabel( singleStatus );
			//statuscount += 1;
			//buffposition += 1;
			if( singleStatus.attributeName != null ){
				characterScript.SetAttribute( attributeName, power );
			}
			//singleStatus.active = true;
			//StartCoroutine(stunTimer( stunPower ));
			StartCoroutine( DurationTimer( duration, singleStatus ) );
		} else 
		if( turnOff ){
			ForceStatusOff( singleStatus );
		}
	}

	//Stat changes - use for Ticking Stat changes 
	public void StatChanges( singleStatus singleStatus, float power, float duration, bool regenOn = true, bool dispellable = true, bool turnOff = false ){
		if( !turnOff ){ 
            statussinglelabel status = GetStatusIfExist( singleStatus.statusName );
            if( !DoesStatusExist( singleStatus.statusName ) ){
                spawnUIscript.ShowLabel( singleStatus );
                tickTimer = new Task( ChangePoints( singleStatus, power, singleStatus.attributeName, regenOn ));
            } else {
                if( singleStatus.canStack ){ 
                    status.stacks = status.stacks < singleStatus.maxStacks ? status.stacks + 1 : status.stacks;
                    spawnUIscript.AddStacks( status ); 
                }
                durationTimer.Stop();
                tickTimer.Stop();
                tickTimer = new Task( ChangePoints( singleStatus, power * status.stacks, singleStatus.attributeName, regenOn ));
            }
            durationTimer = new Task( DurationTimer( duration, singleStatus, ()=> {
                tickTimer.Stop ();
            } ) );
		} else {
			ForceStatusOff( singleStatus, ()=> {
				tickTimer.Stop ();
			});
		}
	}
	IEnumerator ChangePoints( singleStatus singleStatus, float power, string stat, bool regenOn ){
		var currentStat = characterScript.GetAttributeValue( stat );
		var maxStat = characterScript.GetAttributeValue( "max" + stat );
		while( currentStat <= maxStat && currentStat > 0 ){
			if( regenOn ){
				characterScript.incomingHeal = power;
				calculateDMGScript.calculateHdamage();
			} else { 
				characterScript.incomingDmg = power;
                calculateDMGScript.calculatedamage( singleStatus.statusName );
			}
			yield return new WaitForSeconds(5f);
		} 
	}

	//turns Status Off
	public void ForceStatusOff( singleStatus singleStatus, System.Action statusAction = null ){
		//singleStatus.active = false;
		if( statusAction != null ){
			//runs script listed at the coroutine request
			statusAction();
		}
		//singleStatus.statusposition = 0;
		spawnUIscript.RemoveLabel( singleStatus.statusName , true );
		if( singleStatus.hitAnim != "" ){
				AddStatusAnimation( false, singleStatus.hitAnim, singleStatus.holdAnim, false );
			}
	}

	IEnumerator DurationTimer( float waitTime, singleStatus singleStatus, System.Action statusAction = null )
	{
		yield return new WaitForSeconds(waitTime);
		if( statusAction != null ){
			statusAction();
		}
        spawnUIscript.RemoveLabel( singleStatus.statusName, singleStatus.buff );
	}

	public void AddStatusAnimation( bool addStatus, string animName, string holdAnim, bool animHold ){
		if( addStatus ){
            //if animation is death, run immediately
            if( animName == "toDeath" ){
			    //set InAnimation to true... to stop "hit" event cancelling the status animation
                SkeletonAnimation targetAnimData = transform.Find("Animations").GetComponent<SkeletonAnimation>();
                targetAnimData.state.SetAnimation( 0, "toDeath", false);
			    //anim Control.inAnimation = true;
            } else {
                calculateDMGScript.hitAnimation = animName;
                if( animHold ){
                    calculateDMGScript.animationHold = animHold;
                } else {
                    calculateDMGScript.holdAnimation = holdAnim;
                }
            }
		} else {
			SkeletonAnimation skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			skeletonAnimation.state.SetAnimation(0, "stunToIdle", true );
			//set InAnimation to true... to stop "hit" event cancelling the status animation
			calculateDMGScript.hitAnimation = "";		
			calculateDMGScript.animationHold = addStatus;
			//set animation back to idle after recovery animation is complete
			var animationDuration = skeletonAnimation.state.SetAnimation(0, "stunToIdle", false).Animation.duration;
			StartCoroutine( busyAnimation( animationDuration ));
		}
	}

	IEnumerator busyAnimation(float waitTime )
	{
		yield return new WaitForSeconds(waitTime);
		animationControl animControl = this.transform.Find("Animations").GetComponent<animationControl>();
		SkeletonAnimation skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
		skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
		animControl.inAnimation = false;
	}

	//--------------------------------------Negative Status Effects ----------------------------------------------//
	//Duration
	
	//check if status exists
	public bool DoesStatusExist( string statusName ){
		Type statusData = Type.GetType("status");
		var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
		var statusHolderObject = GameObject.Find( buttonClickScript.GetClassRole() + "status" );
		Transform statusPanel;
		singleStatus status;
		for( int i = 0; i < statusList.Count; i++ ){			
			if( statusList[i].name == statusName  ){
				status = statusList[i];
				if( status.buff ){
					statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
				} else {
					statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
				}
				int statusCount = statusPanel.childCount;
				for( int x = 0; x < statusCount; x++ ){
					var statusLabelScript = statusPanel.GetChild(x).GetComponent<statussinglelabel>();
					if( statusLabelScript.singleStatus == status ){
						return true;
					} 
				}
			}
		}
		return false;
	}

	//get status and return it
	public singleStatus GetStatus( string statusName ){
		Type statusData = Type.GetType("status");
		var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
		for( int i = 0; i < statusList.Count; i++ ){			
			if( statusList[i].name == statusName  ){
				return statusList[i];
			}	
		}
		return null;
	}

	//check if status exists
	public statussinglelabel GetStatusIfExist( string statusName ){
		Type statusData = Type.GetType("status");
		var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
		var statusHolderObject = GameObject.Find( buttonClickScript.GetClassRole() + "status" );
		Transform statusPanel;
		singleStatus status;
		for( int i = 0; i < statusList.Count; i++ ){			
			if( statusList[i].name == statusName  ){
				status = statusList[i];
				if( status.buff ){
					statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
				} else {
					statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
				}
				int statusCount = statusPanel.childCount;
				for( int x = 0; x < statusCount; x++ ){
					var statusLabelScript = statusPanel.GetChild(x).GetComponent<statussinglelabel>();
					if( statusLabelScript.singleStatus == status ){
						return statusLabelScript;
					} 
				}
			}
		}
		return null;
	}	

	public List<statussinglelabel> GetAllStatusIfExist( bool buff ){
		currentStatusList.Clear();
		//Type statusData = Type.GetType("status");
		//var statusList = (List<singleStatus>)statusData.GetField("statusListSO").GetValue( this );
		var statusHolderObject = GameObject.Find( buttonClickScript.GetClassRole() + "status" );
		Transform statusPanel;
		//singleStatus status;
		//for( int i = 0; i < statusList.Count; i++ ){			
		//	if( statusList[i].name == statusName  ){
				//status = statusList[i];
				if( buff ){
					statusPanel = statusHolderObject.transform.Find( "Panel buffs" );
				} else {
					statusPanel = statusHolderObject.transform.Find( "Panel debuffs" ); 
				}
				int statusCount = statusPanel.childCount;
				for( int x = 0; x < statusCount; x++ ){
					currentStatusList.Add( statusPanel.GetChild(x).GetComponent<statussinglelabel>() );
				}
		//	}
		//}
		return currentStatusList;
	}	

	//--------------------------------------Event Status Effects Start ----------------------------------------------//

//		public void SecondWindOn( float power, bool turnOff, float duration ){
//			if( turnOff == false ){ 
//				//set status item in list to active
//				statuslist[6].active = true;
//				//adds 1 to buffposition
//				buffposition = buffposition +1;
//				//set buffposition to list item
//				statuslist[6].statusposition = buffposition;
//				//show status icon
//				spawnUIscript.ShowLabel("haste", true);
//				characterScript.maxActionPoints = characterScript.maxActionPoints + power;
//				//characterScript.actionPoints = characterScript.actionPoints + power;
//				StartCoroutine( DurationTimer( duration, 6, "haste", ()=> {
//					//characterScript.actionPoints = characterScript.originalActionPoints;
//					characterScript.maxActionPoints = characterScript.originalActionPoints;
//				})
//				               );
//				//characterScript.actionPoints = characterScript.actionPoints + power;
//				print ( characterScript.objectName + " max action points is now " + characterScript.maxActionPoints );	
//			} else 
//			if( turnOff == true ){
//				ForceStatusOff( 6, "haste", ()=> {
//					//characterScript.actionPoints = characterScript.originalActionPoints;
//					characterScript.maxActionPoints = characterScript.originalActionPoints;
//				});
//			}
//		}

	// Use this for initialization
	void Awake () {
        globalObject = GameObject.Find("Main Camera");
		//statusIndicator = GetComponent<SpriteRenderer>();
		characterScript = GetComponent<character_data>();
		spawnUIscript = GetComponent<spawnUI>();
		calculateDMGScript = GetComponent<calculateDmg>();
		buttonClickScript = GetComponent<button_clicks>();
		//for( int i = 0; i < statuslist.Count; i++ ){
		//	statuslist[i].statusmethodscript = statusScript;
		//}
	}

	// Update is called once per frame
	void Update () {
		
	}

	//Status Function Selected - Turns status Off
    public void RunStatusFunction( singleStatus singleStatus, float power = 0, float duration = 0, bool statusOff = false, classSkills onHitSkillPlayer = null, enemySkill onHitSkillEnemy = null, string stat = "" ){
        //makes duration infinite if set to 0
        duration = duration == 0 ? Mathf.Infinity : duration;
        var statusDetail = new StatusDetail(){
                singleStatus = singleStatus,
                power = power,
                duration = duration,
                turnOff = statusOff
        };
        switch ( singleStatus.selectedStatusFunction ) {
		case singleStatus.statusFunction.AttributeChange:
			AttributeChange( statusDetail );
			break;
		case singleStatus.statusFunction.AddToStat:
			AddToStatus( singleStatus, duration, power, statusOff, targetStat: stat );
			break;
		case singleStatus.statusFunction.StatChange:
			StatChanges( singleStatus, power, duration, singleStatus.buff, statusOff );
			break;
		case singleStatus.statusFunction.StatusOn:
			StatusOn( singleStatus, duration, statusOff );
			break;
		case singleStatus.statusFunction.Tumor:
			Tumor( singleStatus, duration, statusOff );
			break;
		case singleStatus.statusFunction.OnHit:
			OnHit( singleStatus, onHitSkillPlayer, duration, statusOff );
			break;
		case singleStatus.statusFunction.OnHitEnemy:
			OnTakeDmg( statusDetail );
			break;
		case singleStatus.statusFunction.Immune:
			SetImmunity( singleStatus, duration, statusOff );
			break;
		}
	}

	/*Status Function Selected - Turns status Off for Lists
	public void RunStatusFunctionList( singleStatus singleStatus, float power = 0, float duration = 0, bool statusOff = false ){
		for( int x = 0; x < singleStatus.statusFunctionList.Count; x++ ){
			var singleStatusEnum = singleStatus.statusFunctionList[x].GetType();
			singleStatusEnum.
			switch ( singleStatus.statusFunctionList[x] ) {
			case singleStatus.statusFunctionList[x].AttributeChange:
				AttributeChange( singleStatus, power, duration, statusOff );
				break;
			case singleStatus.statusFunctionList[x].AddToStat:
				AddToStatus( singleStatus, duration, power, statusOff );
				break;
			case singleStatus.statusFunctionList[x].StatChange:
				StatChanges( singleStatus, power, duration, singleStatus.buff, statusOff );
				break;
			case singleStatus.statusFunctionList[x].StatusOn:
				StatusOn( singleStatus, duration, statusOff );
				break;
			case singleStatus.statusFunctionList[x].Tumor:
				Tumor( singleStatus, duration, statusOff );
				break;
			}
		}
	}*/

}
