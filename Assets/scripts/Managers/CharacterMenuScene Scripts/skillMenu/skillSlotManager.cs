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

        Transform AddItemToSlot(int slotID)
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
            //skillDataScript = menuManager.GetComponent<skillItems>();
            var allSkills = assetFinder.GetAllSkills();
            //}

            // Use this for initialization
            //void Start () {
            slotAmount = 60;

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
                var newitem = (GameObject)Instantiate(skillPrefab);
                var newItemBehaviour = newitem.GetComponent<skillItemBehaviour>();
                newItemBehaviour.classSkill = item;
                if (item.Class == SkillModel.ClassEnum.Guardian)
                {
                    newItemBehaviour.type = skillItemBehaviour.classType.guardian;
                }
                else if (item.Class == SkillModel.ClassEnum.Stalker)
                {
                    newItemBehaviour.type = skillItemBehaviour.classType.stalker;
                }
                else if (item.Class == SkillModel.ClassEnum.Walker)
                {
                    newItemBehaviour.type = skillItemBehaviour.classType.walker;
                }
                skills.Add(newitem);
                var slotTran = AddItemToSlot(item.GetInstanceID());
                if (slotTran != null)
                {
                    newitem.transform.SetParent(slotTran);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}