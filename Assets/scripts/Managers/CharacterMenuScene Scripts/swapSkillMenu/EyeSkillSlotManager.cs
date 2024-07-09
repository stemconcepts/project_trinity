using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.scripts.Models.skillModels.swapSkills;
//using static UnityEditor.Progress;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts.swapSkillMenu
{
    public class EyeSkillSlotManager : SlotManagerBase
    {
        public List<EyeSkill> ownedSkills = new List<EyeSkill>();
        public GameObject equipSlot;

        void Awake()
        {
            var eyeSkills = MainGameManager.instance.SkillFinder.GetAllEyeSkills(true);
            for (int i = 0; i < eyeSkills.Count; i++)
            {
                if (eyeSkills[i].learned)
                {
                    ownedSkills.Add(eyeSkills[i]);
                }
            }

            for (int i = 0; i < slotAmount; i++)
            {
                AddSlot(i);
            }
            InitLearnedSkills();
        }

        void InitLearnedSkills()
        {
            foreach (var skill in ownedSkills)
            {
                var newitem = Instantiate(eyeSkillPrefab, AddItemToSlot());
                var newItemBehaviour = newitem.GetComponent<EyeSkillItemBehaviour>();
                newItemBehaviour.eyeSkill = skill;
                if (skill.equipped)
                {
                    newItemBehaviour.EquipEyeSkill(equipSlot, skill);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
