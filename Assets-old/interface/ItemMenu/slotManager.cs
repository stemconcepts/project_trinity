using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OwnedItems {
    public List<weaponModel> weapons = new List<weaponModel>();
    public List<bauble> baubles = new List<bauble>();
}

public class slotManager : MonoBehaviour {
	public List<GameObject> slots = new List<GameObject>();
	public List<GameObject> items = new List<GameObject>();
    //public Sprite tempItemIcon;
    public OwnedItems ownedItems = new OwnedItems();
	public GameObject slotPrefab;
	public GameObject itemPrefab;
	public GameObject menuManager;
	weaponItems weaponDataScript;
    baubleItems baubleDataScript;

	public int slotAmount;

    void AddSlot( int i ){
        var newSlot = (GameObject)Instantiate( slotPrefab );
            newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
            slots.Add( newSlot );
            slots[i].transform.SetParent ( this.transform );
    }

    Transform AddItemToSlot( int slotID ){
        foreach (var slot in slots)
        {
            if( slot.transform.childCount == 0 ){
                return slot.transform;
            }
        }
        return null;
    }

	// Use this for initialization
	void Awake(){
		weaponDataScript = menuManager.GetComponent<weaponItems>();
        weaponDataScript.GetAllWeapons();
        baubleDataScript = menuManager.GetComponent<baubleItems>();
	    baubleDataScript.GetAllBaubles();
   // }

	//void Start () {
		slotAmount = 54;

		for( int i = 0; i < weaponDataScript.weapons.Count; i++ ){
				ownedItems.weapons.Add( weaponDataScript.weapons[i] );
		}

        for( int i = 0; i < baubleDataScript.baubles.Count; i++ ){
                ownedItems.baubles.Add( baubleDataScript.baubles[i] );
        }

		for( int i = 0; i < slotAmount; i++ ){
            AddSlot(i);
		}

        foreach (var item in ownedItems.weapons)
        {
            var newitem = (GameObject)Instantiate( itemPrefab );
                var newItemBehaviour = newitem.GetComponent<itemBehaviour>();
                newItemBehaviour.weaponItemScript = item;
                newItemBehaviour.type = (itemBehaviour.weaponType)item.type;
                newItemBehaviour.itemIcon = item.itemIcon;
                items.Add( newitem );
                var slotTran = AddItemToSlot(item.GetInstanceID());
                if( slotTran != null ){
                    newitem.transform.SetParent( slotTran );
                }
        }

        foreach (var item in ownedItems.baubles)
        {
            var newitem = (GameObject)Instantiate( itemPrefab );
                var newItemBehaviour = newitem.GetComponent<itemBehaviour>();
                newItemBehaviour.baubleItemScript = item;
                newItemBehaviour.type = itemBehaviour.weaponType.bauble;
                newItemBehaviour.itemIcon = item.itemIcon;
                items.Add( newitem );
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
