using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class class_skillLabelDisplay : MonoBehaviour {

	public GameObject buttonObject;
	private class_skillSelection skillSelection;
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

	public void BuildClassSkill(){
		if ( selectedRole.tankSelected == true ) {
			skillName = buttonData.tankSkillData.classskill.skillName;
			skillIcon = buttonData.tankSkillData.classskill.skillIcon;
			var tankPlayer = GameObject.FindGameObjectWithTag("Tank");
			var playerData = tankPlayer.transform.parent.GetComponent<skill_effects>();
			if( playerData.classskill.skillActive == true ){
			skillSelection.CooldownClassDisplayCheck( playerData.classskill.skillCooldown, playerData.classskill );
			} else {
				skillSelection.ClearCD();
			}
		} else if ( selectedRole.healerSelected == true ) {
			skillName = buttonData.healerSkillData.classskill.skillName;
			skillIcon = buttonData.healerSkillData.classskill.skillIcon;
			var healerPlayer = GameObject.FindGameObjectWithTag("Healer");
			var playerData = healerPlayer.transform.parent.GetComponent<skill_effects>();
			if( playerData.classskill.skillActive == true ){
			skillSelection.CooldownClassDisplayCheck( playerData.classskill.skillCooldown, playerData.classskill );
			} else {
				skillSelection.ClearCD();
			}
		} else if ( selectedRole.dpsSelected == true ) {
			skillName = buttonData.dpsSkillData.classskill.skillName;
			skillIcon = buttonData.dpsSkillData.classskill.skillIcon;
			var dpsPlayer = GameObject.FindGameObjectWithTag("Dps");
			var playerData = dpsPlayer.transform.parent.GetComponent<skill_effects>();
			if( playerData.classskill.skillActive == true ){
			skillSelection.CooldownClassDisplayCheck( playerData.classskill.skillCooldown, playerData.classskill );
			} else {
				skillSelection.ClearCD();
			}
		}	
		displayText.text = skillName;
		iconImageScript.sprite = skillIcon;
		iconImageScript.type = Image.Type.Filled;
	}

	// Use this for initialization
	void Awake () {
		selectedRole = globalObject.GetComponent<character_select>();
		buttonData = globalObject.GetComponent<sortbuttondata>();
		skillSelection = buttonObject.GetComponent<class_skillSelection>();
		iconImageScript = iconHolder.GetComponent<Image>();
		displayText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
