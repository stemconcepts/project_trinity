using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class skillBuilder : MonoBehaviour {
	public GameObject tankData;
	public GameObject healerData;
	public GameObject dpsData;

	public void BuildSkillList( List<string> equippedSkills, List<skill_properties> skillList  ){
		foreach( string skillName in equippedSkills ){
			for ( int i = 0; i < skillList.Count; i++ ){
				//print ( skillName + " " + skillList[i].skillName );
				if( skillName == skillList[i].skillName ){
					skillList[i].equipped = true;
					break;
				}
			}
		}
	}

	public void AttachWeapon( weapons equipedWeapon, weapons secondEquipedWeapon, GameObject charData  ){
		charData.GetComponent<equipedWeapons>().primaryWeapon = equipedWeapon;
		charData.GetComponent<equipedWeapons>().secondaryWeapon = secondEquipedWeapon;
	}

	// Use this for initialization
	void Awake () {
		//var tankSkillsList = tankData.GetComponent<skill_effects>();
		if( equipmentManager.Instance != null ) {
			var tankFirstWeapon = equipmentManager.Instance.tankWeaponObject;
			var tankSecondWeapon = equipmentManager.Instance.tankSecondWeaponObject;
		//	var healerSkillsList = healerData.GetComponent<skill_effects>();
			var healerFirstWeapon = equipmentManager.Instance.healerWeaponObject;
			var healerSecondWeapon = equipmentManager.Instance.healerSecondWeaponObject;
		//	var dpsSkillsList = dpsData.GetComponent<skill_effects>();
			var dpsFirstWeapon = equipmentManager.Instance.dpsWeaponObject;
			var dpsSecondWeapon = equipmentManager.Instance.dpsSecondWeaponObject;
			
			//equips tank skills
			AttachWeapon( tankFirstWeapon, tankSecondWeapon, tankData );
			//equips healer skills
			AttachWeapon( healerFirstWeapon, healerSecondWeapon, healerData );
			//BuildSkillList( healerSkills, healerSkillsList.skilllist );
			//equips dps skills
			AttachWeapon( dpsFirstWeapon, dpsSecondWeapon, dpsData );
			//BuildSkillList( dpsSkills, dpsSkillsList.skilllist );
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
