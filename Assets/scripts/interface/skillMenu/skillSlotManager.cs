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

    Transform AddItemToSlot( int slotID ){
        foreach (var slot in slots)
        {
            if( slot.transform.childCount == 0 ){
                return slot.transform;
            }
        }
        return null;
    }

    void AddSlot( int i ){
        var newSlot = (GameObject)Instantiate( slotPrefab );
            newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
            slots.Add( newSlot );
            slots[i].transform.SetParent ( this.transform );
    }

	// Use this for initialization
	void Awake(){
		skillDataScript = menuManager.GetComponent<skillItems>();
        skillDataScript.GetAllSkills();
	//}

	// Use this for initialization
	//void Start () {
		slotAmount = 60;
		
		for( int i = 0; i < skillDataScript.skills.Count ; i++ ){
			if( skillDataScript.skills[i].learned ){
				ownedSkills.Add( skillDataScript.skills[i] );
			}
		}
	
		for( int i = 0; i < slotAmount; i++ ){
            AddSlot(i);
		}

        foreach (var item in ownedSkills)
        {
            var newitem = (GameObject)Instantiate( skillPrefab );
                var newItemBehaviour = newitem.GetComponent<skillItemBehaviour>();
                newItemBehaviour.classSkill = item;
                if( item.Class == classSkills.ClassEnum.Guardian){
                    newItemBehaviour.type = skillItemBehaviour.classType.guardian;
                } else if( item.Class == classSkills.ClassEnum.Stalker){
                    newItemBehaviour.type = skillItemBehaviour.classType.stalker;
                } else if( item.Class == classSkills.ClassEnum.Walker){
                    newItemBehaviour.type = skillItemBehaviour.classType.walker;
                }
                skills.Add( newitem );
                var slotTran = AddItemToSlot(item.GetInstanceID());
                if( slotTran != null ){
                    newitem.transform.SetParent( slotTran );
                }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
