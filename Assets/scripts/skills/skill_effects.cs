using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class skill_effects : MonoBehaviour {

	private status playerStatus;
	private float multiplier;
	public spawnUI spawnUIscript;
	private character_data characterScript;
	public GameObject targetSkillCd;
//	private skill_cd skillCdScript;
	//private status statusScript;
//	private skill_confirm skillConfirmScript;
	private Task cooldownTask;
	public enum weaponSlotEnum{
		Main,
		Alt
	};
	public weaponSlotEnum weaponSlot;

	//Get skill data
	//private skill_properties skillPropertiesScript;

	//old skill system.. need at least 1 to find the status list
	//public List<skill_properties> skilllist = new List<skill_properties>();

		//Get Primary Weapon Skills
	public List<classSkills> skilllistMain = new List<classSkills>();
		//Get Secondary Weapon Skills
	public List<classSkills> skilllistAlt = new List<classSkills>();
		//Get Class Skills
	public classSkills classskill;


//--------------------------------------Calculate Power ----------------------------------------------//

	void ClearSkillData(){
		for(int i = 0; i < 2; i++)
		{
			//Reset Skill use - Main weapon
			skilllistMain[i].skillActive = false;
			//skilllistMain[i].skillConfirm = false;
			skilllistMain[i].currentCDAmount = 0;

			//Reset Skill use - Alt weapon
			skilllistAlt[i].skillActive = false;
			//skilllistAlt[i].skillConfirm = false;
			skilllistAlt[i].currentCDAmount = 0;

			//Reset calculated Power
			skilllistMain[i].newSP = 0;
			skilllistAlt[i].newSP = 0;
			skilllistMain[i].newMP = 0;
			skilllistAlt[i].newMP = 0;
		}

		//Reset Skill use - class skill
		classskill.skillActive = false;
		//classskill.skillConfirm = false;
		classskill.currentCDAmount = 0;
		//Reset calculated Power
		classskill.newSP = 0;
		classskill.newMP = 0;

	}

	public void CalculateSkillPower(){
		for(int i = 0; i < skilllistMain.Count; i++)
		{
			//Calculate New Power
			skilllistMain[i].newSP = skilllistMain[i].skillPower * characterScript.PAtk;
			skilllistAlt[i].newSP = skilllistAlt[i].skillPower * characterScript.PAtk;
			classskill.newSP = classskill.skillPower * characterScript.PAtk;
		}
	}
	
	public void CalculateMagicPower(){
		for(int i = 0; i < skilllistMain.Count; i++)
		{
			//Calculate New Power
			skilllistMain[i].newMP = skilllistMain[i].magicPower * characterScript.MAtk;
			skilllistAlt[i].newMP = skilllistAlt[i].magicPower * characterScript.MAtk;
			classskill.newMP = classskill.magicPower * characterScript.MAtk;
		}
	}

	// Use this for initialization
	void Awake () {
		characterScript = GetComponent<character_data>();
	//	skillConfirmScript = targetSkillCd.GetComponent<skill_confirm>();
	//	statusScript = GetComponent<status>();
	//	skillCdScript = targetSkillCd.GetComponent<skill_cd>();
		spawnUIscript = GetComponent<spawnUI>();

	}

	void Start () {
		if( gameObject.tag == "Player" ){
            ClearSkillData();
            StartCoroutine( DelayedSetStartingPanel() );
		} 
		/*if(statusScript && gameObject.tag != "Player")
		{
			for(int i = 0; i < skilllist.Count; i++)
			{
				skilllist[i].statusScript = statusScript;
			}
		}*/

	}

    public IEnumerator DelayedSetStartingPanel( ){
        yield return new WaitForEndOfFrame();
        //send Event to EventManager
        EventManager.BuildEvent( "Passive", eventCallerVar: this.gameObject );
        CalculateMagicPower();
        CalculateSkillPower();
    }

	// Update is called once per frame
	void Update () {

	}


}
