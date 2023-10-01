using AssemblyCSharp;
using Assets.scripts.Models.skillModels.swapSkills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Managers.CharacterMenuScene_Scripts.swapSkillMenu
{
    class EyeSkillItemBehaviour : itemBehaviourBase<EyeSkillItemBehaviour>
    {
        public EyeSkill eyeSkill;

        public void EquipEyeSkill(GameObject equipSlot, EyeSkill skill)
        {
            if (equipSlot.GetComponent<equipControl>().equippedSkill != eyeSkill)
            {
                var equipControl = equipSlot.GetComponent<equipControl>();
                var equippedSkill = equipSlot.GetComponentInChildren<EyeSkillItemBehaviour>();
                if (equippedSkill)
                {
                    Destroy(equippedSkill.gameObject);
                    equipControl.ClearItemQuality();
                }
                equipped = true;
                equipmentManagerScript.eyeSkill = eyeSkill;
                Instantiate(this, equipSlot.transform);
                equipControl.equippedEyeSkill = eyeSkill;
                this.transform.parent.GetComponent<Image>().color = equipColor;
                equipControl.ShowSkillQuality();
                SavedDataManager.SavedDataManagerInstance.SaveEyeSkill(eyeSkill);
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(0f, 0f));
                MainGameManager.instance.soundManager.playSound(MainGameManager.instance.soundManager.uiSounds[6]);
            }
        }

        void UnEquipEyeSkill(GameObject equipSlot)
        {
            var equippedSkill = equipSlot.GetComponentInChildren<EyeSkillItemBehaviour>();
            if (equippedSkill)
            {
                Destroy(equippedSkill.gameObject);
            }
            equipmentManagerScript.eyeSkill = null;
            var equipControl = equipSlot.GetComponent<equipControl>();
            equipControl.equippedEyeSkill = null;
            equipControl.ClearItemQuality();
            this.transform.parent.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            SavedDataManager.SavedDataManagerInstance.SaveEyeSkill(null);
            MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(5f, 5f));
            MainGameManager.instance.soundManager.playSound(MainGameManager.instance.soundManager.uiSounds[7]);
            detailsControlScript.ClearItemDescriptions();
        }

        void OnMouseUp()
        {
            equipmentManagerScript.swapSkillSlotManager.ClearEquippedColor<EyeSkillItemBehaviour>();
            if (equipmentManagerScript.eyeSkill == this.eyeSkill)
            {
                this.eyeSkill.equipped = false;
                UnEquipEyeSkill(equipmentManagerScript.eyeSkillSlot);
            }
            else
            {
                EquipEyeSkill(equipmentManagerScript.eyeSkillSlot, this.eyeSkill);
            }
        }

        void OnMouseDown()
        {
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
                detailsControlScript.DisplaySkillData(eyeSkill);
            }
            currentSlot = this.transform.parent.gameObject;
            hoverControlScript.lastDraggedItem = this.gameObject;
            hoverControlScript.OriginalSlot = this.transform.parent.gameObject;
        }

        public void OnMouseEnter()
        {
            Vector3 rayPoint = Camera.current != null ? Camera.current.ScreenToWorldPoint(Input.mousePosition) : new Vector3();
            rayPoint.z = 0f;
            rayPoint.x += 20f;
            rayPoint.y += 10f;
            liveItemHoverObj = Instantiate(itemHoverObj, rayPoint, Quaternion.identity);
            var itemHoverName = liveItemHoverObj.transform.GetChild(0).GetComponent<Text>();
            liveItemHoverObj.transform.SetParent(GameObject.Find("Canvas - Main").transform);
            liveItemHoverObj.transform.localScale = new Vector3(1f, 1f, 1f);
            itemHoverName.text = "<b>" + eyeSkill.skillName + "</b>";
            hovered = true;
        }
        public void OnMouseExit()
        {
            hovered = false;
            Destroy(liveItemHoverObj);
        }
    }
}
