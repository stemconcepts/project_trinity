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
		public GameObject tankfirstSlot;
        public WeaponModel tankWeaponObject;
        public GameObject tanksecondSlot;
        public WeaponModel tankSecondWeaponObject;
		public Bauble tankBaubleObject;
		public SkillModel tankClassSkill;

        public GameObject healerfirstSlot;
        public WeaponModel healerWeaponObject;
        public GameObject healersecondSlot;
        public WeaponModel healerSecondWeaponObject;
		public Bauble healerBaubleObject;
		public SkillModel healerClassSkill;

        public GameObject dpsfirstSlot;
        public WeaponModel dpsWeaponObject;
        public GameObject dpssecondSlot;
        public WeaponModel dpsSecondWeaponObject;
		public Bauble dpsBaubleObject;
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
            MainGameManager.instance.SceneManager.tankReady = tankWeaponObject && tankSecondWeaponObject && tankClassSkill;
            MainGameManager.instance.SceneManager.healerReady = healerWeaponObject && healerSecondWeaponObject && healerClassSkill;
            MainGameManager.instance.SceneManager.dpsReady = dpsWeaponObject && dpsSecondWeaponObject && dpsClassSkill;
		}
	}
}