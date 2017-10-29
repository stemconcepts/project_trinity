using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class skillItemBehaviour : MonoBehaviour {
	hoverManager hoverControlScript;
	equipmentManager equipmentManagerScript;
	skillItems skillItemScript;
	public int skillID;
	public string skillName;
	BoxCollider2D colliderScript;
	public enum classType {
		guardian,
		stalker,
		walker
	};
	public classType type;
	GameObject currentSlot;
	itemDetailsControl detailsControlScript;
	float distance;
	public bool dragging = false;
	public bool equipped = false;
	Task holdTimeTask;

	//Move item
	void OnMouseDown(){
		//sets active slotarea
		var allSlots = GameObject.FindGameObjectsWithTag("item-slot");
		foreach( GameObject slotItem in allSlots ){
			slotItem.GetComponent<slotBehaviour>().currentSlot = false;
			slotItem.GetComponent<Image>().color = slotItem.GetComponent<slotBehaviour>().inactiveColor;
			slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = true;
		}
		if( this.transform.parent.GetComponent<slotBehaviour>().currentSlot ){
			this.transform.parent.GetComponent<slotBehaviour>().imageScript.color = this.transform.parent.GetComponent<slotBehaviour>().inactiveColor;
			this.transform.parent.GetComponent<slotBehaviour>().currentSlot = false;
		} else {
			this.transform.parent.GetComponent<slotBehaviour>().imageScript.color = Color.blue;
			this.transform.parent.GetComponent<slotBehaviour>().currentSlot = true; 
		}
		//if( this.transform.FindChild("Panel-item(Clone)") ){
		//this.transform.parent.GetComponent<slotBehaviour>().currentItemID = this.itemID;
		detailsControlScript.DisplaySkillData(skillID);
		//}
		//preps drag
		holdTimeTask = new Task( holdtime( 0.1f ) );
		colliderScript.enabled = false;
		currentSlot = this.transform.parent.gameObject;
		hoverControlScript.lastDraggedItem = this.gameObject;
	}

	IEnumerator holdtime( float waitTime ){
		while( dragging == false ){
			yield return new WaitForSeconds( waitTime );
			hoverControlScript.draggedItem = this.gameObject;
			dragging = true;
		} 
	}

	void OnMouseUp()
	{
		dragging = false;
		hoverControlScript.draggedItem = null;
		holdTimeTask.Stop();
		this.transform.SetParent ( hoverControlScript.hoveredSlot.transform);
		colliderScript.enabled = true;
		var allSlots = GameObject.FindGameObjectsWithTag("item-slot");
		foreach( GameObject slotItem in allSlots ){
			slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = false;
		}
		if( hoverControlScript.hoveredEquipSlot == null ){
		} else {
			var allSkills = GameObject.FindGameObjectsWithTag("item-skill");
			foreach( GameObject skillItem in allSkills ){
				skillItem.GetComponent<skillItemBehaviour>().equipped = false;
			}
			this.equipped = true;
			var hoveredClassType = hoverControlScript.lastDraggedItem.GetComponent<skillItemBehaviour>().type;
			if( hoverControlScript.hoveredEquipSlot.name == "Panel-tank skill" ){
				if( hoveredClassType == classType.guardian ){
					equipmentManagerScript.tankClassSkill = skillName;
					equipmentManagerScript.tankSkills.Add(skillItemScript.itemList[skillID].skillName);
					hoverControlScript.hoveredEquipSlot.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
				} else {
					print ("cannot equip this skill");
				}
			} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-dps skill" ) {
				if( hoveredClassType == classType.stalker ){
					equipmentManagerScript.dpsClassSkill = skillName;
					equipmentManagerScript.dpsSkills.Add(skillItemScript.itemList[skillID].skillName);
					hoverControlScript.hoveredEquipSlot.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
				} else {
					print ("cannot equip this skill");
				}
			} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-healer skill" ) {
				if( hoveredClassType == classType.walker ){
					equipmentManagerScript.healerClassSkill = skillName;
					equipmentManagerScript.healerSkills.Add(skillItemScript.itemList[skillID].skillName);
					hoverControlScript.hoveredEquipSlot.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
				} else {
					print ("cannot equip this skill");
				}
			}
		}
	}

	// Use this for initialization
	void Awake () {
		colliderScript = GetComponent<BoxCollider2D>();
		detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
		hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
		equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<equipmentManager>();
		skillItemScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<skillItems>();
	}
	
	// Update is called once per frame
	void Update () {
		if (dragging)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector2 rayPoint = ray.GetPoint(distance);
			transform.position = rayPoint;
		}
	}
}
