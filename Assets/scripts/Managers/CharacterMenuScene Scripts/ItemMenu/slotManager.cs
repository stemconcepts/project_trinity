using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class OwnedItems
    {
        public List<WeaponModel> weapons = new List<WeaponModel>();
        public List<Bauble> baubles = new List<Bauble>();
    }

    public class slotManager : MonoBehaviour
    {
        public List<GameObject> slots = new List<GameObject>();
        public List<GameObject> items = new List<GameObject>();
        public OwnedItems ownedItems = new OwnedItems();
        public GameObject slotPrefab;
        public GameObject itemPrefab;
        public GameObject menuManager;
        SavedDataManager savedDataManager;
        InventoryManager equipmentManager;

        public int slotAmount;

        void AddSlot(int i)
        {
            var newSlot = Instantiate(slotPrefab);
            newSlot.GetComponent<slotBehaviour>().currentSlotID = i;
            slots.Add(newSlot);
            slots[i].transform.SetParent(this.transform);
        }

        Transform AddItemToSlot()
        {
            foreach (var slot in slots)
            {
                if (slot.transform.childCount == 0)
                {
                    return slot.transform;
                }
            }
            return null;
        }

        void SetEquippedItemsAndSkills()
        {
            PlayerData playerData = savedDataManager.LoadPlayerData();
            if (playerData != null)
            {
                equipmentManager.tankWeaponObject = playerData.tankEquipment.weapon;
                equipmentManager.tankSecondWeaponObject = playerData.tankEquipment.secondWeapon;
                equipmentManager.tankBaubleObject = playerData.tankEquipment.bauble;
                equipmentManager.tankClassSkill = playerData.tankEquipment.classSkill;

                equipmentManager.dpsWeaponObject = playerData.dpsEquipment.weapon;
                equipmentManager.dpsSecondWeaponObject = playerData.dpsEquipment.secondWeapon;
                equipmentManager.dpsBaubleObject = playerData.dpsEquipment.bauble;
                equipmentManager.dpsClassSkill = playerData.dpsEquipment.classSkill;

                equipmentManager.healerWeaponObject = playerData.healerEquipment.weapon;
                equipmentManager.healerSecondWeaponObject = playerData.healerEquipment.secondWeapon;
                equipmentManager.healerBaubleObject = playerData.healerEquipment.bauble;
                equipmentManager.healerClassSkill = playerData.healerEquipment.classSkill;
            }
        }

        // Use this for initialization
        void Awake()
        {
            equipmentManager = menuManager.GetComponent<InventoryManager>();
            savedDataManager = SavedDataManager._savedDataManagerInstance; 
        }

        void Start()
        {
            var weapons = MainGameManager.instance.EquipmentFinder.GetAllWeapons(true);
            var baubles = MainGameManager.instance.EquipmentFinder.GetAllBaubles(true);
            if (savedDataManager)
            {
                SetEquippedItemsAndSkills();
            }
            ownedItems.weapons.AddRange(weapons);
            ownedItems.baubles.AddRange(baubles);

            for (int i = 0; i < slotAmount; i++)
            {
                AddSlot(i);
            }

            foreach (var item in ownedItems.weapons)
            {
                var newitem = (GameObject)Instantiate(itemPrefab, AddItemToSlot());
                var newItemBehaviour = newitem.GetComponent<itemBehaviour>();
                newItemBehaviour.weaponItemScript = item;
                newItemBehaviour.type = (itemBehaviour.weaponType)item.type;
                newItemBehaviour.itemIcon = item.itemIcon;
                items.Add(newitem);
                if (newItemBehaviour.weaponItemScript.isEquipped)
                {
                    switch (newItemBehaviour.weaponItemScript.type)
                    {
                        case WeaponModel.weaponType.bladeAndBoard:
                        case WeaponModel.weaponType.heavyHanded:
                            if (equipmentManager.tankWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-tankweapon").transform, false);
                            }
                            else if (equipmentManager.tankSecondWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-secondTankWeapon").transform, false);
                            }
                            //MainGameManager.instance.SceneManager.tankReady = equipmentManager.tankWeaponObject && equipmentManager.tankSecondWeaponObject && equipmentManager.tankClassSkill;
                            break;
                        case WeaponModel.weaponType.clawAndCannon:
                        case WeaponModel.weaponType.dualBlades:
                            if (equipmentManager.dpsWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-dpsweapon").transform, false);
                            }
                            else if (equipmentManager.dpsSecondWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-secondDpsweapon").transform, false);
                            }
                            //MainGameManager.instance.SceneManager.dpsReady = equipmentManager.dpsWeaponObject && equipmentManager.dpsSecondWeaponObject && equipmentManager.dpsClassSkill;
                            break;
                        case WeaponModel.weaponType.cursedGlove:
                        case WeaponModel.weaponType.glove:
                            if (equipmentManager.healerWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-healerweapon").transform, false);
                            }
                            else if (equipmentManager.healerSecondWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-secondHealerweapon").transform, false);
                            }
                            //MainGameManager.instance.SceneManager.healerReady = equipmentManager.healerWeaponObject && equipmentManager.healerSecondWeaponObject && equipmentManager.healerClassSkill;
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (var item in ownedItems.baubles)
            {
                var newitem = (GameObject)Instantiate(itemPrefab, AddItemToSlot());
                var newItemBehaviour = newitem.GetComponent<itemBehaviour>();
                newItemBehaviour.baubleItemScript = item;
                newItemBehaviour.type = itemBehaviour.weaponType.bauble;
                newItemBehaviour.itemIcon = item.itemIcon;
                items.Add(newitem);
                if (newItemBehaviour.baubleItemScript.isEquipped)
                {
                    if (equipmentManager.tankBaubleObject == item)
                    {
                        newitem.transform.SetParent(GameObject.Find("Panel-tankAccessorySlot").transform);
                    }

                    if (equipmentManager.dpsBaubleObject == item)
                    {
                        newitem.transform.SetParent(GameObject.Find("Panel-dpsAccessorySlot").transform);
                    }

                    if (equipmentManager.healerBaubleObject == item)
                    {
                        newitem.transform.SetParent(GameObject.Find("Panel-healerAccessorySlot").transform);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}