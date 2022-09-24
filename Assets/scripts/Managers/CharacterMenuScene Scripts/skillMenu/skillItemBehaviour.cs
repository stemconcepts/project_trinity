using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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
        equipmentManager equipmentManagerScript;
        //skillItems skillItemScript;
        public int skillID;
        public SkillModel classSkill;
        public Color equipColor;
        BoxCollider2D colliderScript;
        public GameObject liveItemHoverObj;
        public GameObject itemHoverObj;
        public GameObject currentSlot;
        public classType type;
        float distance;
        itemDetailsControl detailsControlScript;
        public bool dragging = false;
        public bool hovered = false;
        public bool equipped = false;
        Task holdTimeTask;
        //private soundController soundContScript;
        public AudioClip audioclip;
        public AudioClip audioclip2;
        public AudioClip audioclipEquip;

        void OnMouseDown()
        {
            var allSlots = equipmentManagerScript.skillSlotManager.slots; //GameObject.FindGameObjectsWithTag("item-slot");
            foreach (GameObject slotItem in allSlots)
            {
                slotItem.GetComponent<slotBehaviour>().currentSlot = false;
                slotItem.GetComponent<Image>().color = slotItem.GetComponent<slotBehaviour>().inactiveColor;
                //slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = true;
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
            //holdTimeTask = new Task(holdtime(0.1f));
            //colliderScript.enabled = false;
            currentSlot = this.transform.parent.gameObject;
            hoverControlScript.lastDraggedItem = this.gameObject;
            hoverControlScript.OriginalSlot = this.transform.parent.gameObject;
        }

        IEnumerator holdtime(float waitTime)
        {
            while (dragging == false)
            {
                yield return new WaitForSeconds(waitTime);
                hoverControlScript.draggedItem = this.gameObject;
                dragging = true;
            }
        }

        void EquipSkill(GameObject equipSlot, classType classType)
        {
            var allSkills = equipmentManagerScript.skillSlotManager.skills;
            if (equipSlot.GetComponent<equipControl>().equippedSkill != classSkill)
            {
                if (equipSlot.transform.childCount > 0)
                {
                    Destroy(equipSlot.transform.GetChild(0).gameObject);
                }
                foreach (GameObject skillItem in allSkills)
                {
                    skillItem.GetComponent<skillItemBehaviour>().equipped = false;
                    MainGameManager.instance.ResetAnchorPoints(skillItem.gameObject, new Vector2(5f, 5f));
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
                equipSlot.GetComponent<equipControl>().equippedSkill = classSkill;
                this.transform.parent.GetComponent<Image>().color = equipColor;
                SavedDataManager.SavedDataManagerInstance.AddSkill(classSkill, classType.ToString());
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(0f, 0f));
            }
        }

        void EquipToClass(classType classType)
        {
            var allSkills = GameObject.FindGameObjectsWithTag("item-skill");
            switch (classType)
            {
                case classType.guardian:
                    EquipSkill(equipmentManagerScript.GetSkillSlot(classType), classType);
                    break;
                case classType.stalker:
                    EquipSkill(equipmentManagerScript.GetSkillSlot(classType), classType);
                    break;
                case classType.walker:
                    EquipSkill(equipmentManagerScript.GetSkillSlot(classType), classType);
                    break;
            }
        }

        void OnMouseUp()
        {
            EquipToClass(hoverControlScript.lastDraggedItem.GetComponent<skillItemBehaviour>().type);
        }

        /*void OnMouseUp()
        {
            hoverControlScript.draggedItem = null;
            if (holdTimeTask != null)
            {
                holdTimeTask.Stop();
            }
            colliderScript.enabled = true;
            var allSlots = GameObject.FindGameObjectsWithTag("item-slot");
            foreach (GameObject slotItem in allSlots)
            {
                slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = false;
            }
            if (!hoverControlScript.hoveredEquipSlot)
            {
                if (hoverControlScript.hoveredSlot != null)
                {
                    ClearCurrentEquip(hoverControlScript.OriginalSlot);
                    this.transform.SetParent(hoverControlScript.hoveredSlot.transform.childCount > 0 ? hoverControlScript.OriginalSlot.transform : hoverControlScript.hoveredSlot.transform);
                    if (tankEquipped || healerEquipped || dpsEquipped)
                    {
                        hoverControlScript.OriginalSlot.GetComponent<Image>().color = hoverControlScript.OriginalSlot.GetComponent<slotBehaviour>().origColor;
                        this.transform.parent.GetComponent<Image>().color = equipColor;
                    }
                }
                //this.transform.SetParent( hoverControlScript.hoveredSlot.transform );
                if (dragging)
                {
                   // soundContScript.playSound(audioclip);
                }
                //equipped = false;
            }
            else
            {
                var allSkills = GameObject.FindGameObjectsWithTag("item-skill");
                //this.equipped = true;
                var hoveredClassType = hoverControlScript.lastDraggedItem.GetComponent<skillItemBehaviour>().type;
                if (hoverControlScript.hoveredEquipSlot.name == "Panel-tank skill")
                {
                    if (hoveredClassType == classType.guardian)
                    {
                        equipmentManagerScript.tankClassSkill = classSkill;
                        foreach (GameObject skillItem in allSkills)
                        {
                            skillItem.GetComponent<skillItemBehaviour>().tankEquipped = false;
                        }
                        tankEquipped = true;
                        equipmentManagerScript.tankSkills.Clear();
                        equipmentManagerScript.tankSkills.Add(classSkill.skillName);
                        hoverControlScript.hoveredEquipSlot.GetComponent<Image>().enabled = true;
                        hoverControlScript.hoveredEquipSlot.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                        hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedSkill = classSkill;
                        this.transform.parent.GetComponent<Image>().color = equipColor;
                        SavedDataManager.SavedDataManagerInstance.AddSkill(classSkill, "guardian");
                    }
                    else
                    {
                        print("cannot equip this skill");
                    }
                }
                else if (hoverControlScript.hoveredEquipSlot.name == "Panel-dps skill")
                {
                    if (hoveredClassType == classType.stalker)
                    {
                        equipmentManagerScript.dpsClassSkill = classSkill;
                        foreach (GameObject skillItem in allSkills)
                        {
                            skillItem.GetComponent<skillItemBehaviour>().dpsEquipped = false;
                        }
                        dpsEquipped = true;
                        equipmentManagerScript.dpsSkills.Clear();
                        equipmentManagerScript.dpsSkills.Add(classSkill.skillName);
                        hoverControlScript.hoveredEquipSlot.GetComponent<Image>().enabled = true;
                        hoverControlScript.hoveredEquipSlot.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                        this.transform.parent.GetComponent<Image>().color = equipColor;
                        SavedDataManager.SavedDataManagerInstance.AddSkill(classSkill, "stalker");
                    }
                    else
                    {
                        print("cannot equip this skill");
                    }
                }
                else if (hoverControlScript.hoveredEquipSlot.name == "Panel-healer skill")
                {
                    if (hoveredClassType == classType.walker)
                    {
                        equipmentManagerScript.healerClassSkill = classSkill;
                        foreach (GameObject skillItem in allSkills)
                        {
                            skillItem.GetComponent<skillItemBehaviour>().healerEquipped = false;
                        }
                        healerEquipped = true;
                        equipmentManagerScript.healerSkills.Clear();
                        equipmentManagerScript.healerSkills.Add(classSkill.skillName);
                        hoverControlScript.hoveredEquipSlot.GetComponent<Image>().enabled = true;
                        hoverControlScript.hoveredEquipSlot.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                        this.transform.parent.GetComponent<Image>().color = equipColor;
                        SavedDataManager.SavedDataManagerInstance.AddSkill(classSkill, "walker");
                        //MainGameManager.instance.SceneManager.healerReady = equipmentManager.tankWeaponObject && equipmentManager.tankSecondWeaponObject && equipmentManager.tankClassSkill;
                    }
                    else
                    {
                        print("cannot equip this skill");
                    }
                }
                this.transform.SetParent(hoverControlScript.OriginalSlot.transform);
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(0f, 0f));
                //particleSystem.Play();
            }
            dragging = false;
        }*/

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
            colliderScript = GetComponent<BoxCollider2D>();
            detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
            hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
            equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<equipmentManager>();
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