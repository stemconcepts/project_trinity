using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using UnityEngine.UI;
using TMPro;
using AssemblyCSharp;
using static AssemblyCSharp.miniMapIconBase;

namespace Assets.scripts.Managers
{
    public class detourLink
    {
        public miniMapIconBase start;
        public miniMapIconBase end;
        public int distance;
    }

    public class ExploreManagerV2 : MonoBehaviour
    {
        public CharacterInfoDisplayController characterInfoDisplayController;
        private bool useBackwardRoute;
        public bool lootAdded;
        public GameManager gameManager;
        public AssetFinder assetFinder;
        public string currentRoom;
        public GameObject backButton;
        public GameObject inventoryHolder;
        [HideInInspector]
        private corruptionController corruptionHolder;
        private int stepCounter;
        public DungeonSettings dungeonSettings;
        DungeonSettings dungeonSettingsCopy;
        public List<DungeonRoom> allRooms = new List<DungeonRoom>();
        public List<DungeonRoom> mainRooms = new List<DungeonRoom>();
        public List<DungeonRoom> detourRooms = new List<DungeonRoom>();
        public List<miniMapIconBase> iconControllers = new List<miniMapIconBase>();
        public List<DungeonRoom> previousRooms = new List<DungeonRoom>();

        [Header("Transitions")]
        public Animator roomTransition;

        [Header("Encounters")]
        public int largeEncounters;
        public int smallEncounters;

        [Header("Explorer Objects")]
        public GameObject explorerCanvas;
        public GameObject miniMap;
        Transform roomIconPosition;
        public GameObject routeTemplate;
        public GameObject roomIconTemplate;
        public GameObject objectTemplate;
        public GameObject explorerItemTemplate;
        public GameObject fieldItemTemplate;
        public GameObject enemyEncounterTemplate;

        [Header("Health Bars")]
        public Slider tankHealth;
        public Slider dpsHealth;
        public Slider healerHealth;

        [Header("Inventory")]
        public List<ItemBase> obtainedItems = new List<ItemBase>();
        public Camera explorerCamera;

        void Awake()
        {
            gameManager = gameObject.GetComponent<GameManager>();
            assetFinder = gameObject.GetComponent<AssetFinder>();
        }

        void Start()
        {
            dungeonSettingsCopy = UnityEngine.Object.Instantiate(dungeonSettings);
            dungeonSettings = dungeonSettingsCopy;
            backButton = GameObject.Find("EyeButton");
            inventoryHolder = GameObject.Find("inventoryGrid");
            corruptionHolder = GameObject.Find("CorruptionCounter").GetComponent<corruptionController>();
            SetCurrentHealth();
            Invoke("LevelGenerator", 0.1f);
        }

        /// <summary>
        /// Returns transition for changing rooms
        /// </summary>
        /// <returns></returns>
        public  Animator GetRoomTransition()
        {
            return roomTransition;
        }

        /// <summary>
        /// increases step count and adds 1 curroption to counter every 2 steps, forwards or backwards
        /// </summary>
        /// <param name="amount"></param>
        public void AddStep(int steps, int curroptionAmount)
        {
            corruptionHolder.AddCorruption(curroptionAmount);
            stepCounter += steps;
            /*if (stepCounter%2 == 0)
            {
                corruptionHolder.GetComponent<corruptionController>().AddCorruption(1);
            }*/
        }

        /// <summary>
        /// Return current curroption
        /// </summary>
        /// <returns></returns>
        public int GetCurroption()
        {
            return corruptionHolder.corruptionAmount;
        }

        public void ResetCurroption()
        {
            corruptionHolder.ReduceCorruption(GetCurroption());
        }

        public void EditCurroption(int value)
        {
            corruptionHolder.AddCorruption(value);
        }

        public void LevelGenerator()
        {
            /*needs to check if its loading a new level or not
            var d = LoadDungeon();
            if (d.allRooms.Length == 0 && string.IsNullOrEmpty(d.currentRoomId))
            {*/
            //lootAdded = false;
            GenerateRooms(dungeonSettings.minRooms);
            GenerateDetours(dungeonSettingsCopy.maxDetourLength);
            LinkDetours();
            AddLockedDoors();
            GetTotalRoomsAndHide();
            AddRandomEncounters();
            AddCurroptionToRoutes();
            SetCurrentRoom(mainRooms[mainRooms.Count - 1].gameObject.name);
            SavedDataManager.SavedDataManagerInstance.SaveIconPos(iconControllers);
            //}
        }

        /// <summary>
        /// Add random curroption 0 - 3 to random routes
        /// </summary>
        void AddCurroptionToRoutes()
        {
            allRooms.ForEach(o =>
            {
                o.routes.ForEach(r =>
                {
                    var curroption = UnityEngine.Random.Range(0, 4);
                    r.curroptionAmount = curroption;
                });
            });
        }

        public  void SetUseBackwardRoute(bool setTrue)
        {
            useBackwardRoute = setTrue;
        }

        public  bool IsBackwardRoute()
        {
            return useBackwardRoute;
        }

        void LinkDetours()
        {
            List<detourLink> x = new List<detourLink>();
            iconControllers.ForEach(o =>
            {
                var match = iconControllers.Find(i => i.depth == o.depth && i.lineDirection == o.lineDirection && !i.isMainIcon && !i.isCustomIcon && i.label != o.label);
                if (match && !x.Any(d => d.end == match) /*&& !x.Any(d => d.start == match)*/)
                {
                    var item = new detourLink()
                    {
                        start = match,
                        end = o
                    };

                    item.distance = Math.Abs(item.start.masterDepth - item.end.masterDepth) - 1;
                    x.Add(item);
                }
            });

            x.Reverse();
            x.ForEach(o =>
            {
                DungeonRoom lastRoom = null;
                Debug.Log(o.distance);
                for (int i = 0; i <= o.distance; i++)
                {
                    var room = lastRoom == null ? detourRooms.Find(a => a.name == o.start.label) : lastRoom;
                    var linkedEndRoom = detourRooms.Find(a => a.name == o.end.label);
                    if (room && o.distance == 0)
                    {
                        room.CreateRouteFromRoom(linkedEndRoom, routeTemplate, null);
                        room.roomIcon.ShowLine(lineDirectionEnum.down);
                        linkedEndRoom.CreateRouteFromRoom(room, routeTemplate, null);
                    }
                    else if (room && o.distance != i && GameManager.GetChanceByPercentage(0.9f))
                    {
                        DungeonRoom r = AddRoomFromParentRoom(room, "Detour_Connector");
                        if (i == (o.distance - 1))
                        {
                            r.CreateRouteFromRoom(linkedEndRoom, routeTemplate, null);
                            linkedEndRoom.CreateRouteFromRoom(r, routeTemplate, null);
                        }
                        r.id = $"room_detour_connector_{i}_{room.id}";
                        GenerateRoomIcon(r, lineDirectionEnum.down, false, o.start.depth, o.start.masterDepth + 1);
                        r.roomIcon.SetDetourConnectorColour();
                        r.roomIcon.SetObjectName($"RoomIcon_Detour_Connector_Parent-{o.start.label}");
                        detourRooms.Add(r);
                        lastRoom = r;
                    }
                }
                lastRoom = null;
            });
        }

        public BackRoute GetBackButton()
        {
            return backButton.GetComponent<BackRoute>();
        }

        //Move to external script that sits on obtained items slot
        public  void AddToObtainedItems(ItemBase item, GameObject gameObject = null)
        {
            inventoryHolder.GetComponent<fieldInventoryController>().AddToObtainedItems(item, gameObject);
        }

        public void RemoveObtainedItem(ItemBase item)
        {
            /*obtainedItems.Remove(item);
            GameObject itemObj = inventoryHolder.transform.Find($"{item.name}").gameObject;
            Destroy(itemObj);*/
            inventoryHolder.GetComponent<fieldInventoryController>().RemoveFromObtainedItems(item);
        }

        public void AddPreviousRoom(DungeonRoom room)
        {
            previousRooms.Add(room);
            GameObject backButton = GetBackButton().gameObject;
            if (!backButton.gameObject.activeSelf)
            {
                backButton.SetActive(true);
            }
            SavedDataManager.SavedDataManagerInstance.EditPreviousRoom(room.name, true);
        }

        public void ChangeRouteInBackButton(DungeonRoom lastRoom)
        {
            BackRoute r = GetBackButton();
            r.UpdateRouteDetails(r.gameObject.name = $"{r.gameObject.name}_Route_Back", lastRoom.gameObject.name, lastRoom.lockObj);
        }

        public void ToggleRooms(bool show)
        {
            allRooms.ForEach(o =>
            {
                o.gameObject.SetActive(show);
            });
        }

        public DungeonRoom GetCurrentRoom()
        {
            return allRooms.Find(o => o.name == currentRoom);
        }

        public void SetCurrentRoom(string roomName)
        {
            currentRoom = roomName;
            iconControllers = iconControllers.Where(o => o != null).ToList();
            miniMapIconBase currentIcon = iconControllers.Where(o => o.label == roomName).FirstOrDefault();
            iconControllers.ForEach(o =>
            {
                o.SetActive(false);
            });
            if (currentIcon)
            {
                currentIcon.SetActive(true);
            }
            //var x = allRooms.Where(o => o.visited).Select(o => o.name).ToString();
            SavedDataManager.SavedDataManagerInstance.SaveCurrentRoomData(roomName);
        }

        /*DungeonData LoadDungeon()
        {
            DungeonData data = SavedDataManager.SavedDataManagerInstance.LoadDungeonData();
            if (data.allRooms.Length > 0 && !string.IsNullOrEmpty(data.currentRoomId))
            {
                foreach (var r in data.allRooms)
                {
                    GameObject room = Instantiate(dungeonSettings.ReturnRandomRoom(), explorerCanvas.transform);
                    DungeonRoom dr = room.GetComponent<DungeonRoom>();
                    room.name = r.name;
                    dr.isStartingRoom = r.isStartingRoom;
                    dr.isDetour = r.isDetour;
                    dr.isCustomRoom = r.isCustomRoom;
                    dr.id = r.id;
                    dr.visited = r.visited;
                    dr.parentRoom = allRooms.Where(a => a.name == r.parentRoomName).FirstOrDefault();
                    dr.gameObject.SetActive(r.name == data.currentRoomId);
                    dr.routeLocations = r.routeLocations.ToList();
                    r.roomObjects.ToList().ForEach(o =>
                    {
                        PlaceRoomObject(dr, o.position);
                    });
                    r.routes.ToList().ForEach(ru =>
                    {
                        dr.InsertRouteFromData(ru, routeTemplate);
                    });
                    allRooms.Add(dr);
                }

                allRooms = allRooms.Where(o => o != null).ToList();
                var roomsVisited = allRooms.Where(o => o.visited).Select(c =>
                {
                    return c.gameObject.name;
                }).ToArray();

                previousRooms = allRooms.Where(o => data.previousRooms.Contains(o.name)).ToList();
                previousRooms.Reverse();
                PlaceSavedIcons(data.miniMapIconData, roomsVisited);
                SetCurrentRoom(data.currentRoomId);
                dungeonSettings.enemyEncounters = dungeonSettings.enemyEncounters.Where(o => !data.enemysKilled.Contains(o.id)).ToList();
            }
            return data;
        }*/

        void PlaceSavedIcons(IconData[] icons, string[] visitedRooms)
        {
            var i = 1;
            icons.ToList().ForEach(o =>
            {
                GameObject roomIcon = Instantiate(roomIconTemplate, miniMap.transform);
                miniMapIconController mmc = roomIcon.GetComponent<miniMapIconController>();
                roomIcon.transform.position = new Vector2(o.position.x, o.position.y);
                mmc.label = o.label;
                mmc.ShowLine((lineDirectionEnum)o.direction);
                if (visitedRooms.ToList().Any(r => r == mmc.label))
                {
                    mmc.SetVisited();
                }
                if (i == icons.Length)
                {
                    mmc.SetStartIcon();
                }
                if (i == 0)
                {
                    mmc.SetEndIcon();
                }
                iconControllers.Add(mmc);
                allRooms.ForEach(r =>
                {
                    if (r.gameObject.name == mmc.label)
                    {
                        r.roomIcon = mmc;
                    }
                });
                i++;
            });
        }

        void AddCustomRoomIcon(GameObject roomIconGroupTemplate, miniMapIconBase startIconController)
        {
            GameObject roomIconGroup = Instantiate(roomIconGroupTemplate, miniMap.transform);

            for (int i = 0; i < roomIconGroup.transform.childCount; i++)
            {
                miniMapCustomIcon mmc = roomIconGroup.transform.GetChild(i).gameObject.GetComponent<miniMapCustomIcon>();
                mmc.isCustomIcon = true;
                iconControllers.Add(mmc);
                if (mmc.mapPosition == miniMapCustomIcon.mapPositionEnum.end)
                {
                    roomIconPosition = mmc.transform;
                }
            }
            Transform lastRoomLocation = startIconController.transform;
            roomIconGroup.transform.position = new Vector3(lastRoomLocation.position.x, lastRoomLocation.position.y + 0.4f);
        }

        void GenerateRoomIcon(DungeonRoom dr, lineDirectionEnum direction, bool isMainIcon, int depth, int masterDepth = 0)
        {
            GameObject roomIcon = Instantiate(roomIconTemplate, miniMap.transform);
            miniMapIconController mmc = roomIcon.GetComponent<miniMapIconController>();
            mmc.isMainIcon = isMainIcon;
            mmc.depth = depth;
            if (!isMainIcon)
            {
                mmc.masterDepth = masterDepth;
            }
            mmc.label = dr.gameObject.name;
            if (direction == lineDirectionEnum.down && !isMainIcon)
            {
                Transform parentRoomTransform = dr.parentRoom.roomIcon.transform;
                dr.parentRoom.roomIcon.ShowLine(lineDirectionEnum.down);
                mmc.ShowLine(direction);
                roomIcon.transform.position = new Vector3(parentRoomTransform.position.x, parentRoomTransform.position.y - 0.4f);
            }
            else if (iconControllers.Count > 0)
            {
                Transform lastRoomLocation = iconControllers[iconControllers.Count - 1].transform;
                if (dr.isDetour)
                {
                    Transform parentRoomTransform = iconControllers.Where(o => o.label == dr.parentRoom.gameObject.name).FirstOrDefault().transform;
                    float x = lineDirectionEnum.left == direction ? parentRoomTransform.position.x - 0.4f : parentRoomTransform.position.x + 0.4f;
                    //mmc.ShowLine(direction);
                    if (parentRoomTransform)
                    {
                        roomIcon.transform.position = new Vector3(x, parentRoomTransform.position.y);
                    }
                    else
                    {
                        roomIcon.transform.position = new Vector3(x, lastRoomLocation.position.y);
                    }
                }
                else
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
            int randomIndex = MainGameManager.instance.ReturnRandom(dungeonSettingsCopy.customRouteObjects.Count);
            GameObject customRoute = dungeonSettingsCopy.customRouteObjects[randomIndex];
            List<DungeonRoom> customRooms = new List<DungeonRoom>();
            int firstCustomRoomIndex = customRooms.Count;
            if (customRoute)
            {
                for (int c = 0; c < customRoute.transform.childCount; c++)
                {
                    GameObject customRoom = Instantiate(dungeonSettings.ReturnRandomRoom(), explorerCanvas.transform);
                    customRoom.name = $"CustomRoom_{customRooms.Count}";
                    DungeonRoom dr = customRoom.GetComponent<DungeonRoom>();
                    dr.isCustomRoom = true;
                    dr.roomIcon = customRoute.transform.GetChild(c).GetComponent<miniMapCustomIcon>();
                    if (c == 0)
                    {
                        dr.InheritRouteFromParent(parentRoom, routeTemplate);
                    }
                    else
                    {
                        dr.InheritRouteFromParent(customRooms[customRooms.Count - 1], routeTemplate);
                    }
                    GenerateRoomObject(dr, dungeonSettings.maxChestsPerRoom);
                    customRooms.Add(dr);
                    mainRooms.Add(dr);
                }
                if (customRooms.Any(o => o.name == "CustomRoom_0"))
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
            if (mainRooms.Any(o => o.isCustomRoom) && mainRooms[mainRooms.Count - 1].isCustomRoom)
            {
                return false;
            }

            int actualRoomPosition = dungeonSettings.minRooms - dungeonSettingsCopy.minRoomsBeforeCustomRoutes; //Do this because rooms are traversed backwards
            return roomCount > 0 && dungeonSettings.minRooms != (roomCount + 1) && actualRoomPosition >= roomCount && dungeonSettingsCopy.customRouteObjects.Count > 0;
        }

        bool CanLockDoor(int roomCount)
        {
            int actualLockedDoorPosition = dungeonSettings.minRooms - dungeonSettingsCopy.roomsBeforeLockedDoor; //Do this because rooms are traversed backwards
                                                                                                                 //actualLockedDoorPosition = actualLockedDoorPosition == 2 ? 3 : actualLockedDoorPosition;
            return /*roomCount > 0 &&*/ dungeonSettings.minRooms != (roomCount + 1) && roomCount >= actualLockedDoorPosition && dungeonSettingsCopy.locks.Count > 0;
        }

        void DropKeysInRoom(List<KeyItem> keys, int roomIndex)
        {
            var combinedRooms = mainRooms.Concat(detourRooms);
            combinedRooms.Reverse();
            List<DungeonRoom> allowedRooms = combinedRooms.Take(roomIndex).ToList();  //mainRooms.Take(dungeonSettingsCopy.roomsBeforeLockedDoor).ToList();
            for (int i = 0; i < keys.Count && allowedRooms[i].routes.Any(r => r.lockObj == null); i++)
            {
                DungeonRoom room = allowedRooms[MainGameManager.instance.ReturnRandom(allowedRooms.Count)];
                room.AddKey(keys[i], explorerItemTemplate);
            }
            //mainRooms.Reverse();
        }

        void AddRandomEncounters()
        {
            var attempt = 0;
            var rnd = new System.Random();
            List<DungeonRoom> randomRooms = allRooms.OrderBy(x => rnd.Next()).ToList();
            while (smallEncounters <= dungeonSettingsCopy.maxSmallEncounters && attempt != 3)
            {
                foreach (DungeonRoom room in randomRooms)
                {
                    if (dungeonSettingsCopy.enemyEncounters.Count > 0 && GameManager.GetChanceByPercentage(0.8f) && room.encounter == null && !room.isStartingRoom)
                    {
                        AddEncounter(room);
                    }
                }
                ++attempt;
            }
        }

        void AddEncounter(DungeonRoom dr)
        {
            int encounterIndex = MainGameManager.instance.ReturnRandom(dungeonSettingsCopy.enemyEncounters.Count);
            enemyEncounter ee = UnityEngine.Object.Instantiate(dungeonSettingsCopy.enemyEncounters[encounterIndex]);
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

            if (!lootAdded)
            {
                ee.loot.ForEach(o =>
                {
                    BattleManager.AddToLoot(o);
                });
                lootAdded = true;
            }
        }

        void GenerateRooms(int numberOfRooms)
        {
            // List<KeyItem> savedKeys = new List<KeyItem>();
            for (int i = 0; i < numberOfRooms; i++)
            {
                if (CanGenerateCustomRoute(i) && MainGameManager.instance.GetChanceByPercentage(dungeonSettings.chanceToGenerateCustomRoute)/*GetChance(dungeonSettings.chanceToGenerateCustomRoute)*/)
                {
                    GenerateCustomRooms(mainRooms[mainRooms.Count - 1]);
                }
                else
                {
                    GameObject room = Instantiate(dungeonSettings.ReturnRandomRoom(), explorerCanvas.transform);
                    DungeonRoom dr = room.GetComponent<DungeonRoom>();
                    for (int index = 0; index <= dungeonSettings.maxResourcePerRoom; index++)
                    {
                        if (MainGameManager.instance.GetChanceByPercentage(0.2f))
                        {
                            var randomResource = MainGameManager.instance.ReturnRandom(dungeonSettings.resources.Count);
                            dr.AddResources(dungeonSettings.resources[randomResource], fieldItemTemplate);
                        }
                    }
                    room.name = $"Room_{mainRooms.Count}";
                    dr.isStartingRoom = i == (numberOfRooms - 1);
                    dr.id = $"room_{i}";
                    if (mainRooms.Count > 0)
                    {
                        DungeonRoom parentRoom = mainRooms[mainRooms.Count - 1];
                        /*if (CanLockDoor(i) && GameManager.GetChanceByPercentage(0.3f))
                        {
                            Debug.Log($"Locked door created at {parentRoom.name}");
                            int lockIndex = ExploreManager.gameManager.ReturnRandom(dungeonSettingsCopy.locks.Count);
                            parentRoom.lockObj = UnityEngine.Object.Instantiate(dungeonSettingsCopy.locks[lockIndex]);
                            dungeonSettingsCopy.locks.RemoveAt(lockIndex);
                            savedKeys.Add(parentRoom.lockObj.key);
                        }*/
                        dr.InheritRouteFromParent(parentRoom, routeTemplate);
                    }
                    GenerateRoomObject(dr, dungeonSettings.maxChestsPerRoom);
                    mainRooms.Add(dr);
                    if (!dr.isCustomRoom)
                    {
                        GenerateRoomIcon(dr, lineDirectionEnum.left, true, i);
                    }
                }
            }
            /*if (savedKeys.Count > 0)
            {
                DropKeysInRoom(savedKeys);
            }*/
        }

        Transform GetFreeSection(DungeonRoom room, int randIntFromForeGround)
        {
            bool freeSpot = false;
            Transform result = null;
            int count = 0;
            while (!freeSpot && count < 5)
            {
                Transform section = room.foregroundHolder.transform.GetChild(randIntFromForeGround).transform;
                if (section.childCount == 0)
                {
                    result = section;
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        void PlaceRoomObject(DungeonRoom room, int position, List<GenericItem> items)
        {
            if (true)
            {
                GameObject objectT = Instantiate(objectTemplate, GetFreeSection(room, position));
                var ro = objectT.GetComponent<RoomObject>();
                items.ForEach(item =>
                {
                    if (MainGameManager.instance.GetChanceByPercentage(0.3f))
                    {
                        ro.items.Add(item);
                    }
                });
                ro.position = position;
                //ro.GenerateItems(1);
                room.roomObjects.Add(ro);
            }
        }

        void GenerateRoomObject(DungeonRoom room, int amount)
        {
            amount = amount > 3 ? 3 : amount;
            for (int i = 1; i <= amount; i++)
            {
                if (GameManager.GetChanceByPercentage(0.1f))
                {
                    int randIntFromForeGround = MainGameManager.instance.ReturnRandom(room.foregroundHolder.transform.childCount);
                    PlaceRoomObject(room, randIntFromForeGround, dungeonSettings.chestResources);
                }
            }
        }

        /// <summary>
        /// Creates room from parent room given, direction 0-1 indicates left route, 3-4 indicates a right route, anything in the middle a forward route
        /// </summary>
        /// <param name="parentRoom"></param>
        /// <param name="prefix"></param>
        /// <param name="direction"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        DungeonRoom AddRoomFromParentRoom(DungeonRoom parentRoom, string prefix, int? direction = null, string suffix = "")
        {
            GameObject room = Instantiate(dungeonSettings.ReturnRandomRoom(), explorerCanvas.transform);
            DungeonRoom dr = room.GetComponent<DungeonRoom>();
            room.name = $"{prefix}_Room_{detourRooms.Count}{suffix}";
            dr.CreateRouteFromRoom(parentRoom, routeTemplate, direction);
            if (prefix.ToLower() == "detour")
            {
                dr.parentRoom = parentRoom;
                dr.isDetour = true;
                dr.depth = parentRoom.detourRooms.Count;
            }
            else if (prefix.ToLower() == "detour_connector")
            {
                dr.parentRoom = parentRoom;
                dr.isDetour = true;
            }
            parentRoom.detourRooms.Add(dr);
            GenerateRoomObject(dr, dungeonSettings.maxChestsPerRoom);
            return dr;
        }

        void GenerateDetours(int detourLength)
        {
            var masterIndex = 0;
            mainRooms.Where(m => !m.isCustomRoom && !m.isStartingRoom).ToList().ForEach(o =>
            {
                for (int i = 1; i <= (int)dungeonSettingsCopy.maxDetours; i++)
                {
                    /*if (CanGenerateCustomRoute(i) && GetChance(dungeonSettings.chanceToGenerateCustomRoute))
                    {
                        GenerateCustomRooms(o.detourRooms[o.detourRooms.Count - 1]);
                    }
                    else*/
                    if (MainGameManager.instance.GetChanceByPercentage(0.5f) && detourLength > 0)
                    {
                        DungeonRoom r = AddRoomFromParentRoom(o, "Detour", i);
                        r.id = $"room_detour_{i}_{o.id}";
                        GenerateRoomIcon(r, i == 1 ? lineDirectionEnum.left : lineDirectionEnum.right, false, 0, masterIndex);
                        if (r.roomIcon)
                        {
                            r.roomIcon.ShowLine(i == 1 ? lineDirectionEnum.right : lineDirectionEnum.left);
                            r.roomIcon.SetDetourColour();
                        }
                        for (int x = 1; x <= detourLength; x++)
                        {
                            var depthIndex = 1;
                            if (MainGameManager.instance.GetChanceByPercentage(0.5f))
                            {
                                DungeonRoom parentRoom = r.detourRooms.Count == 0 ? r : r.detourRooms[r.detourRooms.Count - 1];
                                DungeonRoom n = AddRoomFromParentRoom(parentRoom, "Detour", i, $"_{x}");
                                GenerateRoomIcon(n, i == 1 ? lineDirectionEnum.left : lineDirectionEnum.right, false, depthIndex, masterIndex);
                                if (n.roomIcon)
                                {
                                    n.roomIcon.ShowLine(i == 1 ? lineDirectionEnum.right : lineDirectionEnum.left);
                                }
                                depthIndex++;
                                n.roomIcon.SetDetourColour();
                                detourRooms.Add(n);
                            }
                        }
                        detourRooms.Add(r);
                    }
                }
                masterIndex++;
            });
        }

        /// <summary>
        /// Adds a locked door to a mainRoom and a key in a room before a mainRoom
        /// </summary>
        void AddLockedDoors()
        {
            List<KeyItem> savedKeys = new List<KeyItem>();
            int i = 0;
            mainRooms.Reverse();
            mainRooms.ForEach(m =>
            {
                if (dungeonSettingsCopy.locks.Count == 0)
                {
                    return;
                }
                if (CanLockDoor(i) && GameManager.GetChanceByPercentage(0.3f))
                {
                    Debug.Log($"Locked door created at {m.name}");
                    int lockIndex = MainGameManager.instance.ReturnRandom(dungeonSettingsCopy.locks.Count);
                    m.lockObj = UnityEngine.Object.Instantiate(dungeonSettingsCopy.locks[lockIndex]);
                    dungeonSettingsCopy.locks.RemoveAt(lockIndex);
                    savedKeys.Add(m.lockObj.key);
                }
                i++;
            });
            mainRooms.Reverse();
            if (savedKeys.Count > 0)
            {
                DropKeysInRoom(savedKeys, i);
            }
        }

        /// <summary>
        /// Return player status for battle
        /// </summary>
        /// <returns></returns>
        public  List<ExplorerStatus> GetDungeonStatus(bool friendly)
        {
            return friendly ? dungeonSettings.playertStatuses : dungeonSettings.enemyStatuses;
        }

        void GetTotalRoomsAndHide()
        {
            var allRoomGameObjects = GameObject.FindGameObjectsWithTag("dungeonRoom").ToList();
            allRoomGameObjects.ForEach(o =>
            {
                var dr = o.GetComponent<DungeonRoom>();
                allRooms.Add(dr);
                SavedDataManager.SavedDataManagerInstance.SaveRoomsAndData(dr, dr.roomObjects);
                o.SetActive(false);
            });
            mainRooms[mainRooms.Count - 1].gameObject.SetActive(true);
            allRooms[mainRooms.Count - 1].roomIcon.SetStartIcon();
            allRooms[0].roomIcon.SetEndIcon();
        }

        public void SetCurrentHealth()
        {
            PlayerData playerData = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
            tankHealth.value = playerData.tankHealth;
            dpsHealth.value = playerData.dpsHealth;
            healerHealth.value = playerData.healerHealth;
            characterInfoDisplayController.LoadHealthInfo();
            SetMaxHealth(playerData.tankMaxHealth, RoleEnum.tank);
            SetMaxHealth(playerData.dpsMaxHealth, RoleEnum.dps);
            SetMaxHealth(playerData.healerMaxHealth, RoleEnum.healer);
        }

        public  void SetMaxHealth(float value, RoleEnum role)
        {
            switch (role)
            {
                case RoleEnum.tank:
                    tankHealth.maxValue = value;
                    break;
                case RoleEnum.healer:
                    healerHealth.maxValue = value;
                    break;
                case RoleEnum.dps:
                    dpsHealth.maxValue = value;
                    break;
                default:
                    break;
            }
        }

        public  void UpdateSliderHealth(float value, RoleEnum role)
        {
            switch (role)
            {
                case RoleEnum.tank:
                    tankHealth.value = value;
                    break;
                case RoleEnum.healer:
                    healerHealth.value = value;
                    break;
                case RoleEnum.dps:
                    dpsHealth.value = value;
                    break;
                default:
                    break;
            }
        }

        public  void AddToSliderHealth(float value, RoleEnum role)
        {
            switch (role)
            {
                case RoleEnum.tank:
                    tankHealth.value += value;
                    characterInfoDisplayController.SetHealthInfo(classType.guardian, (int)tankHealth.value);
                    break;
                case RoleEnum.healer:
                    healerHealth.value += value;
                    characterInfoDisplayController.SetHealthInfo(classType.walker, (int)healerHealth.value);
                    break;
                case RoleEnum.dps:
                    dpsHealth.value += value;
                    characterInfoDisplayController.SetHealthInfo(classType.stalker, (int)dpsHealth.value);
                    break;
                default:
                    break;
            }
            SavedDataManager.SavedDataManagerInstance.SavePlayerHealth((int)tankHealth.value, (int)dpsHealth.value, (int)healerHealth.value);
        }
    }
}
