using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class skillSlotManager : MonoBehaviour {
	public List<GameObject> slots = new List<GameObject>();
	public List<GameObject> skills = new List<GameObject>();
	public List<classSkills> ownedSkills = new List<classSkills>();
	public GameObject slotPrefab;
	public GameObject skillPrefab;
	public GameObject menuManager;
	skillItems skillDataScript;
	public int slotAmount;

	// Use this for initialization
	void Awake(){
		skillDataScript = menuManager.GetComponent<skillItems>();
	}

	// Use this for initialization
	void Start () {
		slotAmount = 60;
		
		for( int i = 0; i < skillDataScript.itemList.Count ; i++ ){
			if( skillDataScript.itemList[i].learned ){
				ownedSkills.Add( skillDataScript.itemList[i] );
			}
		}
		
		for( int i = 0; i < slotAmount; i++ ){
			//add Slots
			var newSlot = (GameObject)Instantiate( slotPrefab );
			newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
			slots.Add( newSlot );
			slots[i].transform.SetParent(this.transform);
			if( i < ownedSkills.Count ){
				var newitem = (GameObject)Instantiate( skillPrefab );
				newitem.GetComponent<skillItemBehaviour>().skillID = i;
				newitem.GetComponent<skillItemBehaviour>().skillName = ownedSkills[i].skillName;
				newitem.GetComponent<skillItemBehaviour>().type = (skillItemBehaviour.classType)ownedSkills[i].Class;
				skills.Add( newitem );
				newitem.transform.SetParent(slots[i].transform);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
