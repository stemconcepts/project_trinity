using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class skillSlotManager : MonoBehaviour
    {
        public List<GameObject> slots = new List<GameObject>();
        public List<GameObject> skills = new List<GameObject>();
        public List<SkillModel> ownedSkills = new List<SkillModel>();
        public GameObject slotPrefab;
        public GameObject skillPrefab;
        public int slotAmount;
        public GameObject tankEquipSlot;
        public GameObject dpsEquipSlot;
        public GameObject healerEquipSlot;

        Transform AddItemToSlot()
        {
            foreach (var slot in slots)
            {
                if (slot.transform.childCount == 0)
                {
                    return slot.transform;
                }
            }
            return null;
        }

        void AddSlot(int i)
        {
            var newSlot = (GameObject)Instantiate(slotPrefab);
            newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
            slots.Add(newSlot);
            slots[i].transform.SetParent(this.transform);
        }

        /// <summary>
        /// Set state of equipped skills in inventory
        /// </summary>
        void SetEquippedSkills()
        {
            foreach (var item in ownedSkills)
            {
                var newitem = (GameObject)Instantiate(skillPrefab, AddItemToSlot());
                var newItemBehaviour = newitem.GetComponent<skillItemBehaviour>();
                newItemBehaviour.classSkill = item;
                switch (item.Class)
                {
                    case SkillModel.ClassEnum.Guardian:
                        newItemBehaviour.type = classType.guardian;
                        if (item.equipped)
                        {
                            newItemBehaviour.EquipSkill(tankEquipSlot, newItemBehaviour.type);
                        }
                        break;
                    case SkillModel.ClassEnum.Stalker:
                        newItemBehaviour.type = classType.stalker;
                        if (item.equipped)
                        {
                            newItemBehaviour.EquipSkill(dpsEquipSlot, newItemBehaviour.type);
                        }
                        break;
                    case SkillModel.ClassEnum.Walker:
                        newItemBehaviour.type = classType.walker;
                        if (item.equipped)
                        {
                            newItemBehaviour.EquipSkill(healerEquipSlot, newItemBehaviour.type);
                        }
                        break;
                }
                skills.Add(newitem);
            }
        }

        // Use this for initialization
        void Awake()
        {
            var allSkills = MainGameManager.instance.SkillFinder.GetAllSkills(true);
            for (int i = 0; i < allSkills.Count; i++)
            {
                if (allSkills[i].learned)
                {
                    ownedSkills.Add(allSkills[i]);
                }
            }

            for (int i = 0; i < slotAmount; i++)
            {
                AddSlot(i);
            }

            SetEquippedSkills();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}