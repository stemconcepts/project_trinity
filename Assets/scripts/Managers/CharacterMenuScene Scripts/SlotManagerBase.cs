using AssemblyCSharp;
using Assets.scripts.Managers.CharacterMenuScene_Scripts.swapSkillMenu;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts
{
    public class SlotManagerBase : MonoBehaviour
    {
        public List<GameObject> slots = new List<GameObject>();
        public List<GameObject> items = new List<GameObject>();
        public GameObject slotPrefab;
        //public GameObject skillPrefab;
        public GameObject eyeSkillPrefab;
        public AssetFinder assetFinder;
        public int slotAmount;

        public void AddSlot(int i)
        {
            var newSlot = Instantiate(slotPrefab);
            newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
            slots.Add(newSlot);
            slots[i].transform.SetParent(this.transform);
        }

        public Transform AddItemToSlot()
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

        public void ClearEquippedColor<T>() where T : itemBehaviourBase<T>
        {
            slots.ForEach(slot =>
            {
                if (slot.transform.childCount > 0)
                {
                    var itemBehaviour = slot.GetComponentInChildren<T>().equipped = false;
                }
                slot.GetComponent<slotBehaviour>().currentSlot = false;
                slot.GetComponent<Image>().color = new Color(1, 1, 1, 1); //slot.GetComponent<slotBehaviour>().inactiveColor;
            });
        }
    }
}
