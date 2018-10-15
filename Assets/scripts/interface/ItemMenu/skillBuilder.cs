using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class skillBuilder : MonoBehaviour {
	public GameObject tankData;
	public GameObject healerData;
	public GameObject dpsData;
    public bool skillsReady;

	/*public void BuildSkillList( List<string> equippedSkills, List<skill_properties> skillList  ){
		foreach( string skillName in equippedSkills ){
			for ( int i = 0; i < skillList.Count; i++ ){
				if( skillName == skillList[i].skillName ){
					skillList[i].equipped = true;
					break;
				}
			}
		}
	}*/

    public void AttachClassSkill( classSkills equipedSkill, GameObject charData ){
        charData.GetComponent<equipedWeapons>().classSkill = equipedSkill;
        charData.GetComponent<equipedWeapons>().PopulateSkills();
    }

    public void AttachWeapon( weapons equipedWeapon, weapons secondEquipedWeapon, bauble equipedBauble, GameObject charData  ){
		charData.GetComponent<equipedWeapons>().primaryWeapon = equipedWeapon;
		charData.GetComponent<equipedWeapons>().secondaryWeapon = secondEquipedWeapon;
        charData.GetComponent<equipedBauble>().bauble = equipedBauble;
	}

	// Use this for initialization
	void Awake () {
		//var tankSkillsList = tankData.GetComponent<skill_effects>();
		if( equipmentManager.Instance != null ) {
			var tankFirstWeapon = equipmentManager.Instance.tankWeaponObject;
			var tankSecondWeapon = equipmentManager.Instance.tankSecondWeaponObject;
            var tankBauble = equipmentManager.Instance.tankBaubleObject;
            var tankSkill = equipmentManager.Instance.tankClassSkill;
			var healerFirstWeapon = equipmentManager.Instance.healerWeaponObject;
			var healerSecondWeapon = equipmentManager.Instance.healerSecondWeaponObject;
            var healerBauble = equipmentManager.Instance.healerBaubleObject;
            var healerSkill = equipmentManager.Instance.healerClassSkill;
			var dpsFirstWeapon = equipmentManager.Instance.dpsWeaponObject;
			var dpsSecondWeapon = equipmentManager.Instance.dpsSecondWeaponObject;
            var dpsBauble = equipmentManager.Instance.dpsBaubleObject;
            var dpsSkill = equipmentManager.Instance.dpsClassSkill;
			
			//equips tank skills
			AttachWeapon( tankFirstWeapon, tankSecondWeapon, tankBauble, tankData );
            AttachClassSkill( tankSkill, tankData );
			//equips healer skills
			AttachWeapon( healerFirstWeapon, healerSecondWeapon, healerBauble, healerData );
            AttachClassSkill( healerSkill, healerData );
			//BuildSkillList( healerSkills, healerSkillsList.skilllist );
			//equips dps skills
			AttachWeapon( dpsFirstWeapon, dpsSecondWeapon, dpsBauble, dpsData );
			AttachClassSkill( dpsSkill, dpsData );
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
