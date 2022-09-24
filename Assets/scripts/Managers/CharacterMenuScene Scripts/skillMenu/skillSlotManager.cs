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
        //skillItems skillDataScript;
        public int slotAmount;
        //public GameObject menuManager;
        AssetFinder assetFinder;

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

        // Use this for initialization
        void Awake()
        {
            assetFinder = MainGameManager.instance.assetFinder;
            var allSkills = assetFinder.GetAllSkills();
            slotAmount = 81;

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

            foreach (var item in ownedSkills)
            {
                var newitem = (GameObject)Instantiate(skillPrefab, AddItemToSlot());
                var newItemBehaviour = newitem.GetComponent<skillItemBehaviour>();
                newItemBehaviour.classSkill = item;
                if (item.Class == SkillModel.ClassEnum.Guardian)
                {
                    newItemBehaviour.type = classType.guardian;
                }
                else if (item.Class == SkillModel.ClassEnum.Stalker)
                {
                    newItemBehaviour.type = classType.stalker;
                }
                else if (item.Class == SkillModel.ClassEnum.Walker)
                {
                    newItemBehaviour.type = classType.walker;
                }
                skills.Add(newitem);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}