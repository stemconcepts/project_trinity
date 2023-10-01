using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using Assets.scripts.Models.skillModels.swapSkills;
using Assets.scripts.Managers.CharacterMenuScene_Scripts.swapSkillMenu;

namespace AssemblyCSharp
{
	public class InventoryManager : MonoBehaviour
	{
		public CharacterInfoDisplayController characterInfoDisplayController;
        public weaponModel tankWeaponObject;
		public weaponModel tankSecondWeaponObject;
		public bauble tankBaubleObject;
		public SkillModel tankClassSkill;

        public weaponModel healerWeaponObject;
		public weaponModel healerSecondWeaponObject;
		public bauble healerBaubleObject;
		public SkillModel healerClassSkill;

		public weaponModel dpsWeaponObject;
		public weaponModel dpsSecondWeaponObject;
		public bauble dpsBaubleObject;
		public SkillModel dpsClassSkill;

        public EyeSkill eyeSkill;

        [Header("Equip Slots")]
		public GameObject tankSkillSlot;
        public GameObject dpsSkillSlot;
        public GameObject healerSkillSlot;
        public GameObject eyeSkillSlot;

        [Header("Equipment Managers")]
        public slotManager slotManager;
        public skillSlotManager skillSlotManager;
        public EyeSkillSlotManager swapSkillSlotManager;

        void Awake()
		{
			//equipmentCamera = equipmentCameraTarget;
		}

		public GameObject GetSkillSlot(classType classType)
		{
			switch (classType)
			{
				case classType.guardian:
					return tankSkillSlot;
				case classType.stalker:
					return dpsSkillSlot;
				case classType.walker:
					return healerSkillSlot;
				default:
					return null;
			}
		}

		void Start()
        {
            characterInfoDisplayController.LoadHealthInfo();
            if (MainGameManager.instance.ShowTutorialText("Inventory"))
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