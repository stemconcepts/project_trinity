using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using UnityEngine.UI;
using AssemblyCSharp;
using static AssemblyCSharp.miniMapIconBase;
using Assets.scripts.Helpers.Utility;

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
        public int MaxBagSize;
        private bool useBackwardRoute;
        public bool lootAdded;
        public GameManager gameManager;
        public string currentRoom;
        public GameObject backButton;
        public GameObject inventoryHolder;
        [HideInInspector]
        private corruptionController corruptionHolder;
        private int stepCounter;
        public DungeonSettings dungeonSettings;
        DungeonSettings dungeonSettingsCopy;

        public List<DungeonRoom> roomsInOrder = new List<DungeonRoom>(); //Does not currently include starting room and custom rooms

        public List<DungeonRoom> allRooms = new List<DungeonRoom>();
        public List<DungeonRoom> mainRooms = new List<DungeonRoom>();
        public List<DungeonRoom> detourRooms = new List<DungeonRoom>();

        public List<miniMapIconBase> iconControllers = new List<miniMapIconBase>();
        //public Dictionary<string, miniMapIconBase> IconControllers = new Dictionary<string, miniMapIconBase>();

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
            GenerateRooms(dungeonSettings.minRooms);
            GenerateDetours(dungeonSettingsCopy.maxDetourLength);
            //LinkDetours();
            AddLockedDoors();
            GetTotalRoomsAndHide();
            AddRandomEncounters();
            AddCurroptionToRoutes();
            MainGameManager.instance.DisableEnableLiveBoxColliders(false);
            SetCurrentRoom(mainRooms[mainRooms.Count - 1].gameObject.name);
            //SavedDataManager.SavedDataManagerInstance.SaveIconPos(iconControllers);
            
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
            List<detourLink> detourLinks = new List<detourLink>();
            iconControllers.ForEach(o =>
            {
                var match = iconControllers.Find(i => i.depth == o.depth && i.lineDirection == o.lineDirection && !i.isMainRoute && !i.isCustomRoute && i.label != o.label);
                if (match && !detourLinks.Any(d => d.end == match) /*&& !x.Any(d => d.start == match)*/)
                {
                    var item = new detourLink()
                    {
                        start = match,
                        end = o
                    };

                    item.distance = Math.Abs(item.start.masterDepth - item.end.masterDepth) - 1;
                    detourLinks.Add(item);
                }
            });

            detourLinks.Reverse();
            detourLinks.ForEach(o =>
            {
                DungeonRoom lastRoom = null;

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
                       // GenerateRoomIcon(r, false, o.start.depth, o.start.masterDepth + 1);

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
            //SavedDataManager.SavedDataManagerInstance.SaveCurrentRoomData(roomName);
        }

        /*GameObject AddCustomRoomIcon(GameObject roomIconGroupTemplate, miniMapIconBase startIconController)
        {
            GameObject roomIconGroup = Instantiate(roomIconGroupTemplate, miniMap.transform);
            Transform startingIconLocation = startIconController.transform;

            for (int i = 0; i < roomIconGroup.transform.childCount; i++)
            {
                miniMapCustomIcon mmc = roomIconGroup.transform.GetChild(i).gameObject.GetComponent<miniMapCustomIcon>();
                mmc.gameObject.name += $"_{iconControllers.Count}";
                mmc.isCustomRoute = true;
                iconControllers.Add(mmc);
                if (mmc.mapPosition == miniMapCustomIcon.mapPositionEnum.start)
                {
                    mmc.ChooseCustomRouteDirection(startingIconLocation, roomIconGroup.transform);
                }
            }

            return roomIconGroup;
        }*/

        GameObject AddCustomRouteIcon(GameObject roomIconGroupTemplate, DungeonRoom parentRoom)
        {
            GameObject roomIconGroup = Instantiate(roomIconGroupTemplate, miniMap.transform);
            Transform startingIconLocation = parentRoom.roomIcon.transform;

            var miniMapControls = roomIconGroup.transform.GetComponentsInChildren<miniMapCustomIcon>();
            var direction = lineDirectionEnum.down;
            foreach (var miniMapIconController in miniMapControls)
            {
                miniMapIconController.gameObject.name += $"_{iconControllers.Count}";
                miniMapIconController.isCustomRoute = true;
                iconControllers.Add(miniMapIconController);
                if (miniMapIconController.mapPosition == miniMapCustomIcon.mapPositionEnum.start)
                {
                    var newRoomResult = CanGenerateRoom(parentRoom, miniMapControls);
                    //Do this if no connection could be made
                    if (newRoomResult.Item3 != null)
                    {
                        Destroy(roomIconGroup);
                        return null;
                        //continue;
                    }
                    miniMapIconController.LocationId = newRoomResult.Item1;
                    direction = newRoomResult.Item2;
                }
            }
            miniMapControls.Where(icon => icon.mapPosition == miniMapCustomIcon.mapPositionEnum.start)
                .FirstOrDefault()
                .ChooseCustomRouteDirection(startingIconLocation, direction);

            return roomIconGroup;
        }

        string CreateLocationId(lineDirectionEnum finalDirection, (string alphabet, int index) parentLocationId)
        {
            var index = 0;
            var alphabet = "A";
            switch (finalDirection)
            {
                case lineDirectionEnum.down:
                    index = iconControllers.Count == 0 ? 0 : parentLocationId.index;
                    alphabet = iconControllers.Count == 0 ? "A" : LabelConverter.GetLetterFromCalculation(parentLocationId.alphabet, 1);
                    break;
                case lineDirectionEnum.right:
                    index = parentLocationId.index - 1;
                    alphabet = parentLocationId.alphabet;
                    break;
                case lineDirectionEnum.left:
                    index = parentLocationId.index + 1;
                    alphabet = parentLocationId.alphabet;
                    break;
                default:
                    break;
            }

            //System.Diagnostics.Debug.WriteLine($"{alphabet}{index}");

            return $"{alphabet}{index}";
        }

        void GenerateRoomIcon(DungeonRoom dr, string locationId = null, lineDirectionEnum direction = lineDirectionEnum.none)
        {
            GameObject roomIcon = Instantiate(roomIconTemplate, miniMap.transform);
            miniMapIconController mmc = roomIcon.GetComponent<miniMapIconController>();
            mmc.label = dr.gameObject.name;

            var finalDirection = lineDirectionEnum.down;

            (string alphabet, int index) parentLocationId = ("A", 0);

            if (dr.isDetour)
            {
                roomIcon.name += $"_Detour_{iconControllers.Count}";
                miniMapIconBase parentIcon = iconControllers.Where(o => o.label == dr.parentRoom.gameObject.name).FirstOrDefault();
                Transform parentRoomTransform = parentIcon.transform;
                parentLocationId = parentIcon.GetLocationId();
                finalDirection = mmc.ChooseDirection(parentRoomTransform, direction);
            } 
            else if (iconControllers.Count > 0)
            {
                roomIcon.name += $"_{iconControllers.Count}";
                var iconController = iconControllers[iconControllers.Count - 1];
                Transform trueLocation = roomIconPosition != null ? roomIconPosition : iconController.transform;
                parentLocationId = trueLocation.gameObject.GetComponent<miniMapIconBase>().GetLocationId();
                finalDirection = mmc.ChooseDirection(trueLocation, direction);
                roomIconPosition = roomIconPosition != null ? null : roomIconPosition;
            }
            
            mmc.LocationId = locationId != null ? locationId : CreateLocationId(finalDirection, parentLocationId);

            dr.roomIcon = mmc;
            iconControllers.Add(mmc);
        }

        bool CanAddEncounter(enemyEncounter ee)
        {
            return dungeonSettingsCopy.enemyEncounters.Count > 0 && !ee.spawnOnce && dungeonSettingsCopy.maxSmallEncounters > smallEncounters;
        }

        /// <summary>
        /// Add custom route to main path, note.. route is generated backwards.
        /// </summary>
        /// <param name="parentRoom"></param>
        /*void GenerateCustomRooms(DungeonRoom parentRoom)
        {
            int randomIndex = MainGameManager.instance.ReturnRandom(dungeonSettingsCopy.customRouteObjects.Count);
            GameObject customRoute = dungeonSettingsCopy.customRouteObjects[randomIndex];
            List<DungeonRoom> customRooms = new List<DungeonRoom>();

            void GenerateLocationId(DungeonRoom room, miniMapCustomIcon controller)
            {
                var isNotFirstRoom = customRooms.Count > 0;
                var previousCustomRoom = isNotFirstRoom ? customRooms[customRooms.Count - 1] : parentRoom;
                var parentLocationId = previousCustomRoom.roomIcon.GetLocationId();
                var lineDirection = previousCustomRoom.roomIcon.lineDirection;
                room.roomIcon.LocationId = CreateLocationId(lineDirection, parentLocationId);
                room.id = isNotFirstRoom ? $"custom_room_connector_{customRooms.Count}_{previousCustomRoom.id}" : $"custom_room_connector_0";

                if (controller.nextRoomIcon && room.customRoute)
                {
                    room.SetCustomRouteToLocation(controller.nextRoomIcon.label);
                }
            }

            void LinkUnlinkedCustomRooms()
            {
                for (int index = 1; index < customRooms.Count - 1; index++)
                {
                    var nextCustomRoom = customRooms[index];
                    customRooms[index - 1].InheritRouteFromParent(nextCustomRoom, routeTemplate);
                }
            }

            if (customRoute)
            {
                //Link custom rooms together first
                var miniMapControls = customRoute.transform.GetComponentsInChildren<miniMapCustomIcon>().Reverse();
                foreach (var controller in miniMapControls)
                {
                    GameObject customRoom;
                    //Creates or inserts the room for this location
                    if (controller.customRoom != null)
                    {
                        customRoom = Instantiate(controller.customRoom, explorerCanvas.transform);
                    }
                    else
                    {
                        customRoom = Instantiate(dungeonSettings.ReturnRandomRoom(), explorerCanvas.transform);
                    }
                    customRoom.name = $"CustomRoom_{customRooms.Count}_{customRoute.name}";

                    //Needed to let the route know where to go
                    controller.label = customRoom.name;

                    DungeonRoom dr = customRoom.GetComponent<DungeonRoom>();
                    dr.roomIcon = controller;
                    dr.isCustomRoom = true;

                    GenerateLocationId(dr, controller);
                    GenerateRoomObject(dr, dungeonSettings.maxChestsPerRoom);

                    customRooms.Add(dr);
                    mainRooms.Add(dr);
                }

                LinkUnlinkedCustomRooms();

                //Attach start point to main route
                var startRoom = customRooms.FirstOrDefault(o => ((miniMapCustomIcon)o.GetComponent<DungeonRoom>().roomIcon).mapPosition == miniMapCustomIcon.mapPositionEnum.start);
                var endRoom = customRooms.FirstOrDefault(o => ((miniMapCustomIcon)o.GetComponent<DungeonRoom>().roomIcon).mapPosition == miniMapCustomIcon.mapPositionEnum.end);

                if (startRoom != null)
                {
                    startRoom.InheritRouteFromParent(parentRoom, routeTemplate);
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
        }*/

        void GenerateCustomRooms(DungeonRoom parentRoom, CustomRouteSetting setting)
        {
            int randomIndex = MainGameManager.instance.ReturnRandom(setting.customRouteObjects.Count);
            GameObject customRoute = setting.customRouteObjects[randomIndex];
            List<DungeonRoom> customRooms = new List<DungeonRoom>();

            void GenerateChildLocationIds(DungeonRoom room, int index)
            {
                var controller = room.roomIcon as miniMapCustomIcon;
                var isFirstRoom = index == 0;

                if (!isFirstRoom)
                {
                    var previousCustomRoom = customRooms[index - 1];
                    var parentLocationId = previousCustomRoom.roomIcon.GetLocationId();
                    var lineDirection = controller.lineDirection;
                    controller.LocationId = CreateLocationId(lineDirection, parentLocationId);
                    room.id = isFirstRoom ? $"custom_room_connector_{customRooms.Count}_{previousCustomRoom.id}" : $"custom_room_connector_0";

                    controller.ChooseDirection(previousCustomRoom.roomIcon.transform, lineDirection);

                    if (controller.nextRoomIcon && room.customRoute)
                    {
                        room.SetCustomRouteToLocation(controller.nextRoomIcon.label);
                    } else
                    {
                        previousCustomRoom.InheritRouteFromParent(room, routeTemplate);
                    }
                }
            }

            void LinkUnlinkedCustomRooms()
            {
                for (int index = 1; index < customRooms.Count - 1; index++)
                {
                    var nextCustomRoom = customRooms[index];
                    customRooms[index - 1].InheritRouteFromParent(nextCustomRoom, routeTemplate);
                }
            }

            if (customRoute)
            {
                //Link custom rooms together first
                var instantiatedRoute = AddCustomRouteIcon(customRoute, parentRoom);

                //leave method if no connection was made for the customRoute
                if (!instantiatedRoute)
                {
                    return;
                }

                var miniMapControls = instantiatedRoute.transform.GetComponentsInChildren<miniMapCustomIcon>().Reverse();
                foreach (var controller in miniMapControls)
                {
                    GameObject customRoom;
                    //Creates or inserts the room for this location
                    if (controller.customRoom != null)
                    {
                        customRoom = Instantiate(controller.customRoom, explorerCanvas.transform);
                    }
                    else
                    {
                        customRoom = Instantiate(dungeonSettings.ReturnRandomRoom(), explorerCanvas.transform);
                    }
                    customRoom.name = $"CustomRoom_{customRooms.Count}_{customRoute.name}";

                    //Needed to let the route know where to go
                    controller.label = customRoom.name;

                    DungeonRoom dr = customRoom.GetComponent<DungeonRoom>();
                    dr.roomIcon = controller;
                    dr.isCustomRoom = true;

                    customRooms.Add(dr);
                    if ((dr.roomIcon as miniMapCustomIcon).mainPathOnly)
                    {
                        mainRooms.Add(dr);
                    }
                }

                var i = 0;
                foreach(var customRoom in customRooms)
                {
                    DungeonRoom dr = customRoom.GetComponent<DungeonRoom>();
                    GenerateChildLocationIds(dr, i);
                    GenerateRoomObject(dr, dungeonSettings.maxChestsPerRoom);
                    i++;
                }

                LinkUnlinkedCustomRooms();

                //Attach start point to main route
                var startRoom = customRooms.FirstOrDefault(o => ((miniMapCustomIcon)o.GetComponent<DungeonRoom>().roomIcon).mapPosition == miniMapCustomIcon.mapPositionEnum.start);
                var endRoom = customRooms.FirstOrDefault(o => ((miniMapCustomIcon)o.GetComponent<DungeonRoom>().roomIcon).mapPosition == miniMapCustomIcon.mapPositionEnum.end);

                if (startRoom != null)
                {
                    startRoom.InheritRouteFromParent(parentRoom, routeTemplate);
                }
            }
            //AddCustomRoomIcon(customRoute, iconControllers[iconControllers.Count - 1]);
            setting.customRouteObjects.RemoveAt(randomIndex);
        }

        bool CanGenerateCustomRoute(int roomIndex, CustomRouteSetting setting)
        {
            /*if (mainRooms.Any(o => o.isCustomRoom) && mainRooms[mainRooms.Count - 1].isCustomRoom)
            {
                return false;
            }*/

            if (mainRooms.Count == 0)
            {
                return false;
            }

            //var isParentRoomValid = !mainRooms[roomIndex - 1].isCustomRoom;
            var isParentRoomValid = !mainRooms[mainRooms.Count - 1].isCustomRoom;

            return roomIndex > setting.minRoomsBeforeCustomRoutes && isParentRoomValid;

            //var hasCustomRooms = setting.customRouteObjects.Count > 0;
            //int actualRoomPosition = dungeonSettings.minRooms - setting.minRoomsBeforeCustomRoutes; //Do this because rooms are traversed backwards
            //return roomCount > 0 && dungeonSettings.minRooms != (roomCount + 1) && actualRoomPosition >= roomCount; //&& setting.customRouteObjects.Count > 0;
        }

        bool CanLockDoor(int roomCount)
        {
            int actualLockedDoorPosition = dungeonSettings.minRooms - dungeonSettingsCopy.roomsBeforeLockedDoor; //Do this because rooms are traversed backwards
            return /*roomCount > 0 &&*/ dungeonSettings.minRooms != (roomCount + 1) && roomCount >= actualLockedDoorPosition && dungeonSettingsCopy.locks.Count > 0;
        }

        void DropKeysInRoom(List<KeyItem> keys, int roomIndex)
        {
            //Saved id of blocked paths
            var blockedRooms = new List<string>();
            var lockedRoom = roomsInOrder.Where(room => room.lockObj != null)
                .FirstOrDefault();
            if (lockedRoom != null)
            {
                blockedRooms = lockedRoom.detourRooms.Select(room => room.id).ToList();
                blockedRooms.Add(lockedRoom.id);
            };

            //Gets rooms to place key in excluding blocked paths
            List<DungeonRoom> allowedRooms = roomsInOrder
                .Where(room => !blockedRooms.Contains(room.id))
                .Take(Math.Abs(roomsInOrder.Count() - roomIndex))
                .ToList();

            keys.ForEach(key =>
            {
                var randomIndex = UnityEngine.Random.Range(0, allowedRooms.Count);
                DungeonRoom room = allowedRooms[randomIndex];
                room.AddKey(key, explorerItemTemplate);
            });
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
            enemyEncounter ee = Instantiate(dungeonSettingsCopy.enemyEncounters[encounterIndex]);
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

        private GameObject ReturnRoomTemplate(int roomNumber)
        {
            if (roomNumber == 0 && dungeonSettings.EndRoom != null)
            {
                return dungeonSettings.EndRoom;
            } else if (roomNumber == (dungeonSettings.minRooms - 1))
            {
                return dungeonSettings.StartRoom;
            } else
            {
                return dungeonSettings.ReturnRandomRoom();
            }
        }

        void GenerateRooms(int numberOfRooms)
        {
            for (int i = 0; i < numberOfRooms; i++)
            {
                var endRoom = i == 0;
                var startRoom = i == (numberOfRooms - 1);

                var settingsWithcustomObjects = dungeonSettings.customRouteSettings.Where(setting => setting.customRouteObjects.Count > 0).ToList();
                int randomIndex = MainGameManager.instance.ReturnRandom(settingsWithcustomObjects.Count);
                CustomRouteSetting customRouteSetting = settingsWithcustomObjects.Count > 0 ? settingsWithcustomObjects[randomIndex] : null;

                if (customRouteSetting != null && CanGenerateCustomRoute(i, customRouteSetting) && MainGameManager.instance.GetChanceByPercentage(dungeonSettings.chanceToGenerateCustomRoute))
                {
                    //Filter out the rooms that can be used to make the main path
                    var validRooms = mainRooms.Where(room => {
                        return !room.isCustomRoom || ((miniMapCustomIcon)room.roomIcon).mainPathOnly == customRouteSetting.MainPathOnly;
                    }).ToList();

                    GenerateCustomRooms(validRooms[validRooms.Count - 1], customRouteSetting);
                }
                else
                {
                    GameObject roomGameObject = Instantiate(ReturnRoomTemplate(i), explorerCanvas.transform);
                    DungeonRoom dungeonRoom = roomGameObject.GetComponent<DungeonRoom>();

                    for (int index = 0; index <= dungeonSettings.maxResourcePerRoom; index++)
                    {
                        //Chance to load resource
                        if ((!startRoom && !endRoom) && MainGameManager.instance.GetChanceByPercentage(dungeonSettings.chanceToGenerateResources) && dungeonSettings.resources.Count > 0)
                        {
                            var randomResource = MainGameManager.instance.ReturnRandom(dungeonSettings.resources.Count);
                            dungeonRoom.AddResources(dungeonSettings.resources[randomResource], fieldItemTemplate);
                        }
                    }
                    roomGameObject.name = $"Room_{mainRooms.Count}" + (startRoom ? "_StartRoom" : endRoom ? "_EndRoom" : "");
                    dungeonRoom.isStartingRoom = startRoom;
                    dungeonRoom.id = $"room_{i}" + (startRoom ? "_StartRoom" : endRoom ? "_EndRoom" : "");

                    //Filter out the rooms that can be used to make the main path
                    var validRooms = mainRooms.Where(room => {
                        return !room.isCustomRoom || ((miniMapCustomIcon)room.roomIcon).mainPathOnly;
                    }).ToList();

                    if (!endRoom)
                    {
                        //Get the last valid room to connect to
                        DungeonRoom parentRoom = validRooms[validRooms.Count - 1];

                        //If there is a Starting room set link to its customroute
                        if (startRoom && dungeonSettings.StartRoom != null)
                        {
                            dungeonRoom.SetCustomRouteToLocation(parentRoom.gameObject.name);
                        } else
                        {
                            dungeonRoom.InheritRouteFromParent(parentRoom, routeTemplate);
                        }

                        var newRoomResult = CanGenerateRoom(parentRoom);
                        if (newRoomResult.Item3 != null)
                        {
                            continue;
                        }
                        GenerateRoomIcon(dungeonRoom, newRoomResult.Item1, newRoomResult.Item2);
                    } else
                    {
                        GenerateRoomIcon(dungeonRoom);
                    }

                    mainRooms.Add(dungeonRoom);

                    //Generate random room objects to interact with
                    if (!startRoom && !endRoom)
                    {
                        GenerateRoomObject(dungeonRoom, dungeonSettings.maxChestsPerRoom);
                    }
                }
            }
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
            //var possiblePositions = new List<int>() { 1, 3 };
            for (int i = 1; i <= amount; i++)
            {
                if (GameManager.GetChanceByPercentage(dungeonSettings.chanceToGenerateChests))
                {
                    int randIntFromForeGround = MainGameManager.instance.ReturnRandom(room.foregroundHolder.transform.childCount);
                    //int range = UnityEngine.Random.Range(0, possiblePositions.Count);
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

        lineDirectionEnum GetPossibleDirection(miniMapIconBase parentMiniMapIcon)
        {
            var direction = (lineDirectionEnum)MainGameManager.instance.ReturnRandom(3);

            // Check Direction isnt taken
            if (parentMiniMapIcon != null)
            {
                if (parentMiniMapIcon is miniMapCustomIcon)
                {
                    direction = (parentMiniMapIcon as miniMapCustomIcon).CorrectDirectionCustom(direction);
                } else
                {
                    direction = parentMiniMapIcon.CorrectDirection(direction);
                }
            }

            return direction;
        }

        (string LocationId, lineDirectionEnum Direction, DungeonRoom existingRoom) CanGenerateRoom(DungeonRoom parentRoom, miniMapCustomIcon[] miniMapIconControllers = null)
        {
            var connectionCreated = false;

            string newLocationId = null;
            var newDirection = lineDirectionEnum.none;
            DungeonRoom existingRoom = null;

            var loop = 0;
            while (!connectionCreated && loop <= iconControllers.Count)
            {
                var randomDirection = GetPossibleDirection(parentRoom.roomIcon);

                var parentlocationId = parentRoom.roomIcon.GetLocationId();
                /*var parentlocationId = (
                    alphabet: parentRoom.roomIcon.LocationId.Substring(0, 1),
                    index: parentRoom.roomIcon.LocationId.Length > 2 ? int.Parse(parentRoom.roomIcon.LocationId.Substring(1, 2)) : int.Parse(parentRoom.roomIcon.LocationId.Substring(1, 1))
                    );*/

                (string alphabet, int index) ConvertLocationIdToCode(string locationId){
                    return (
                    alphabet: locationId.Substring(0, 1),
                    index: locationId.Length > 2 ? int.Parse(locationId.Substring(1, 2)) : int.Parse(locationId.Substring(1, 1))
                    );
                }

                var possiblelocationIds = new[] { CreateLocationId(randomDirection, parentlocationId) };

                if (miniMapIconControllers != null && miniMapIconControllers.Length > 0)
                {
                    (string alphabet, int index) lastLocationId = (alphabet:"", index:0);
                    possiblelocationIds = miniMapIconControllers.Reverse().Select((icon, i) => {

                        var relevantLocationId = i > 0 ? lastLocationId : parentlocationId;
                        var direction = i > 0 ? icon.lineDirection : randomDirection;
                        var newLocationId = CreateLocationId(direction, relevantLocationId);
                        lastLocationId = ConvertLocationIdToCode(newLocationId);

                        return newLocationId;
                    }).ToArray();
                }

                if (!iconControllers.Any(icon => possiblelocationIds.Contains(icon.LocationId)))
                {
                    connectionCreated = true;
                    newDirection = randomDirection;
                    newLocationId = possiblelocationIds[0];
                    existingRoom = null;
                }
                else
                {
                    Debug.LogWarning($"Failed to get free locationId : {possiblelocationIds}");
                    var rooms = mainRooms.Concat(detourRooms);
                    var existingIcon = iconControllers.Find(icon => possiblelocationIds.Contains(icon.LocationId));
                    var relevantRoom = rooms.FirstOrDefault(room => room.gameObject.name == existingIcon.label);
                    newDirection = randomDirection;
                    existingRoom = relevantRoom;
                    possiblelocationIds = null;
                }
                loop++;
            }

            return (newLocationId, newDirection, existingRoom);
        }

        void GenerateDetours(int detourLength)
        {
            var masterIndex = 0;
            var filteredRooms = mainRooms.Where(room => {
                if (room.roomIcon is miniMapCustomIcon)
                {
                    if ((room.roomIcon as miniMapCustomIcon).mainPathOnly)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            });
            filteredRooms.Where(mainRoom => !mainRoom.isStartingRoom).ToList().ForEach(room =>
            {
                //Used to add ordering to room
                roomsInOrder.Add(room);

                for (int i = 1; i <= (int)dungeonSettingsCopy.MaxDetourPerMainRoute; i++)
                {
                    //Stops detours from being added to last room
                    if (masterIndex > 0)
                    {
                        /*if (CanGenerateCustomRoute(i) && GetChance(dungeonSettings.chanceToGenerateCustomRoute))
                        {
                            GenerateCustomRooms(o.detourRooms[o.detourRooms.Count - 1]);
                        }
                        else*/

                        //Generates initial starting point for detour from main route
                        if (MainGameManager.instance.GetChanceByPercentage(0.5f) && detourLength > 0)
                        {
                            //Logic to check that room can be created, will skip if a locationId already exists
                            var newRoomResult = CanGenerateRoom(room);
                            if (newRoomResult.Item3 != null || newRoomResult.Item1 == null)
                            {
                                continue;
                            } 

                            DungeonRoom detourRoom = AddRoomFromParentRoom(room, "Detour", i);
                            detourRoom.id = $"room_detour_{i}_{room.id}";

                            GenerateRoomIcon(detourRoom, newRoomResult.LocationId, newRoomResult.Direction);
                            /*if (detourRoom.roomIcon)
                            {
                                detourRoom.roomIcon.ShowLine(i == 1 ? lineDirectionEnum.right : lineDirectionEnum.left);
                                detourRoom.roomIcon.SetDetourColour();
                            }*/
                            detourRoom.roomIcon.SetDetourColour();

                            //Used to add ordering to room
                            roomsInOrder.Add(detourRoom);

                            //Generates extra detour rooms for detour path
                            for (int x = 1; x <= detourLength; x++)
                            {
                                var depthIndex = 1;
                                if (MainGameManager.instance.GetChanceByPercentage(0.5f))
                                {
                                    DungeonRoom parentDetourRoom = detourRoom.detourRooms.Count == 0 ? detourRoom : detourRoom.detourRooms[detourRoom.detourRooms.Count - 1];

                                    var newDetourRoomResult = CanGenerateRoom(parentDetourRoom);
                                    if (newDetourRoomResult.Item3 != null) 
                                    {
                                       continue;
                                    }

                                    DungeonRoom detourRoomOfDetour = AddRoomFromParentRoom(parentDetourRoom, "Detour", i, $"_{x}");

                                    GenerateRoomIcon(detourRoomOfDetour, newDetourRoomResult.LocationId, newDetourRoomResult.Direction);
                                    /*if (detourRoomOfDetour.roomIcon)
                                    {
                                        detourRoomOfDetour.roomIcon.ShowLine(i == 1 ? lineDirectionEnum.right : lineDirectionEnum.left);
                                    }*/

                                    depthIndex++;
                                    detourRoomOfDetour.roomIcon.SetDetourColour();

                                    //Used to add ordering to room
                                    roomsInOrder.Add(detourRoomOfDetour);

                                    detourRooms.Add(detourRoomOfDetour);
                                }
                            }

                            detourRooms.Add(detourRoom);
                        }
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
            mainRooms.ForEach(mainRooom =>
            {
                if (dungeonSettingsCopy.locks.Count == 0)
                {
                    return;
                }
                
                if (CanLockDoor(i) && mainRooom.routes.Any(route => route.routeTag.StartsWith("Room_")) /*&& GameManager.GetChanceByPercentage(0.3f)*/)
                {
                    Debug.Log($"Locked door created at {mainRooom.name}");
                    int lockIndex = MainGameManager.instance.ReturnRandom(dungeonSettingsCopy.locks.Count);
                    mainRooom.lockObj = UnityEngine.Object.Instantiate(dungeonSettingsCopy.locks[lockIndex]);

                    var route = mainRooom.routes
                        .Where(route => route.routeTag.StartsWith("Room_"))
                        .FirstOrDefault();

                    //Lock all routes that lead to this room
                    route.lockObj = mainRooom.lockObj;
                    var relevantDetourRooms = detourRooms
                        .Where(detour => detour.parentRoom.name.ToLower() == mainRooom.name.ToLower())
                        .ToList();
                    relevantDetourRooms.ForEach(room =>
                    {
                        room.routes.Where(route => route.location.ToLower() == mainRooom.name.ToLower())
                            .ToList()
                            .ForEach(route => route.lockObj = mainRooom.lockObj);
                    });

                    dungeonSettingsCopy.locks.RemoveAt(lockIndex);
                    savedKeys.Add(mainRooom.lockObj.key);
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
