using AssemblyCSharp;
using Assets.scripts.Managers.CharacterMenuScene_Scripts.swapSkillMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts
{
    public abstract class itemBehaviourBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        public hoverManager hoverControlScript;
        public InventoryManager equipmentManagerScript;
        public Color equipColor;
        public GameObject liveItemHoverObj;
        public GameObject itemHoverObj;
        public GameObject currentSlot;
        //public classType type;
        float distance;
        public itemDetailsControl detailsControlScript;
        public bool dragging = false;
        public bool hovered = false;
        public bool equipped = false;

        void UnEquipSkillOnClass(GameObject equipSlot, classType classType)
        {
            var equippedSkill = equipSlot.GetComponentInChildren<T>();
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

        /*public void EquipSkillOnClass(GameObject equipSlot, classType classType)
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

        void EquipToClass(classType classType)
        {
            skillItemBehaviour skillItem = hoverControlScript.lastDraggedItem.GetComponent<skillItemBehaviour>();
            GameObject slot = equipmentManagerScript.GetSkillSlot(classType);
            if (IsSkillEquipped(skillItem, slot))
            {
                skillItem.equipped = false;
                UnEquipSkillOnClass(slot, classType);
            }
            else
            {
                EquipSkillOnClass(slot, classType);
            }
        }*/

        public void ClearCurrentEquip(GameObject originalEquipSlot)
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
                } else
                {
                    equipmentManagerScript.eyeSkill = null;
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
            /*if (type == classType.guardian)
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
            }*/
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
