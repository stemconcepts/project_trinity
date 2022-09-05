using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;

namespace AssemblyCSharp
{
	public class equipmentManager : MonoBehaviour
	{
		//public GameObject sceneControllerObject;
		//sceneManager sceneControlScript;
		//public GameManager gameManager;
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

        //public static Camera equipmentCamera;
        //public Camera equipmentCameraTarget;

        void Awake()
		{
            //equipmentCamera = equipmentCameraTarget;
        }

        void Start()
        {
			if (MainGameManager.instance.ShowTutorialText())
			{
				MainGameManager.instance.gameMessanger.DisplayMessage(MainGameManager.instance.GetText("Inventory"), MainGameManager.instance.GlobalCanvas.transform, 0, "Inventory Tutorial");
			}
		}

		void Update()
		{
			if (tankWeaponObject && tankSecondWeaponObject && tankClassSkill != null)
			{
				MainGameManager.instance.SceneManager.tankReady = true;
			}
			if (healerWeaponObject && healerSecondWeaponObject && healerClassSkill != null)
			{
				MainGameManager.instance.SceneManager.healerReady = true;
			}
			if (dpsWeaponObject && dpsSecondWeaponObject && dpsClassSkill != null)
			{
				MainGameManager.instance.SceneManager.dpsReady = true;
			}
		}
	}
}