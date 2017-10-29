using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class slotManager : MonoBehaviour {
	public List<GameObject> slots = new List<GameObject>();
	public List<GameObject> items = new List<GameObject>();
	public List<weapons> ownedItems = new List<weapons>();
	//public List<itemData> ownedItems = new List<itemData>();
	public GameObject slotPrefab;
	public GameObject itemPrefab;
	public GameObject menuManager;
	weaponItems weaponDataScript;
	public int slotAmount;

	// Use this for initialization
	void Awake(){
		weaponDataScript = menuManager.GetComponent<weaponItems>();
	}

	void Start () {
		slotAmount = 54;

		//for( int i = 0; i < weaponDataScript.itemList.Count; i++ ){
		//	if( weaponDataScript.itemList[i].owned ){
		//		ownedItems.Add( weaponDataScript.itemList[i] );
		//	}
		//}
		for( int i = 0; i < weaponDataScript.weaponList.Count; i++ ){
			if( weaponDataScript.weaponList[i].owned ){
				ownedItems.Add( weaponDataScript.weaponList[i] );
				if( i < ownedItems.Count ){
					ownedItems[i].itemNumber = i;
				}
			}
		}
		

		for( int i = 0; i < slotAmount; i++ ){
			//add Slots
			var newSlot = (GameObject)Instantiate( slotPrefab );
			newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
			slots.Add( newSlot );
			slots[i].transform.SetParent ( this.transform );
			if( i < ownedItems.Count ){
					var newitem = (GameObject)Instantiate( itemPrefab );
					newitem.GetComponent<itemBehaviour>().itemID = i;
					newitem.GetComponent<itemBehaviour>().itemName = ownedItems[i].WeaponName;
					newitem.GetComponent<itemBehaviour>().type = (itemBehaviour.weaponType)ownedItems[i].type;
					items.Add( newitem );
					//print( ownedItems[i].WeaponName + i );
					newitem.transform.SetParent( slots[i].transform );
				}
		}

		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
