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

		void OnMouseDown()
		{
			var allSlots = equipmentManagerScript.slotManager.slots; //GameObject.FindGameObjectsWithTag("item-slot");
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

		void EquipItem(weaponModel weaponObject)
		{
            weaponItemScript.isEquipped = true;
            weaponObject = weaponItemScript;
			SavedDataManager.SavedDataManagerInstance.AddWeaponModel(weaponItemScript, true);
			var equipControl = hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>();
            equipControl.equippedWeapon = this.gameObject;
			equipControl.ShowItemQuality();
            hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
			equipControl.qualityDisplay = hoverControlScript.hoveredEquipSlot.transform.Find("itemQuality").GetComponent<Image>();
            this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
        }

		void EquipBauble(bauble baubleObject, classType classType)
		{
            baubleItemScript.isEquipped = true;
            baubleObject = baubleItemScript;
            hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedBauble = this.gameObject;
            hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
            this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
            SavedDataManager.SavedDataManagerInstance.AddBauble(baubleItemScript, classType.ToString());
        }

		void OnMouseUp()
		{
			hoverControlScript.draggedItem = null;
			holdTimeTask.Stop();
			colliderScript.enabled = true;
			var allSlots = equipmentManagerScript.slotManager.slots;
			foreach (GameObject slotItem in allSlots)
			{
				slotItem.GetComponent<slotBehaviour>().colliderScript.enabled = false;
			}
			if (!hoverControlScript.hoveredEquipSlot)
			{
				if (hoverControlScript.hoveredSlot != null)
				{
					var originalEquipSlot = hoverControlScript.OriginalSlot.GetComponent<equipControl>();
					if (originalEquipSlot)
					{
                        hoverControlScript.OriginalSlot.GetComponent<equipControl>().ClearItemQuality();
                    }
                    ClearCurrentEquip(hoverControlScript.OriginalSlot);
					this.transform.SetParent(hoverControlScript.hoveredSlot.transform.childCount > 0 ? hoverControlScript.OriginalSlot.transform : hoverControlScript.hoveredSlot.transform);
				}
				equipped = false;
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(5f, 5f));
            }
			else
			{
				var allweapons = equipmentManagerScript.slotManager.items;
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
                            EquipItem(equipmentManagerScript.tankWeaponObject);
                            /*weaponItemScript.isEquipped = true;
							equipmentManagerScript.tankWeaponObject = weaponItemScript;
							SavedDataManager.SavedDataManagerInstance.AddWeaponModel(weaponItemScript, true);*/
						}
						else if (hoverControlScript.hoveredEquipSlot.name == "Panel-secondTankWeapon")
						{
                            EquipItem(equipmentManagerScript.tankSecondWeaponObject);
                            /*weaponItemScript.isEquipped = true;
							equipmentManagerScript.tankSecondWeaponObject = weaponItemScript;
							SavedDataManager.SavedDataManagerInstance.AddWeaponModel(weaponItemScript, false);*/
						}
						/*hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedWeapon = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);*/
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
                            EquipItem(equipmentManagerScript.dpsWeaponObject);
						}
						else if (hoverControlScript.hoveredEquipSlot.name == "Panel-secondDpsweapon")
						{
                            EquipItem(equipmentManagerScript.dpsSecondWeaponObject);
						}
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
                            EquipItem(equipmentManagerScript.healerWeaponObject);
						}
						else if (hoverControlScript.hoveredEquipSlot.name == "Panel-secondHealerweapon")
						{
                            EquipItem(equipmentManagerScript.healerSecondWeaponObject);
						}
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
						EquipBauble(equipmentManagerScript.tankBaubleObject, classType.guardian);
                        /*baubleItemScript.isEquipped = true;
						equipmentManagerScript.tankBaubleObject = baubleItemScript;
						hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedBauble = this.gameObject;
						hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
						this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
						SavedDataManager.SavedDataManagerInstance.AddBauble(baubleItemScript, "guardian");*/
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
                        EquipBauble(equipmentManagerScript.dpsBaubleObject, classType.stalker);
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
                        EquipBauble(equipmentManagerScript.healerBaubleObject, classType.walker);
					}
					else
					{
						print("cannot equip this bauble");
					}
				}
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(0f, 0f));
                this.transform.SetParent(hoverControlScript.OriginalSlot.transform);
                particleSystem.Play();
			}
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
			
		}

		void Start()
		{
			colliderScript = GetComponent<BoxCollider2D>();
			detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
			hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
			equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<equipmentManager>();
			//soundContScript = GetComponent<soundController>();
			particleSystem = GetComponent<ParticleSystem>();
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