using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static AssemblyCSharp.miniMapIconBase;

namespace AssemblyCSharp
{
    public class ExploreManager : MonoBehaviour
    {
        public static GameManager gameManager;
        public static AssetFinder assetFinder;
        public static string currentRoom;
        public static GameObject backButton;
        public static GameObject inventoryHolder;
        public DungeonSettings dungeonSettings;
        DungeonSettings dungeonSettingsCopy;
        public static List<DungeonRoom> allRooms = new List<DungeonRoom>();
        public static List<DungeonRoom> mainRooms = new List<DungeonRoom>();
        public static List<miniMapIconBase> iconControllers = new List<miniMapIconBase>();
        public static List<DungeonRoom> previousRooms = new List<DungeonRoom>();
        public static int rand;
        [Header("Encounters")]
        public int largeEncounters;
        public int smallEncounters;
        [Header("Explorer Objects")]
        public GameObject explorerCanvas;
        public GameObject miniMap;
        Transform roomIconPosition;
        public GameObject roomTemplate;
        public GameObject routeTemplate;
        public GameObject roomIconTemplate;
        public GameObject objectTemplate;
        public GameObject explorerItemTemplate;
        public GameObject enemyEncounterTemplate;
        [Header("Inventory")]
        public static List<ItemBase> obtainedItems = new List<ItemBase>();

        void Awake()
        {
            gameManager = gameObject.GetComponent<GameManager>();
            assetFinder = gameObject.GetComponent<AssetFinder>();
        }

        void Start()
        {
            dungeonSettingsCopy = UnityEngine.Object.Instantiate(dungeonSettings);
            backButton = GameObject.Find("backButton");
            inventoryHolder = GameObject.Find("inventoryHolder");
            Invoke("LevelGenerator", 0.1f);
        }

        public void LevelGenerator()
        {
            GenerateRooms(dungeonSettings.minRooms);
            GenerateDetours(dungeonSettingsCopy.maxDetourLength);
            GetTotalRoomsAndHide();
            AddRandomEncounters();
            SetCurrentRoom(mainRooms[mainRooms.Count - 1].gameObject.name);
        }

        public static BackRoute GetBackButton()
        {
            return backButton.GetComponent<BackRoute>();
        }

        public static void AddToObtainedItems(ItemBase item)
        {
            obtainedItems.Add(item);
        }

        public static void RemoveObtainedItem(ItemBase item)
        {
            obtainedItems.Remove(item);
            //var x = inventoryHolder.transform.
            GameObject itemObj = inventoryHolder.transform.Find($"{item.name}").gameObject;
            Destroy(itemObj);
        }

        public static void AddPreviousRoom(DungeonRoom room)
        {
            previousRooms.Add(room);
            GameObject backButton = ExploreManager.GetBackButton().gameObject;
            if (!backButton.gameObject.activeSelf)
            {
                backButton.SetActive(true);
            }
        }

        public static void ChangeRouteInBackButton(DungeonRoom lastRoom)
        {
            BackRoute r = ExploreManager.GetBackButton();
            r.UpdateRouteDetails(r.gameObject.name = $"{r.gameObject.name}_Route_Back", lastRoom.gameObject.name, lastRoom.lockObj);
        }

        public static void ToggleRooms(bool show)
        {
            allRooms.ForEach(o =>
            {
                o.gameObject.SetActive(show);
            });
        }

        public static void SetCurrentRoom(string roomName)
        {
            currentRoom = roomName;
            miniMapIconBase currentIcon = iconControllers.Where(o => o.label == roomName).FirstOrDefault();
            iconControllers.ForEach(o =>
            {
                o.SetActive(false);
            });
            if (currentIcon)
            {
                currentIcon.SetActive(true);
            }
        }

        void AddCustomRoomIcon(GameObject roomIconGroupTemplate, miniMapIconBase startIconController)
        {
            GameObject roomIconGroup = Instantiate(roomIconGroupTemplate, miniMap.transform);

            for (int i = 0; i < roomIconGroup.transform.childCount; i++)
            {
                miniMapCustomIcon mmc = roomIconGroup.transform.GetChild(i).gameObject.GetComponent<miniMapCustomIcon>();
                iconControllers.Add(mmc);
                if (mmc.mapPosition == miniMapCustomIcon.mapPositionEnum.end)
                {
                    roomIconPosition = mmc.transform;
                }
            }
            Transform lastRoomLocation = startIconController.transform;
            roomIconGroup.transform.position = new Vector3(lastRoomLocation.position.x, lastRoomLocation.position.y + 0.4f);
        }

        void GenerateRoomIcon(DungeonRoom dr, lineDirectionEnum direction)
        {
            GameObject roomIcon = Instantiate(roomIconTemplate, miniMap.transform);
            miniMapIconController mmc = roomIcon.GetComponent<miniMapIconController>();
            mmc.label = dr.gameObject.name;
            if (iconControllers.Count > 0)
            {
                Transform lastRoomLocation = iconControllers[iconControllers.Count - 1].transform;
                if (dr.isDetour)
                {
                    Transform parentRoomTransform = iconControllers.Where(o => o.label == dr.parentRoom.gameObject.name).FirstOrDefault().transform;
                    float x = lineDirectionEnum.left == direction ? parentRoomTransform.position.x - 0.4f : parentRoomTransform.position.x + 0.4f;
                    mmc.ShowLine(direction);
                    if (parentRoomTransform)
                    {
                        roomIcon.transform.position = new Vector3(x, parentRoomTransform.position.y);
                    } else
                    {
                        roomIcon.transform.position = new Vector3(x, lastRoomLocation.position.y);
                    }
                } else
                {
                    mmc.ShowLine(lineDirectionEnum.down);
                    Transform trueLocation = roomIconPosition != null ? roomIconPosition : iconControllers[iconControllers.Count - 1].transform;
                    roomIcon.transform.position = new Vector3(trueLocation.position.x, trueLocation.position.y + 0.4f);
                    roomIconPosition = roomIconPosition != null ? null : roomIconPosition;
                }
            }
            dr.roomIcon = mmc;
            iconControllers.Add(mmc);
        }

        bool CanAddEncounter(enemyEncounter ee)
        {
            return dungeonSettingsCopy.enemyEncounters.Count > 0 && !ee.spawnOnce && dungeonSettingsCopy.maxSmallEncounters > smallEncounters;
        }

        void GenerateCustomRooms(DungeonRoom parentRoom)
        {
            int randomIndex = ExploreManager.gameManager.ReturnRandom(dungeonSettingsCopy.customRouteObjects.Count);
            GameObject customRoute = dungeonSettingsCopy.customRouteObjects[randomIndex];
            List<DungeonRoom> customRooms = new List<DungeonRoom>();
            int firstCustomRoomIndex = customRooms.Count;
            if (customRoute)
            {
                for (int c = 0; c < customRoute.transform.childCount; c++)
                {
                    GameObject customRoom = Instantiate(roomTemplate, explorerCanvas.transform);
                    customRoom.name = $"CustomRoom_{customRooms.Count}";
                    DungeonRoom dr = customRoom.GetComponent<DungeonRoom>();
                    dr.isCustomRoom = true;
                    dr.roomIcon = customRoute.transform.GetChild(c).GetComponent<miniMapCustomIcon>();
                    if (c == 0)
                    {
                        dr.InheritRouteFromParent(parentRoom, routeTemplate);
                    } else
                    {
                        dr.InheritRouteFromParent(customRooms[customRooms.Count - 1], routeTemplate);
                    }
                    GenerateRoomObject(dr, 3);
                    customRooms.Add(dr);
                    mainRooms.Add(dr);
                }
                if(customRooms.Any(o => o.name == "CustomRoom_0"))
                {
                    DungeonRoom firstCustomRoom = customRooms.Where(o => o.name == $"CustomRoom_{firstCustomRoomIndex}").FirstOrDefault();
                    DungeonRoom lastCustomRoom = customRooms.Where(o => o.name == $"CustomRoom_{customRooms.Count - 1}").FirstOrDefault();
                    //firstCustomRoom.InheritRouteFromParent(lastCustomRoom, routeTemplate);
                    lastCustomRoom.InheritRouteFromParent(firstCustomRoom, routeTemplate);
                }
            }
            AddCustomRoomIcon(customRoute, iconControllers[iconControllers.Count - 1]);
            dungeonSettingsCopy.customRouteObjects.RemoveAt(randomIndex);
        }

        bool CanGenerateCustomRoute(int roomCount)
        {
            int actualRoomPosition = dungeonSettings.minRooms - dungeonSettingsCopy.minRoomsBeforeCustomRoutes; //Do this because rooms are traversed backwards
            return roomCount > 0 && dungeonSettings.minRooms != (roomCount + 1) && actualRoomPosition <= roomCount && dungeonSettingsCopy.customRouteObjects.Count > 0;
        }

        bool CanLockDoor(int roomCount)
        {
            int actualLockedDoorPosition = dungeonSettings.minRooms - dungeonSettingsCopy.roomsBeforeLockedDoor; //Do this because rooms are traversed backwards
            //actualLockedDoorPosition = actualLockedDoorPosition == 2 ? 3 : actualLockedDoorPosition;
            return /*roomCount > 0 &&*/ dungeonSettings.minRooms != (roomCount + 1) && roomCount >= actualLockedDoorPosition && dungeonSettingsCopy.locks.Count > 0;
        }

        void DropKeysInRoom(List<KeyItem> keys)
        {
            mainRooms.Reverse();
            List<DungeonRoom> allowedRooms = mainRooms.Take(dungeonSettingsCopy.roomsBeforeLockedDoor).ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                DungeonRoom room = allowedRooms[ExploreManager.gameManager.ReturnRandom(allowedRooms.Count)];
                room.AddKey(keys[i], explorerItemTemplate);
            }
            mainRooms.Reverse();
        }

        void AddRandomEncounters()
        {
            var attempt = 0;
            while (smallEncounters < dungeonSettingsCopy.maxSmallEncounters && attempt != 3)
            {
                foreach (DungeonRoom room in allRooms)
                {
                    if (dungeonSettingsCopy.enemyEncounters.Count > 0 && GetChance(2) && room.encounter == null && !room.isStartingRoom)
                    {
                        AddEncounter(room);
                    }
                }
                ++attempt;
            }
        }

        void AddEncounter(DungeonRoom dr)
        {
            int encounterIndex = ExploreManager.gameManager.ReturnRandom(dungeonSettingsCopy.enemyEncounters.Count);
            enemyEncounter ee = dungeonSettingsCopy.enemyEncounters[encounterIndex];
            if (CanAddEncounter(ee))
            {
                dr.AddEncounter(ee, enemyEncounterTemplate);
                ++smallEncounters;
                Debug.Log($"Encounter created at {dr.name}");
            }
            else if (ee.spawnOnce)
            {
                dr.AddEncounter(ee, enemyEncounterTemplate);
                dungeonSettingsCopy.enemyEncounters.RemoveAt(encounterIndex);
                ++largeEncounters;
                Debug.Log($"Large encounter created at {dr.name}");
            }
        }

        void GenerateRooms(int numberOfRooms)
        {
            List<KeyItem> savedKeys = new List<KeyItem>();
            for (int i = 0; i < numberOfRooms; i++)
            {
                if (CanGenerateCustomRoute(i) && GetChance(dungeonSettings.chanceToGenerateCustomRoute))
                {
                    GenerateCustomRooms(mainRooms[mainRooms.Count - 1]);
                } else
                {
                    GameObject room = Instantiate(roomTemplate, explorerCanvas.transform);
                    DungeonRoom dr = room.GetComponent<DungeonRoom>();
                    room.name = $"Room_{mainRooms.Count}";
                    dr.isStartingRoom = i == (numberOfRooms - 1);
                    if (mainRooms.Count > 0)
                    {
                        DungeonRoom parentRoom = mainRooms[mainRooms.Count - 1];
                        if (CanLockDoor(i) && GetChance(2))
                        {
                            Debug.Log($"Locked door created at {parentRoom.name}");
                            int lockIndex = ExploreManager.gameManager.ReturnRandom(dungeonSettingsCopy.locks.Count);
                            parentRoom.lockObj = UnityEngine.Object.Instantiate(dungeonSettingsCopy.locks[lockIndex]);
                            dungeonSettingsCopy.locks.RemoveAt(lockIndex);
                            savedKeys.Add(parentRoom.lockObj.key);
                        }
                        dr.InheritRouteFromParent(parentRoom, routeTemplate);
                    }
                    GenerateRoomObject(dr, 3);
                    mainRooms.Add(dr);
                    if (!dr.isCustomRoom)
                    {
                        GenerateRoomIcon(dr, lineDirectionEnum.left);
                    }
                }
            }
            if (savedKeys.Count > 0)
            {
                DropKeysInRoom(savedKeys);
            }
        }

        Transform GetFreeSection(DungeonRoom room)
        {
            bool freeSpot = false;
            Transform result = null;
            int count = 0;
            while (!freeSpot && count < 5)
            {
                Transform section = room.foregroundHolder.transform.GetChild(ExploreManager.gameManager.ReturnRandom(room.foregroundHolder.transform.childCount)).transform;
                if (section.childCount == 0)
                {
                    result = section;
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        void GenerateRoomObject(DungeonRoom room, int amount)
        {
            amount = amount > 3 ? 3 : amount;
            for (int i = 1; i <= amount; i++)
            {
                if (GetChance(3))
                {
                    GameObject objectT = Instantiate(objectTemplate, GetFreeSection(room));
                    objectT.GetComponent<RoomObject>().GenerateItems(1);
                }
            }
        }

        DungeonRoom AddRoomFromParentRoom(DungeonRoom parentRoom, string suffix)
        {
            GameObject room = Instantiate(roomTemplate, explorerCanvas.transform);
            DungeonRoom dr = room.GetComponent<DungeonRoom>();
            room.name = $"{suffix}_Room_{Guid.NewGuid()}";
            dr.CreateRouteFromRoom(parentRoom, routeTemplate);
            if (suffix.ToLower() == "detour")
            {
                parentRoom.detourRooms.Add(dr);
                dr.parentRoom = parentRoom;
                dr.isDetour = true;
            }
            GenerateRoomObject(dr, 3);
            return dr;
        }

        void GenerateDetours(int detourLength)
        {
            mainRooms.Where(m => !m.isCustomRoom).ToList().ForEach(o =>
            {
                for (int i = 1; i <= (int)dungeonSettingsCopy.maxDetours; i++)
                {
                    /*if (CanGenerateCustomRoute(i) && GetChance(dungeonSettings.chanceToGenerateCustomRoute))
                    {
                        GenerateCustomRooms(o.detourRooms[o.detourRooms.Count - 1]);
                    }
                    else*/ if (GetChance(1) && detourLength > 0)
                    {
                        DungeonRoom r = AddRoomFromParentRoom(o, "Detour");
                        GenerateRoomIcon(r, i == 1 ? lineDirectionEnum.left : lineDirectionEnum.right);
                        if (r.roomIcon)
                        {
                            r.roomIcon.ShowLine(i == 1 ? lineDirectionEnum.right : lineDirectionEnum.left);
                        }
                        for (int x = 1; x <= detourLength; x++)
                        {
                            if (GetChance(1))
                            {
                                DungeonRoom parentRoom = r.detourRooms.Count == 0 ? r : r.detourRooms[r.detourRooms.Count - 1];
                                DungeonRoom n = AddRoomFromParentRoom(parentRoom, "Detour");
                                GenerateRoomIcon(n, i == 1 ? lineDirectionEnum.left : lineDirectionEnum.right);
                                if (n.roomIcon)
                                {
                                    n.roomIcon.ShowLine(i == 1 ? lineDirectionEnum.right : lineDirectionEnum.left);
                                }
                            }
                        }
                    }
                }
            });
        }

        public static bool GetChance(int maxChance)
        {
            rand = UnityEngine.Random.Range(0, (maxChance + 1));
            return rand == 1;
        }

        int ReturnRandom(int maxNumber)
        {
            rand = UnityEngine.Random.Range(0, maxNumber);
            return rand;
        }

        void GetTotalRoomsAndHide()
        {
            var allRoomGameObjects = GameObject.FindGameObjectsWithTag("dungeonRoom").ToList();
            allRoomGameObjects.ForEach(o =>
            {
                allRooms.Add(o.GetComponent<DungeonRoom>());
                o.SetActive(false);
            });
            mainRooms[mainRooms.Count - 1].gameObject.SetActive(true);
            allRooms[mainRooms.Count - 1].roomIcon.SetStartIcon();
            allRooms[0].roomIcon.SetEndIcon();
            backButton.gameObject.SetActive(false);
        }
    }
}

