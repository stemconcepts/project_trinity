using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class itemBehaviour : MonoBehaviour
	{
		hoverManager hoverControlScript;
		equipmentManager equipmentManagerScript;
		public weaponModel weaponItemScript;
		public bauble baubleItemScript;
		public Sprite itemIcon;
		public string itemName;
		public GameObject itemHoverObj;
		public GameObject liveItemHoverObj;
		public enum weaponType
		{
			bladeAndBoard,
			heavyHanded,
			dualBlades,
			clawAndCannon,
			glove,
			cursedGlove,
			bauble
		};
		public weaponType type;
		GameObject currentSlot;
		itemDetailsControl detailsControlScript;
		float distance;
		public bool dragging = false;
		public bool hovered = false;
		public bool equipped = false;
		Task holdTimeTask;
		BoxCollider2D colliderScript;
		//private soundController soundContScript;
		public AudioClip audioclip;
		public AudioClip audioclip2;
		public AudioClip audioclipEquip;
		private ParticleSystem particleSystem;

		//Move item
		void OnMouseDown()
		{
			//soundContScript.playSound(audioclip2);
			//sets active slotarea
			var allSlots = GameObject.FindGameObjectsWithTag("item-slot");
			foreach (GameObject slotItem in allSlots)
			{
				slotItem.GetComponent<slotBehaviour>().currentSlot = false;
				slotItem.GetComponent<Image>().color = slotItem.GetComponent<slotBehaviour>().inactiveColor;
				slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = true;
			}
			if (this.transform.parent.GetComponent<slotBehaviour>())
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
			}
			this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
			//if( this.transform.FindChild("Panel-item(Clone)") ){
			//this.transform.parent.GetComponent<slotBehaviour>().currentItemID = this.itemID;
			if (!equipped)
			{
				if (weaponItemScript != null)
				{
					detailsControlScript.DisplayWeaponData(weaponItemScript);
				}
				else
				{
					detailsControlScript.DisplayBaubleData(baubleItemScript);
				}
			}
			//}
			//preps drag
			holdTimeTask = new Task(holdtime(0.1f));
			colliderScript.enabled = false;
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

		void ClearCurrentEquip(GameObject originalEquipSlot)
		{
			if (originalEquipSlot.transform.childCount > 0)
			{
				var classEquipSlot = originalEquipSlot.name;
				if (classEquipSlot == "Panel-tankweapon")
				{
					equipmentManagerScript.tankWeaponObject = null;
				}
				else if (classEquipSlot == "Panel-secondTankWeapon")
				{
					equipmentManagerScript.tankSecondWeaponObject = null;
				}
				else if (classEquipSlot == "Panel-dpsweapon")
				{
					equipmentManagerScript.dpsWeaponObject = null;
				}
				else if (classEquipSlot == "Panel-secondDpsWeapon")
				{
					equipmentManagerScript.dpsSecondWeaponObject = null;
				}
				else if (classEquipSlot == "Panel-healerweapon")
				{
					equipmentManagerScript.healerWeaponObject = null;
				}
				else if (classEquipSlot == "Panel-secondHealerWeapon")
				{
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
			foreach (GameObject slotItem in allSlots)
			{
				slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = false;
			}
			if (!hoverControlScript.hoveredEquipSlot)
			{
				//this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
				if (hoverControlScript.hoveredSlot != null)
				{
					ClearCurrentEquip(hoverControlScript.OriginalSlot);
					this.transform.SetParent(hoverControlScript.hoveredSlot.transform.childCount > 0 ? hoverControlScript.OriginalSlot.transform : hoverControlScript.hoveredSlot.transform);
				}
				//this.transform.SetParent( hoverControlScript.hoveredSlot.transform );
				if (dragging)
				{
					//soundContScript.playSound(audioclip);
				}
				equipped = false;
			}
			else
			{
				//this.transform.SetParent( hoverControlScript.hoveredEquipSlot.transform );
				var allweapons = GameObject.FindGameObjectsWithTag("item-weapon");
				foreach (GameObject weaponItem in allweapons)
				{
					if (equipped == true)
					{
						weaponItem.GetComponent<itemBehaviour>().equipped = false;
					}
				}
				equipped = true;
				var hoveredWeaponType = hoverControlScript.lastDraggedItem.GetComponent<itemBehaviour>().type;
				if (hoverControlScript.hoveredEquipSlot.name == "Panel-tankweapon" || hoverControlScript.hoveredEquipSlot.name == "Panel-secondTankWeapon")
				{
					if (hoveredWeaponType == weaponType.heavyHanded || hoveredWeaponType == weaponType.bladeAndBoard)
					{
						if (hoverControlScript.hoveredEquipSlot.name == "Panel-tankweapon")
						{
							weaponItemScript.isEquipped = true;
							equipmentManagerScript.tankWeaponObject = weaponItemScript;
							equipmentManagerScript.gameManager.SavedDataManager.AddWeaponModel(weaponItemScript, true);
						}
						else if (hoverControlScript.hoveredEquipSlot.name == "Panel-secondTankWeapon")
						{
							weaponItemScript.isEquipped = true;
							equipmentManagerScript.tankSecondWeaponObject = weaponItemScript;
							equipmentManagerScript.gameManager.SavedDataManager.AddWeaponModel(weaponItemScript, false);
						}
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
					}
					else
					{
						print("cannot equip this weapon");
					}
				}
				else if (hoverControlScript.hoveredEquipSlot.name == "Panel-dpsweapon" || hoverControlScript.hoveredEquipSlot.name == "Panel-secondDpsweapon")
				{
					if (hoveredWeaponType == weaponType.clawAndCannon || hoveredWeaponType == weaponType.dualBlades)
					{
						if (hoverControlScript.hoveredEquipSlot.name == "Panel-dpsweapon")
						{
							weaponItemScript.isEquipped = true;
							equipmentManagerScript.dpsWeaponObject = weaponItemScript;
							equipmentManagerScript.gameManager.SavedDataManager.AddWeaponModel(weaponItemScript, true);
						}
						else if (hoverControlScript.hoveredEquipSlot.name == "Panel-secondDpsweapon")
						{
							weaponItemScript.isEquipped = true;
							equipmentManagerScript.dpsSecondWeaponObject = weaponItemScript;
							equipmentManagerScript.gameManager.SavedDataManager.AddWeaponModel(weaponItemScript, false);
						}
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
					}
					else
					{
						print("cannot equip this weapon");
					}
				}
				else if (hoverControlScript.hoveredEquipSlot.name == "Panel-healerweapon" || hoverControlScript.hoveredEquipSlot.name == "Panel-secondHealerweapon")
				{
					if (hoveredWeaponType == weaponType.glove || hoveredWeaponType == weaponType.cursedGlove)
					{
						if (hoverControlScript.hoveredEquipSlot.name == "Panel-healerweapon")
						{
							weaponItemScript.isEquipped = true;
							equipmentManagerScript.healerWeaponObject = weaponItemScript;
							equipmentManagerScript.gameManager.SavedDataManager.AddWeaponModel(weaponItemScript, true);
						}
						else if (hoverControlScript.hoveredEquipSlot.name == "Panel-secondHealerweapon")
						{
							weaponItemScript.isEquipped = true;
							equipmentManagerScript.healerSecondWeaponObject = weaponItemScript;
							equipmentManagerScript.gameManager.SavedDataManager.AddWeaponModel(weaponItemScript, false);
						}
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
					}
					else
					{
						print("cannot equip this weapon");
					}
				}
				else if (hoverControlScript.hoveredEquipSlot.name == "Panel-tankAccessorySlot")
				{
					if (hoveredWeaponType == weaponType.bauble)
					{
						baubleItemScript.isEquipped = true;
						equipmentManagerScript.tankBaubleObject = baubleItemScript;
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedBauble = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
					}
					else
					{
						print("cannot equip this bauble");
					}
				}
				else if (hoverControlScript.hoveredEquipSlot.name == "Panel-dpsAccessorySlot")
				{
					if (hoveredWeaponType == weaponType.bauble)
					{
						baubleItemScript.isEquipped = true;
						equipmentManagerScript.dpsBaubleObject = baubleItemScript;
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedBauble = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
					}
					else
					{
						print("cannot equip this bauble");
					}
				}
				else if (hoverControlScript.hoveredEquipSlot.name == "Panel-healerAccessorySlot")
				{
					if (hoveredWeaponType == weaponType.bauble)
					{
						baubleItemScript.isEquipped = true;
						equipmentManagerScript.healerBaubleObject = baubleItemScript;
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedBauble = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
					}
					else
					{
						print("cannot equip this bauble");
					}
				}

				this.transform.SetParent(hoverControlScript.OriginalSlot.transform);
				//soundContScript.playSound(audioclipEquip);
				particleSystem.Play();
			}
			this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
			dragging = false;
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
			itemHoverName.text = "<b>" + itemName + "</b>";
			hovered = true;
		}
		public void OnMouseExit()
		{
			hovered = false;
			Destroy(liveItemHoverObj);
		}

		void Awake()
		{
			colliderScript = GetComponent<BoxCollider2D>();
			detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
			hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
			equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<equipmentManager>();
			//soundContScript = GetComponent<soundController>();
			particleSystem = GetComponent<ParticleSystem>();
		}

		void Start()
		{
			if (itemIcon != null)
			{
				gameObject.GetComponent<Image>().sprite = itemIcon;
			}
			if (baubleItemScript != null)
			{
				itemName = baubleItemScript.baubleName;
			}
			else
			{
				itemName = weaponItemScript.DisplayName;
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
				liveItemHoverObj.transform.position = rayPoint;
			}
			if (dragging)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector2 rayPoint = ray.GetPoint(distance);
				rayPoint.y += 9;
				transform.position = rayPoint;
			}
		}
	}
}