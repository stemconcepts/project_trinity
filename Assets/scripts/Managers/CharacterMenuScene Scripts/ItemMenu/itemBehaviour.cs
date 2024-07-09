using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Assets.scripts.Helpers.Utility.GenericMouseInteraction;
using Assets.scripts.Helpers.Utility;

namespace AssemblyCSharp
{
	public class itemBehaviour : GenericMouseInteraction
    {
		hoverManager hoverControlScript;
		InventoryManager equipmentManagerScript;
		public WeaponModel weaponItemScript;
		public Bauble baubleItemScript;
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
			if (true)
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

		WeaponModel EquipItemAndReturnWeaponModel(bool primaryWeapon, GameObject slot = null)
		{
            equipControl equipControl = null;
            if (slot != null && hoverControlScript != null)
			{
                equipControl = slot.GetComponent<equipControl>();
                this.transform.SetParent(slot.transform);
                hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot?.gameObject;
            } else
			{
                equipControl = hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>();
                this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
            }

            weaponItemScript.isEquipped = true;
			SavedDataManager.SavedDataManagerInstance.AddWeaponModel(weaponItemScript, primaryWeapon);
            equipControl.equippedWeapon = this.gameObject;
			equipControl.ShowItemQuality();
            MainGameManager.instance.soundManager.playSound(MainGameManager.instance.soundManager.uiSounds[4]);
			return weaponItemScript;
        }

		Bauble EquipBauble(classType classType)
		{
            baubleItemScript.isEquipped = true;
            hoverControlScript.hoveredEquipSlot.GetComponent<equipControl>().equippedBauble = this.gameObject;
            hoverControlScript.OriginalSlot = hoverControlScript.hoveredEquipSlot.gameObject;
            this.transform.SetParent(hoverControlScript.hoveredEquipSlot.transform);
			SavedDataManager.SavedDataManagerInstance.AddBauble(baubleItemScript, classType.ToString());
			return baubleItemScript;

        }

		private GameObject GetFreeSlotName()
		{
			switch (type)
			{
				case weaponType.bladeAndBoard:
					return equipmentManagerScript.tankWeaponObject == null ? equipmentManagerScript.tankfirstSlot : equipmentManagerScript.tanksecondSlot;
                case weaponType.heavyHanded:
                    return equipmentManagerScript.tankWeaponObject == null ? equipmentManagerScript.tankfirstSlot : equipmentManagerScript.tanksecondSlot;
                case weaponType.dualBlades:
                    return equipmentManagerScript.dpsWeaponObject == null ? equipmentManagerScript.dpsfirstSlot : equipmentManagerScript.dpssecondSlot;
                case weaponType.clawAndCannon:
                    return equipmentManagerScript.dpsWeaponObject == null ? equipmentManagerScript.dpsfirstSlot : equipmentManagerScript.dpssecondSlot;
                case weaponType.glove:
                    return equipmentManagerScript.healerWeaponObject == null ? equipmentManagerScript.healerfirstSlot : equipmentManagerScript.healersecondSlot;
                case weaponType.cursedGlove:
                    return equipmentManagerScript.healerWeaponObject == null ? equipmentManagerScript.healerfirstSlot : equipmentManagerScript.healersecondSlot;
            }
			return null;
		}

		public bool TryToEquip(weaponType weaponType, GameObject slot)
		{
			if (slot.name == "Panel-tankweapon" || slot.name == "Panel-secondTankWeapon")
			{
				if (weaponType == weaponType.heavyHanded || weaponType == weaponType.bladeAndBoard)
				{
					if (slot.name == "Panel-tankweapon")
					{
						equipmentManagerScript.tankWeaponObject = EquipItemAndReturnWeaponModel(true, slot);
                        return true;
                    }
					else if (slot.name == "Panel-secondTankWeapon")
					{
						equipmentManagerScript.tankSecondWeaponObject = EquipItemAndReturnWeaponModel(false, slot);
                        return true;
                    }
				}
				else
				{
					print("cannot equip this weapon");
				}
			}
			else if (slot.name == "Panel-dpsweapon" || slot.name == "Panel-secondDpsweapon")
			{
				if (weaponType == weaponType.clawAndCannon || weaponType == weaponType.dualBlades)
				{
					if (slot.name == "Panel-dpsweapon")
					{
						equipmentManagerScript.dpsWeaponObject = EquipItemAndReturnWeaponModel(true, slot);
                        return true;
                    }
					else if (slot.name == "Panel-secondDpsweapon")
					{
						equipmentManagerScript.dpsSecondWeaponObject = EquipItemAndReturnWeaponModel(false, slot);
                        return true;
                    }
				}
				else
				{
					print("cannot equip this weapon");
				}
			}
			else if (slot.name == "Panel-healerweapon" || slot.name == "Panel-secondHealerweapon")
			{
				if (weaponType == weaponType.glove || weaponType == weaponType.cursedGlove)
				{
					if (slot.name == "Panel-healerweapon")
					{
						equipmentManagerScript.healerWeaponObject = EquipItemAndReturnWeaponModel(true, slot);
                        return true;
                    }
					else if (slot.name == "Panel-secondHealerweapon")
					{
						equipmentManagerScript.healerSecondWeaponObject = EquipItemAndReturnWeaponModel(false, slot);
                        return true;
                    }
				}
				else
				{
					print("cannot equip this weapon");
				}
			}
			else if (slot.name == "Panel-tankAccessorySlot")
			{
				if (weaponType == weaponType.bauble)
				{
					equipmentManagerScript.tankBaubleObject = EquipBauble(classType.guardian);
                    return true;
                }
				else
				{
					print("cannot equip this bauble");
				}
			}
			else if (slot.name == "Panel-dpsAccessorySlot")
			{
				if (weaponType == weaponType.bauble)
				{
					equipmentManagerScript.dpsBaubleObject = EquipBauble(classType.stalker);
                    return true;
                }
				else
				{
					print("cannot equip this bauble");
				}
			}
			else if (slot.name == "Panel-healerAccessorySlot")
			{
				if (weaponType == weaponType.bauble)
				{
					equipmentManagerScript.healerBaubleObject = EquipBauble(classType.walker);
                    return true;
                }
				else
				{
					print("cannot equip this bauble");
				}
			}

            MainGameManager.instance.GenericEventManager.CreateGenericEventOrTriggerEvent(GenericEventEnum.EquipmentReady);
            return false;
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
                    MainGameManager.instance.soundManager.playSound(MainGameManager.instance.soundManager.uiSounds[1]);
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
				var hoveredWeaponType = hoverControlScript.lastDraggedItem.GetComponent<itemBehaviour>().type;
                equipped = TryToEquip(hoveredWeaponType, hoverControlScript.hoveredEquipSlot);
                MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(0f, 0f));
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
			equipmentManagerScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<InventoryManager>();
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

		void EquipWithRightClick()
		{
			if (equipped)
			{
				return;
			}
			var item = CheckRightClick();
            if (item != null && item == this.gameObject)
            {
                hoverControlScript.draggedItem = this.gameObject;
                equipped = TryToEquip(type, GetFreeSlotName());
				MainGameManager.instance.ResetAnchorPoints(this.gameObject, new Vector2(5f, 5f));
            }
		}

		// Update is called once per frame
		void Update()
		{
			EquipWithRightClick();
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
				transform.position = rayPoint;
			}
		}
	}
}