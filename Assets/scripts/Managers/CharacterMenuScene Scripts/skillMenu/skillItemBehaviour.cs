using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Spine;

namespace AssemblyCSharp
{
    public enum classType
    {
        guardian,
        stalker,
        walker
    };

    public class skillItemBehaviour : MonoBehaviour
    {
        hoverManager hoverControlScript;
        InventoryManager equipmentManagerScript;
        public int skillID;
        public SkillModel classSkill;
        public Color equipColor;
        public GameObject liveItemHoverObj;
        public GameObject itemHoverObj;
        public GameObject currentSlot;
        public classType type;
        float distance;
        itemDetailsControl detailsControlScript;
        public bool dragging = false;
        public bool hovered = false;
        public bool equipped = false;

        void OnMouseDown()
        {
            var allSlots = equipmentManagerScript.skillSlotManager.slots; //GameObject.FindGameObjectsWithTag("item-slot");
            foreach (GameObject slotItem in allSlots)
            {
                var slotsSkill = slotItem.GetComponentInChildren<skillItemBehaviour>();
                if (slotsSkill != null && this.classSkill.Class == slotsSkill.classSkill.Class)
                {
                    slotItem.GetComponent<slotBehaviour>().currentSlot = false;
                    slotItem.GetComponent<Image>().color = slotItem.GetComponent<slotBehaviour>().inactiveColor;
                }
            }
            if (this.transform.parent.GetComponent<slotBehaviour>().currentSlot)
            {
                this.transform.parent.GetComponent<slotBehaviour>().imageScript.color = this.transform.parent.GetComponent<slotBehaviour>().inactiveColor;
                this.transform.parent.GetComponent<slotBehaviour>().currentSlot = false;
            }
            else
            {
                this.transform.parent.GetComponent<slotBehaviour>().imageScript.color = new Color(0, 0, 0, 0.9f);
                this.transform.parent.GetComponent<slotBehaviour>().currentSlot = true;
            }
            if (!equipped)
            {
                detailsControlScript.DisplaySkillData(classSkill);
            }
            currentSlot = this.transform.parent.gameObject;
            hoverControlScript.lastDraggedItem = this.gameObject;
            hoverControlScript.OriginalSlot = this.transform.parent.gameObject;
        }

        public void EquipSkill(GameObject equipSlot, classType classType)
        {
            if (equipSlot.GetComponent<equipControl>().equippedSkill != classSkill)
            {
                var equipControl = equipSlot.GetComponent<equipControl>();
                var equippedSkill = equipSlot.GetComponentInChildren<skillItemBehaviour>();
                if (equippedSkill)
                {
                    Destroy(equippedSkill.gameObject);
                    equipControl.ClearItemQuality();
                }
                equipped = true;
                switch (classType)
                {
                    case classType.guardian:
                        equipmentManagerScript.tankClassSkill = classSkill;
                        break;
                    case classType.stalker:
                        equipmentManagerScript.dpsClassSkill = classSkill;
                        break;
                    case classType.walker:
                        equipmentManagerScript.healerClassSkill = classSkill;
                        break;
                }
                Instantiate(this, equipSlot.transform);
                equipControl.equippedSkill = classSkill;
                this.transform.parent.GetComponent<Image>().color = equipColor;
                equipControl.ShowSkillQuality();
                SavedDataManager.SavedDataManagerInstance.AddSkill(classSkill, classType.ToString());
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(0f, 0f));
                MainGameManager.instance.soundManager.playSound(MainGameManager.instance.soundManager.uiSounds[6]);
            }
        }

        void UnEquipSkill(GameObject equipSlot, classType classType)
        {
            var equippedSkill = equipSlot.GetComponentInChildren<skillItemBehaviour>();
            if (equippedSkill)
            {
                Destroy(equippedSkill.gameObject);
            }
            switch (classType)
            {
                case classType.guardian:
                    equipmentManagerScript.tankClassSkill = null;
                    break;
                case classType.stalker:
                    equipmentManagerScript.dpsClassSkill = null;
                    break;
                case classType.walker:
                    equipmentManagerScript.healerClassSkill = null;
                    break;
            }
            ClearCurrentEquip(equipSlot);
            var equipControl = equipSlot.GetComponent<equipControl>();
            equipControl.equippedSkill = null;
            equipControl.ClearItemQuality();
            this.transform.parent.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(5f, 5f));
            MainGameManager.instance.soundManager.playSound(MainGameManager.instance.soundManager.uiSounds[7]);
        }

        void EquipToClass(classType classType)
        {
            skillItemBehaviour skillItem = hoverControlScript.lastDraggedItem.GetComponent<skillItemBehaviour>();
            GameObject slot = equipmentManagerScript.GetSkillSlot(classType);
            if (IsSkillEquipped(skillItem, slot))
            {
                skillItem.equipped = false;
                UnEquipSkill(slot, classType);
            }
            else
            {
                EquipSkill(slot, classType);
            }
        }

        bool IsSkillEquipped(skillItemBehaviour skillItem, GameObject slot)
        {
            return slot.GetComponent<equipControl>().equippedSkill == skillItem.classSkill;
        }

        void OnMouseUp()
        {
            EquipToClass(hoverControlScript.lastDraggedItem.GetComponent<skillItemBehaviour>().type);
        }
        public void OnMouseEnter()
        {
            Vector3 rayPoint = Camera.current != null ? Camera.current.ScreenToWorldPoint(Input.mousePosition) : new Vector3();
            rayPoint.z = 0f;
            rayPoint.x += 20f;
            rayPoint.y += 10f;
            liveItemHoverObj = (GameObject)Instantiate(itemHoverObj, rayPoint, Quaternion.identity);
            var itemHoverName = liveItemHoverObj.transform.GetChild(0).GetComponent<Text>();
            liveItemHoverObj.transform.SetParent(GameObject.Find("Canvas - Main").transform);
            liveItemHoverObj.transform.localScale = new Vector3(1f, 1f, 1f);
            itemHoverName.text = "<b>" + classSkill.skillName + "</b>";
            hovered = true;
        }
        public void OnMouseExit()
        {
            hovered = false;
            Destroy(liveItemHoverObj);
        }

        void ClearCurrentEquip(GameObject originalEquipSlot)
        {
            if (originalEquipSlot.transform.childCount > 0)
            {
                var classEquipSlot = originalEquipSlot.name;
                if (classEquipSlot == "Panel-tank skill")
                {
                    equipmentManagerScript.tankWeaponObject = null;
                }
                else if (classEquipSlot == "Panel-dps skill")
                {
                    equipmentManagerScript.dpsWeaponObject = null;
                }
                else if (classEquipSlot == "Panel-healer skill")
                {
                    equipmentManagerScript.healerWeaponObject = null;
                }
            }
        }

        // Use this for initialization
        void Awake()
        {
            //skillItemScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<skillItems>();
            //soundContScript = GetComponent<soundController>();
        }

        void Start()
        {
            detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
            hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
            equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<InventoryManager>();
            if (type == classType.guardian)
            {
                equipColor = new Vector4(0.9f, 0.4f, 0.4f, 1f);
            }
            else if (type == classType.stalker)
            {
                equipColor = new Vector4(0.4f, 0.9f, 0.4f, 1f);
            }
            else if (type == classType.walker)
            {
                equipColor = new Vector4(0.8f, 0.8f, 0.4f, 1f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (hovered)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector2 rayPoint = ray.GetPoint(distance);
                rayPoint.y += 9;
                if (liveItemHoverObj)
                {
                    liveItemHoverObj.transform.position = rayPoint;
                }
            }
            /*if (dragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector2 rayPoint = ray.GetPoint(distance);
                rayPoint.y += 9;
                transform.position = rayPoint;
            }*/
        }
    }
}