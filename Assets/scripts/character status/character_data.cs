using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Spine;
using Spine.Unity;

public class character_data : MonoBehaviour {

	public Sprite characterIcon;
	public bool isAlive = true;
	public float Health;
	public float maxHealth;
	public float blockPoints;
	public float absorbPoints;
	public float PDef;
	public float originalPDef;
	public float MDef;
	public float originalMDef;
	public float PAtk;
	public float originalPAtk;
	public float MAtk;
	public float originalMAtk;
	public float ATKspd;
	public float originalATKspd;
	public float critchance;
	public float vigor = 1;
	public float thornsDmg;
	public float originalthornsDmg = 0;
	public float originalvigor = 1;
	public float actionPoints = 6;
	public float originalactionPoints;
	public float maxactionPoints = 6;
	public string characterType;
	public string objectName;
	public string role;
	public character_data target;
	public bool damageImmune;
	public bool canAutoAttack;
	public bool isAttacking;
	public float incomingDmg;
	public float incomingMDmg;
	public float incomingHeal;
	public float current_health;
	public float full_health;
	public GameObject targetHealthBar;
	private Slider SliderScript;
	public GameObject targetActionBar;
	private Slider ApSliderScript;
	public GameObject actionPointsDisplay;
	private Text availableActionPoints;
	public bool isMoving;
	public Vector2 origPosition;
	public Vector2 currentPosition;
	public Quaternion currentRotation;
	public GameObject posMarker;
    public GameObject posMarkerMin;
	public Vector2 attackedPos;
	public GameObject currentPanel;
	public int rowNumber;
	public bool inVoidZone;
	public bool inVoidCounter;
	public bool inThreatZone;
	//public GameObject skillHolderPos;
	//other scripts
	private status statusScript;
    animationControl targetAnimControl;
    public singleStatus deathStatus;

	//Set controls health bar
	void set_healthbar_size(){
			current_health = Health;
			SliderScript.maxValue = full_health;
			SliderScript.value = current_health;
	}

	void maintainHealthValue(){ 
			if( current_health > maxHealth ){
				Health = maxHealth;
			} else if( current_health <= 0 && isAlive ){
                //targetAnimControl = GameObject.transform.Find("Animations").GetComponent<animationControl>();
                if ( !targetAnimControl.inAnimation ){
				    isAlive = false;
				    Health = 0;
                    statusScript.RunStatusFunction( deathStatus, 0f, 0f );
			    }
            }
	}

	void ResetAbsorbPoints(){
		if( absorbPoints <= 0 && statusScript.DoesStatusExist( "damageAbsorb" ) ){
			absorbPoints = 0;
			statusScript.ForceStatusOff( statusScript.GetStatus( "damageAbsorb" ) );
		}
		//reset block points
		if( blockPoints <= 0 && statusScript.DoesStatusExist( "block" ) ){
			blockPoints = 0;
			statusScript.ForceStatusOff( statusScript.GetStatus( "block" ) );
		}
	}

	//Regeneration rules for Action points
	void RegenApStart(){
		Task regenrepeat = new Task(RegenActionPoints());
	}
	IEnumerator RegenActionPoints(){
		while( actionPoints < maxactionPoints ){
			actionPoints += vigor;
			yield return new WaitForSeconds(7f);
			//print ("Running");
		} 
		yield return new WaitForSeconds(7f);
		RegenApStart();
	}

	void set_actionbar_size(){
		ApSliderScript.maxValue = maxactionPoints;
		ApSliderScript.value = actionPoints;
	}

	//Return Attribute Value of choice
	public float GetAttributeValue( string attributeName ){
        if( attributeName != "" ){
		    Type charData = Type.GetType("character_data");
		    var attributeValue = (float)charData.GetField(attributeName).GetValue( this );
		    return attributeValue;
	    }
        return 0;
    }

	//Set Attribute Value of choice
	public void SetAttribute( string attributeName, float value ){
        if( attributeName != "" ){
		    Type charData = Type.GetType("character_data");
		    charData.GetField(attributeName).SetValue( this, value );
        }
	}

	// Use this for initialization
	void Start () {
		if( currentPanel != null ){
            currentPanel.GetComponent<movementPanelController>().isOccupied = true;
            currentPanel.GetComponent<movementPanelController>().currentOccupier = gameObject;
            //Set minion position
            currentPanel.GetComponent<movementPanelController>().moved = role.Contains("Minion") ? false : currentPanel.GetComponent<movementPanelController>().moved;
		}
		originalPDef = PDef;
		originalMDef = MDef;
		originalPAtk = PAtk;
        originalMAtk = MAtk;
		originalMDef = MDef;
		originalATKspd = ATKspd;
		originalactionPoints = actionPoints;
		maxHealth = Health;
		full_health = Health;
        if( targetActionBar != null && targetHealthBar != null ){
		    SliderScript = targetHealthBar.GetComponent<Slider>();
		    ApSliderScript = targetActionBar.GetComponent<Slider>();
        } else {
            SetDataTargets();
        }
        statusScript = GetComponent<status>();
		if(actionPointsDisplay != null){
			availableActionPoints = actionPointsDisplay.GetComponent<Text>();
		}
		origPosition = this.transform.position;
		RegenApStart();
		origPosition = this.transform.position;
        targetAnimControl = this.transform.Find("Animations").GetComponent<animationControl>();
	}

    //Set DataTargets for Minions
    void SetDataTargets(){
        targetHealthBar = transform.parent.GetChild(0).GetChild(0).gameObject;
        SliderScript = targetHealthBar.GetComponent<Slider>();
    }    

	// Update is called once per frame
	void Update () {
		if( posMarker != null ){
			attackedPos.x = posMarker.transform.position.x;
			attackedPos.y = posMarker.transform.position.y;
		}
		ResetAbsorbPoints();
		if( targetHealthBar != null ){
            set_healthbar_size();
        }
        if( targetActionBar != null ){
            set_actionbar_size();
        }
		maintainHealthValue();
		currentPosition = this.transform.position;
		currentRotation = this.transform.rotation;
		if(actionPointsDisplay != null){
			availableActionPoints.text = actionPoints.ToString();
		}
	}
	
}
