using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class enemySkill_effects : MonoBehaviour {

	private status playerStatus;
	private float multiplier;
	public spawnUI spawnUIscript;
	private character_data characterScript;
	public GameObject targetSkillCd;
	private skill_cd skillCdScript;
	private status statusScript;
	private skill_confirm skillConfirmScript;
	private Task cooldownTask;

	//Get skill data
	//private skill_properties skillPropertiesScript;

	//old skill system.. need at least 1 to find the status list
	//public List<skill_properties> skilllist = new List<skill_properties>();

	//Get Primary Weapon Skills
	public List<enemySkill> enemySkilllist = new List<enemySkill>();


//--------------------------------------Calculate Power ----------------------------------------------//

	void ClearSkillData(){
		for(int i = 0; i < enemySkilllist.Count; i++)
		{
			//Reset Skill use - Main weapon
			enemySkilllist[i].skillActive = false;
			enemySkilllist[i].skillConfirm = false;
			enemySkilllist[i].currentCDAmount = 0;

			//Reset calculated Power
			enemySkilllist[i].newSP = 0;
			enemySkilllist[i].newSP = 0;
			enemySkilllist[i].newMP = 0;
			enemySkilllist[i].newMP = 0;

			//Reset castTime
			enemySkilllist[i].castTimeReady = false;
		}
	}

	void CalculateEnemyPower(){
		for(int i = 0; i < enemySkilllist.Count; i++)
		{
			enemySkilllist[i].newSP = enemySkilllist[i].skillPower * characterScript.PAtk;
			enemySkilllist[i].newMP = enemySkilllist[i].magicPower * characterScript.MAtk;
		}
	}

	// Use this for initialization
	void Awake () {
        targetSkillCd = GameObject.Find("Main Camera");
		characterScript = GetComponent<character_data>();
		skillConfirmScript = targetSkillCd.GetComponent<skill_confirm>();
		statusScript = GetComponent<status>();
		skillCdScript = targetSkillCd.GetComponent<skill_cd>();
		spawnUIscript = GetComponent<spawnUI>();

	}

	void Start () {
		ClearSkillData();
		CalculateEnemyPower();	
		/*if(statusScript && gameObject.tag != "Player")
		{
			for(int i = 0; i < skilllist.Count; i++)
			{
				skilllist[i].statusScript = statusScript;
			}
		}*/

	}

	// Update is called once per frame
	void Update () {

	}


}
