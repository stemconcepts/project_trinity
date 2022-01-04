using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class equipmentManager : MonoBehaviour
	{
		//public GameObject sceneControllerObject;
		//sceneManager sceneControlScript;
		public Game_Manager gameManager;
		//public static equipmentManager _Instance;

		public weaponModel tankWeaponObject;
		public weaponModel tankSecondWeaponObject;
		public bauble tankBaubleObject;
		public List<string> tankSkills = new List<string>();
		public SkillModel tankClassSkill;

		public weaponModel healerWeaponObject;
		public weaponModel healerSecondWeaponObject;
		public bauble healerBaubleObject;
		public List<string> healerSkills = new List<string>();
		public SkillModel healerClassSkill;

		public weaponModel dpsWeaponObject;
		public weaponModel dpsSecondWeaponObject;
		public bauble dpsBaubleObject;
		public List<string> dpsSkills = new List<string>();
		public SkillModel dpsClassSkill;

		void Awake()
		{
			gameManager = GetComponent<Game_Manager>();
		}

		void Update()
		{
			if (tankWeaponObject && tankSecondWeaponObject && tankClassSkill != null)
			{
				gameManager.SceneManager.tankReady = true;
			}
			if (healerWeaponObject && healerSecondWeaponObject && healerClassSkill != null)
			{
				gameManager.SceneManager.healerReady = true;
			}
			if (dpsWeaponObject && dpsSecondWeaponObject && dpsClassSkill != null)
			{
				gameManager.SceneManager.dpsReady = true;
			}
		}
	}
}