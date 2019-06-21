using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class skillLabeldisplay : MonoBehaviour {

	public GameObject buttonObject;
	private skill_selection skillSelection;
	public GameObject globalObject;
	private character_select selectedRole;
	public GameObject iconHolder;
	private Image iconImageScript;
	private sortbuttondata buttonData;
	//order ID set manually - used to know button and skill order from buttondatamanager
	public int orderID;
	private Text displayText;
	public string skillName;
	public Sprite skillIcon;

	public void GetSkillData(){
		if( selectedRole.healerSelected == true ){
			var skillButtonID = buttonData.healerSkillIDs[orderID];
			skillName = buttonData.healerSkillNames[skillButtonID];
			skillIcon = buttonData.healerSkillIcons[skillButtonID];
			var healPlayer = GameObject.FindGameObjectWithTag("Healer");
			var playerData = healPlayer.transform.parent.GetComponent<skill_effects>();
			//sets button ID to skill selection code
			//skillSelection.buttonID = orderID;
			skillSelection.buttonID = buttonData.healerSkillIDs[orderID];
			//Sets ACTUAL skill ID from skillList
			skillSelection.skillID = buttonData.healerSkillIDs[orderID];
			List<classSkills> playerSkillList;
			if( playerData.weaponSlot == skill_effects.weaponSlotEnum.Main ){
				playerSkillList = playerData.skilllistMain;
			} else {
				playerSkillList = playerData.skilllistAlt;
			}
			if( playerSkillList[orderID].skillActive == true ){
				skillSelection.CooldownDisplayCheck( playerSkillList[skillButtonID].skillCooldown, playerSkillList, orderID );
			} else {
				skillSelection.ClearCD();
			}
		} else if ( selectedRole.tankSelected == true ) {
			var skillButtonID = buttonData.tankSkillIDs[orderID];
			skillName = buttonData.tankSkillNames[skillButtonID];
			skillIcon = buttonData.tankSkillIcons[skillButtonID];
			var tankPlayer = GameObject.FindGameObjectWithTag("Tank");
			var playerData = tankPlayer.transform.parent.GetComponent<skill_effects>();
			skillSelection.buttonID = orderID;
			skillSelection.skillID = buttonData.tankSkillIDs[orderID];
			List<classSkills> playerSkillList;
			if( playerData.weaponSlot == skill_effects.weaponSlotEnum.Main ){
				playerSkillList = playerData.skilllistMain;
			} else {
				playerSkillList = playerData.skilllistAlt;
			}

			if( playerSkillList[orderID].skillActive == true ){
				//print ( playerSkillList[orderID].skillName + " set CD" + playerData.skilllist[skillButtonID].skillName );
				skillSelection.CooldownDisplayCheck( playerSkillList[skillButtonID].skillCooldown,playerSkillList, orderID );
			} else {
				skillSelection.ClearCD();
			}
		} else if ( selectedRole.dpsSelected == true ) {
			var skillButtonID = buttonData.dpsSkillIDs[orderID];
			skillName = buttonData.dpsSkillNames[skillButtonID];
			skillIcon = buttonData.dpsSkillIcons[skillButtonID];
			var dpsPlayer = GameObject.FindGameObjectWithTag("Dps");
			var playerData = dpsPlayer.transform.parent.GetComponent<skill_effects>();
			skillSelection.buttonID = orderID;
			skillSelection.skillID = buttonData.dpsSkillIDs[orderID];
			List<classSkills> playerSkillList;
			if( playerData.weaponSlot == skill_effects.weaponSlotEnum.Main ){
				playerSkillList = playerData.skilllistMain;
			} else {
				playerSkillList = playerData.skilllistAlt;
			}

			if( playerSkillList[orderID].skillActive == true ){
				//print ( playerSkillList[orderID].skillName + " set CD" + orderID );
				skillSelection.CooldownDisplayCheck( playerSkillList[skillButtonID].skillCooldown, playerSkillList, orderID );
			} else {
				skillSelection.ClearCD();
			}
		}
		displayText.text = skillName;
		iconImageScript.sprite = skillIcon;
		iconImageScript.type = Image.Type.Filled;
	}



	// Use this for initialization
	void Awake() {
		selectedRole = globalObject.GetComponent<character_select>();
		buttonData = globalObject.GetComponent<sortbuttondata>();
		skillSelection = buttonObject.GetComponent<skill_selection>();
		iconImageScript = iconHolder.GetComponent<Image>();
		displayText = GetComponent<Text>();
	}

	void Start() {
		//GetSkillData();
	}
	
	// Update is called once per frame
	void Update () {
		//getData();
	}
}
