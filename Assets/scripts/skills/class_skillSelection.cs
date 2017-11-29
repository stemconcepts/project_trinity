using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class class_skillSelection : MonoBehaviour {
	public GameObject skillObject;
	private Image skillObjectScript;
	public GameObject skillLoader;
	private skill_selection skillLoaderScript;
	public GameObject globalObject;
	private character_select selectedRole;
	private int skillCost;
	private Image buttonObject;
	private sortbuttondata sortButtonsScript;

	public void RunClassSkill(){
		if( !IsCharBusy( GameObject.Find(selectedRole.GetClassRole()) ) ){
			//skillLoaderScript.PrepSkill( selectedRole.GetClassRoleCaps(), 0, true );
			skillLoaderScript.PrepSkillNew( sortButtonsScript.GetClassSkill(selectedRole.GetClassRoleCaps()), false );
		}
	}

	//For Class-skill : Run to check if skills are on Cooldown - if so set the fill amount to the remaining time left
	public void CooldownClassDisplayCheck( float cdtime, classSkills skillEffectsVar ){
			//if( buttonID == buttonIDvar ){
				float timeLeft = 1f/cdtime * skillEffectsVar.currentCDAmount;
				//print (timeLeft);
				skillObjectScript.fillAmount = 1f - timeLeft;
			//}
	}

	//Clear CD timer
	public void ClearCD(){
		skillObjectScript.fillAmount = 0f;
	}

	void CanAffordSkill(){
		if( selectedRole.healerSelected == true ){
			skillCost = sortButtonsScript.healerSkillCosts[0];
			var healPlayer = GameObject.FindGameObjectWithTag("Healer");
			var healPlayerData = healPlayer.transform.parent.GetComponent<character_data>();
			if( skillCost > healPlayerData.actionPoints || IsCharBusy( GameObject.Find("Walker") ) ){
				buttonObject.color = new Color(0.9f, 0.2f, 0.2f);
			} else {
				buttonObject.color = new Color(1f, 1f, 1f);
			}
		} else if ( selectedRole.tankSelected == true ){
			skillCost = sortButtonsScript.tankSkillCosts[0];
			var tankPlayer = GameObject.FindGameObjectWithTag("Tank");
			var tankPlayerData = tankPlayer.transform.parent.GetComponent<character_data>();
			if( skillCost > tankPlayerData.actionPoints || IsCharBusy( GameObject.Find("Guardian") ) ){
				buttonObject.color = new Color(0.9f, 0.2f, 0.2f);
			} else {
				buttonObject.color = new Color(1f, 1f, 1f);
			}
		} else if ( selectedRole.dpsSelected == true ){
			skillCost = sortButtonsScript.dpsSkillCosts[0];
			var dpsPlayer = GameObject.FindGameObjectWithTag("Dps");
			var dpsPlayerData = dpsPlayer.transform.parent.GetComponent<character_data>();
			if( skillCost > dpsPlayerData.actionPoints || IsCharBusy( GameObject.Find("Stalker") ) ){
				buttonObject.color = new Color(0.9f, 0.2f, 0.2f);
			} else {
				buttonObject.color = new Color(1f, 1f, 1f);
			}
		}
	}

	//Run skill On Key Press 4
	void KeyPressSkill( string role ){
        skillLoaderScript.PrepSkillNew( sortButtonsScript.GetClassSkill(role), false );
	}

	void Awake(){
		selectedRole = globalObject.GetComponent<character_select>();
		skillObjectScript = skillObject.GetComponent<Image>();
		buttonObject = GetComponent<Image>();
		skillLoaderScript = skillLoader.GetComponent<skill_selection>();
		sortButtonsScript = globalObject.GetComponent<sortbuttondata>();
	}

	//Check if character is ready to perform Skill
	bool IsCharBusy( GameObject character ){
		return character.transform.Find("Animations").GetComponent<animationControl>().inAnimation;
	}

	// Update is called once per frame
	void Update () {
		//CanAffordSkill();
		if( Input.GetKeyDown( KeyCode.Alpha3 ) && !IsCharBusy( GameObject.Find( selectedRole.GetClassRole() ) ) ){
			KeyPressSkill( selectedRole.GetClassRoleCaps() );
		}
	}
}
