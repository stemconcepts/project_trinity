using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class skillBuilder : MonoBehaviour
	{
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

		public void AttachClassSkill(SkillModel equipedSkill, GameObject charData)
		{
			charData.GetComponent<EquipmentManager>().classSkill = equipedSkill;
			charData.GetComponent<EquipmentManager>().PopulateSkills();
		}

		public void AttachWeapon(WeaponModel equipedWeapon, WeaponModel secondEquipedWeapon, Bauble equipedBauble, GameObject charData)
		{
			if (equipedWeapon)
			{
                charData.GetComponent<EquipmentManager>().primaryWeapon = equipedWeapon;
            }
			if (secondEquipedWeapon)
			{
				charData.GetComponent<EquipmentManager>().secondaryWeapon = secondEquipedWeapon;
			}
			if (equipedBauble)
			{
				charData.GetComponent<EquipmentManager>().bauble = equipedBauble;
			}
		}

		// Use this for initialization
		void Start()
		{
			//var tankSkillsList = tankData.GetComponent<skill_effects>();
			if (SavedDataManager.SavedDataManagerInstance != null)
			{
				/*var tankFirstWeapon = SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.weapon;
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
				var dpsSkill = equipmentManager.Instance.dpsClassSkill;*/

				//equips tank skills
				AttachWeapon(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.weapon,
					SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.secondWeapon,
					SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.bauble, tankData);
				AttachClassSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.classSkill, tankData);
				//equips healer skills
				AttachWeapon(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.weapon,
					SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.secondWeapon,
					SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.bauble, healerData);
				AttachClassSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.classSkill, healerData);
				//equips dps skills
				AttachWeapon(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.weapon,
					SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.secondWeapon,
					SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.bauble, healerData);
				AttachClassSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.classSkill, dpsData);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
