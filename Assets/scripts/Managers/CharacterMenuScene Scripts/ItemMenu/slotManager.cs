using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class OwnedItems
    {
        public List<weaponModel> weapons = new List<weaponModel>();
        public List<bauble> baubles = new List<bauble>();
    }

    public class slotManager : MonoBehaviour
    {
        public List<GameObject> slots = new List<GameObject>();
        public List<GameObject> items = new List<GameObject>();
        public OwnedItems ownedItems = new OwnedItems();
        public GameObject slotPrefab;
        public GameObject itemPrefab;
        public GameObject menuManager;
        AssetFinder assetFinder;
        SavedDataManager savedDataManager;
        InventoryManager equipmentManager;

        public int slotAmount;

        void AddSlot(int i)
        {
            var newSlot = (GameObject)Instantiate(slotPrefab);
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
            assetFinder = MainGameManager.instance.assetFinder;
            savedDataManager = SavedDataManager._savedDataManagerInstance; 
        }

        void Start()
        {
            var weapons = assetFinder.GetAllWeapons();
            var baubles = assetFinder.GetAllBaubles();
            if (savedDataManager)
            {
                SetEquippedItemsAndSkills();
            }

            for (int i = 0; i < weapons.Count; i++)
            {
                ownedItems.weapons.Add(weapons[i]);
            }

            for (int i = 0; i < baubles.Count; i++)
            {
                ownedItems.baubles.Add(baubles[i]);
            }

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
                        case weaponModel.weaponType.bladeAndBoard:
                        case weaponModel.weaponType.heavyHanded:
                            if (equipmentManager.tankWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-tankweapon").transform, false);
                            }
                            else if (equipmentManager.tankSecondWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-secondTankWeapon").transform, false);
                            }
                            MainGameManager.instance.SceneManager.tankReady = equipmentManager.tankWeaponObject && equipmentManager.tankSecondWeaponObject && equipmentManager.tankClassSkill;
                            break;
                        case weaponModel.weaponType.clawAndCannon:
                        case weaponModel.weaponType.dualBlades:
                            if (equipmentManager.dpsWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-dpsweapon").transform, false);
                            }
                            else if (equipmentManager.dpsSecondWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-secondDpsweapon").transform, false);
                            }
                            MainGameManager.instance.SceneManager.dpsReady = equipmentManager.dpsWeaponObject && equipmentManager.dpsSecondWeaponObject && equipmentManager.dpsClassSkill;
                            break;
                        case weaponModel.weaponType.cursedGlove:
                        case weaponModel.weaponType.glove:
                            if (equipmentManager.healerWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-healerweapon").transform, false);
                            }
                            else if (equipmentManager.healerSecondWeaponObject == item)
                            {
                                newitem.transform.SetParent(GameObject.Find("Panel-secondHealerweapon").transform, false);
                            }
                            MainGameManager.instance.SceneManager.healerReady = equipmentManager.healerWeaponObject && equipmentManager.healerSecondWeaponObject && equipmentManager.healerClassSkill;
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