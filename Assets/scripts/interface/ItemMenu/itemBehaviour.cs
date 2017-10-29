using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class itemBehaviour : MonoBehaviour {
	hoverManager hoverControlScript;
	equipmentManager equipmentManagerScript;
	weaponItems weaponItemScript;
	public int itemID;
	public string itemName;
	public enum weaponType {
		bladeAndBoard,
		heavyHanded,
		dualBlades,
		clawAndPistol,
		glove,
		cursedGlove
	};
	public weaponType type;
	GameObject currentSlot;
	itemDetailsControl detailsControlScript;
	float distance;
	public bool dragging = false;
	public bool equipped = false;
	Task holdTimeTask;
	BoxCollider2D colliderScript;
	private soundController soundContScript;
	public AudioClip audioclip;
	public AudioClip audioclip2;
	public AudioClip audioclipEquip;
	private ParticleSystem particleSystem;

	//Move item
	void OnMouseDown(){
		soundContScript.playSound( audioclip2 );
		//sets active slotarea
		var allSlots = GameObject.FindGameObjectsWithTag("item-slot");
		foreach( GameObject slotItem in allSlots ){
			slotItem.GetComponent<slotBehaviour>().currentSlot = false;
			slotItem.GetComponent<Image>().color = slotItem.GetComponent<slotBehaviour>().inactiveColor;
			slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = true;
		}
		if( this.transform.parent.GetComponent<slotBehaviour>() ){
			if( this.transform.parent.GetComponent<slotBehaviour>().currentSlot ){
				this.transform.parent.GetComponent<slotBehaviour>().imageScript.color = this.transform.parent.GetComponent<slotBehaviour>().inactiveColor;
				this.transform.parent.GetComponent<slotBehaviour>().currentSlot = false;
			} else {
				this.transform.parent.GetComponent<slotBehaviour>().imageScript.color = Color.blue;
				this.transform.parent.GetComponent<slotBehaviour>().currentSlot = true;
			}
		}  
		this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
		//if( this.transform.FindChild("Panel-item(Clone)") ){
			//this.transform.parent.GetComponent<slotBehaviour>().currentItemID = this.itemID;
		if( !equipped){
			detailsControlScript.DisplayWeaponData(itemID);
		}
		//}
		//preps drag
		holdTimeTask = new Task( holdtime( 0.1f ) );
		colliderScript.enabled = false;
		currentSlot = this.transform.parent.gameObject;
		hoverControlScript.lastDraggedItem = this.gameObject;
		hoverControlScript.OriginalSlot = this.transform.parent.gameObject;
	}
	
	IEnumerator holdtime( float waitTime ){
		while( dragging == false ){
			yield return new WaitForSeconds( waitTime );
			hoverControlScript.draggedItem = this.gameObject;
			dragging = true;
		} 
	}
	
	void ClearCurrentEquip( GameObject originalEquipSlot ){
		if( originalEquipSlot.transform.childCount > 0  ){
			var classEquipSlot = originalEquipSlot.name;
			if(	classEquipSlot == "Panel-tankweapon" ) {
				equipmentManagerScript.tankWeaponObject = null;
			} else if( classEquipSlot == "Panel-secondTankWeapon" ){
				equipmentManagerScript.tankSecondWeaponObject = null;
			} else if( classEquipSlot == "Panel-dpsweapon" ) {
				equipmentManagerScript.dpsWeaponObject = null;
			} else if( classEquipSlot == "Panel-secondDpsWeapon" ){
				equipmentManagerScript.dpsSecondWeaponObject = null;
			} else if( classEquipSlot == "Panel-healerweapon" ) {
				equipmentManagerScript.healerWeaponObject = null;
			} else if( classEquipSlot == "Panel-secondHealerWeapon" ){
				equipmentManagerScript.healerSecondWeaponObject = null;
			}
		}
	}

	void OnMouseUp()
	{
		hoverControlScript.draggedItem = null;
		holdTimeTask.Stop();
		colliderScript.enabled = true;
		var allSlots = GameObject.FindGameObjectsWithTag("item-slot");
		foreach( GameObject slotItem in allSlots ){
			slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = false;
		}
		if( !hoverControlScript.hoveredEquipSlot ){
			//this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
			if( hoverControlScript.hoveredSlot != null ){
				ClearCurrentEquip( hoverControlScript.OriginalSlot );
				this.transform.SetParent( hoverControlScript.hoveredSlot.transform.childCount > 0 ? hoverControlScript.OriginalSlot.transform : hoverControlScript.hoveredSlot.transform );
			}
			//this.transform.SetParent( hoverControlScript.hoveredSlot.transform );
			if( dragging ){
				soundContScript.playSound( audioclip );
			}
			equipped = false;
		} else {
			//this.transform.SetParent( hoverControlScript.hoveredEquipSlot.transform );
			var allweapons = GameObject.FindGameObjectsWithTag("item-weapon");
			foreach( GameObject weaponItem in allweapons ){
				if ( equipped == true ){
					weaponItem.GetComponent<itemBehaviour>().equipped = false;
				}
			}
			equipped = true;
			var hoveredWeaponType = hoverControlScript.lastDraggedItem.GetComponent<itemBehaviour>().type;
			if( hoverControlScript.hoveredEquipSlot.name == "Panel-tankweapon" || hoverControlScript.hoveredEquipSlot.name == "Panel-secondTankWeapon" ){
				if( hoveredWeaponType == weaponType.heavyHanded || hoveredWeaponType == weaponType.bladeAndBoard  ){
					if( hoverControlScript.hoveredEquipSlot.name == "Panel-tankweapon" ){
						equipmentManagerScript.tankWeaponObject = weaponItemScript.weaponList[itemID];
						//equipmentManagerScript.tankWeapon = itemName;
					} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-secondTankWeapon" ) {
						equipmentManagerScript.tankSecondWeaponObject = weaponItemScript.weaponList[itemID];
					}
					hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
					hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
					this.transform.SetParent( hoverControlScript.hoveredEquipSlot.transform );
				} else {
						print ("cannot equip this weapon");
				}
			} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-dpsweapon" || hoverControlScript.hoveredEquipSlot.name == "Panel-secondDpsweapon" ) {
				if( hoveredWeaponType == weaponType.clawAndPistol || hoveredWeaponType == weaponType.dualBlades  ){
					if( hoverControlScript.hoveredEquipSlot.name == "Panel-dpsweapon" ){
						equipmentManagerScript.dpsWeaponObject = weaponItemScript.weaponList[itemID];
						equipmentManagerScript.dpsWeapon = itemName;
					} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-secondDpsweapon" ) {
						equipmentManagerScript.dpsSecondWeaponObject = weaponItemScript.weaponList[itemID];
					}
					hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
					hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
					this.transform.SetParent( hoverControlScript.hoveredEquipSlot.transform );
				} else {
						print ("cannot equip this weapon");
				}
			} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-healerweapon" || hoverControlScript.hoveredEquipSlot.name == "Panel-secondHealerweapon" ) {
				if( hoveredWeaponType == weaponType.glove || hoveredWeaponType == weaponType.cursedGlove  ){
					if( hoverControlScript.hoveredEquipSlot.name == "Panel-healerweapon" ){
						equipmentManagerScript.healerWeaponObject = weaponItemScript.weaponList[itemID];
						equipmentManagerScript.healerWeapon = itemName;
					} else if ( hoverControlScript.hoveredEquipSlot.name == "Panel-secondHealerweapon" ) {
						equipmentManagerScript.healerSecondWeaponObject = weaponItemScript.weaponList[itemID];
					}
					hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
					hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
					this.transform.SetParent( hoverControlScript.hoveredEquipSlot.transform );
				} else {
						print ("cannot equip this weapon");
				}
			}
			this.transform.SetParent( hoverControlScript.OriginalSlot.transform );
			soundContScript.playSound( audioclip );
			particleSystem.Play();

		}
		this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
		dragging = false; 
	}

	void Awake(){
		colliderScript = GetComponent<BoxCollider2D>();
		detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
		hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
		equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<equipmentManager>();
		weaponItemScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<weaponItems>();
		soundContScript = GetComponent<soundController>();
		particleSystem = GetComponent<ParticleSystem>();
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
