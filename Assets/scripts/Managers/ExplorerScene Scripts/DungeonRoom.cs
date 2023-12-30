using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;
using System;
using Random = UnityEngine.Random;

namespace AssemblyCSharp
{
    public enum ResourceSourceType
    {
        any,
        groundOnly,
        wallOnly
    }

    [Serializable]
    public class ResourcePoint {
        public GameObject resourceGameObject;
        public ResourceSourceType resourceSourceType;

    }

    public class DungeonRoom : MonoBehaviour
    {
        public string id;
        public bool isDetour, isCustomRoom, isStartingRoom;
        public LockObject lockObj;
        public bool visited;
        public GameObject mainPanel;
        public GameObject topPanel;
        public GameObject encounterHolder;
        public GameObject foregroundHolder;
        public List<RoomObject> roomObjects;
        public List<ResourcePoint> resourcePoints2;
        public DungeonRoom parentRoom;
        public List<DungeonRoom> detourRooms;
        public List<Route> routes;
        public List<string> routeLocations;
        public enemyEncounter encounter;
        public miniMapIconBase roomIcon;
        public int depth;
        [Header("Replaces Generated Routes")]
        public CustomRouteController customRoute;

        public void SetCustomRouteToLocation(string location)
        {
            if (customRoute != null)
            {
                customRoute.lockObj = lockObj != null ? UnityEngine.Object.Instantiate(lockObj) : null;
                UpdateCustomRoute(location, customRoute, "CustomRoute_Main");
                routeLocations.Add(location);
                //this.parentRoom.routeLocations.Add(miniMapController.label);
            }
        }

        public void InheritRouteFromParent(DungeonRoom parentRoom, GameObject routeTemplate)
        {
            var section = GetFreeSectionFromRoom();
            if (section != null)
            {
                GameObject route = Instantiate(routeTemplate, section.transform);
                Route r = route.GetComponent<Route>();
                r.position = section.position;
                AddRoute(parentRoom, r, "Route_Main");
                routeLocations.Add(parentRoom.gameObject.name);
                parentRoom.routeLocations.Add(this.gameObject.name);
            }
        }

        public void CreateRouteFromRoom(DungeonRoom room, GameObject routeTemplate, int? direction)
        {
            var range = direction == 1 ? new List<int> { 0, 2 } : new List<int> { 2, 4 };
            SectionObject section = GetFreeSectionFromRoom(room.gameObject, direction == null ? null : range);
            if (section != null)
            {
                GameObject route = Instantiate(routeTemplate, section.transform);
                Route r = route.GetComponent<Route>();
                r.position = section.position;
                AddRoute(this, r, "Route_Detour");
                routeLocations.Add(room.gameObject.name);
                room.routeLocations.Add(this.gameObject.name);
                //room.routes.Add(r);
            }
        }

        GameObject GetFreeResourcePoint(ResourceSourceType resourceSourceType)
        {
            var points = new List<GameObject>();
            resourcePoints2.ForEach(resourcePoint =>
            {
                if ((resourceSourceType == ResourceSourceType.any || resourcePoint.resourceSourceType == resourceSourceType) && resourcePoint.resourceGameObject.transform.childCount == 0)
                {
                    points.Add(resourcePoint.resourceGameObject);
                }
            });
            return points.Count > 0 ? points[MainGameManager.instance.ReturnRandom(points.Count)] : null;
        }

        public void InsertRouteFromData(RouteData routeData, GameObject routeTemplate)
        {
            GameObject route = Instantiate(routeTemplate, GetFreeSection(this.gameObject, routeData.position));
            Route r = route.GetComponent<Route>();
            r.name = routeData.name;
            r.location = routeData.location;
            if (routeData.lockObject)
            {
                //AddLockToolTip(this, r);
            }
            routes.Add(r);
        }

        public void CheckEncounterAndStart()
        {
            if (encounter)
            {
                MainGameManager.instance.taskManager.CallTask(1f, () =>
                {
                    MainGameManager.instance.SceneManager.LoadBattle(encounter.enemies);
                });
                MainGameManager.instance.taskManager.CallTask(3f, () =>
                {
                    Destroy(encounter.instanciatedObject);
                });
            }
        }

        public void AddEncounter(enemyEncounter encounter, GameObject encounterTemplate)
        {
            GameObject encounterObj = Instantiate(encounterTemplate, encounterHolder.transform);
            enemyEncounterController encounterController = encounterObj.GetComponent<enemyEncounterController>();
            encounter.instanciatedObject = encounterObj;
            this.encounter = encounter;
        }

        /// <summary>
        /// Add key to free area
        /// </summary>
        /// <param name="key"></param>
        /// <param name="explorerItemTemplate"></param>
        public void AddKey(KeyItem key, GameObject explorerItemTemplate)
        {
            var section = GetFreeSectionFromTopRoom(this.gameObject, new List<int> { 1, 3 });
            GameObject keyObj = Instantiate(explorerItemTemplate, section.transform);
            keyObj.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            ExplorerItemsController itemController = keyObj.GetComponent<ExplorerItemsController>();
            itemController.SetUpItem(key, this.name);
            Debug.Log($"Key spawned at {this.name}");
        }

        /// <summary>
        /// Add resource to free area
        /// </summary>
        /// <param name="resourceItem"></param>
        /// <param name="resourceTemplate"></param>
        public void AddResources(GenericItem resourceItem, GameObject resourceTemplate)
        {
            GameObject section = GetFreeResourcePoint(resourceItem.resourceSourceType);
            if (section)
            {
                GameObject resource = Instantiate(resourceTemplate, section.transform);
                ExplorerItemsController resourceController = resource.GetComponent<ExplorerItemsController>();
                resourceController.SetUpItem(resourceItem, this.name);
                Debug.Log($"Resource spawned at {this.name}");
            }
        }

        SectionObject GetFreeRouteSection()
        {
            bool freeSpot = false;
            SectionObject result = null;
            int count = 0;
            while (!freeSpot && count < 5)
            {
                int range = Random.Range(0, 4);
                Transform section = mainPanel.transform.Find($"routeHolder{range}");
                if (section.childCount == 0)
                {
                    result = new SectionObject()
                    {
                        transform = section,
                        position = range
                    };
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        /// <summary>
        /// Replaces the route locations in room being entered by removing the room location you just left from the options
        /// </summary>
        /// <param name="routeLocationToDisable"></param>
        public void SetRouteLocations(string routeLocationToDisable)
        {
            var freeRouteLocations = routeLocations.Where(o => o != routeLocationToDisable).ToList();
            var relevantRoute = routes.Where(o => o.location == routeLocationToDisable).FirstOrDefault();
            var otherRoutes = routes.Where(o => o.location != routeLocationToDisable).ToList();
            if (freeRouteLocations.Count == 1)
            {
                relevantRoute.location = freeRouteLocations[0];
            } else
            {
                relevantRoute.location = freeRouteLocations.Where(o => otherRoutes.Any(r => r.location != o)).Select(o => o).FirstOrDefault();
            }
        }

        SectionObject GetFreeSectionFromRoom(GameObject room = null, List<int> allowedRange = null)
        {
            GameObject parentRoom = room ? room : this.gameObject;
            bool freeSpot = false;
            SectionObject result = null;
            int count = 0;
            allowedRange = allowedRange == null ? new List<int> { 0, 2, 4 } : allowedRange;
            while (!freeSpot && count < 10 && allowedRange.Count > 0)
            {
                //int range = allowedRange.Count == 1 ? allowedRange[0] : Random.Range(0, allowedRange.Count);
                int range = Random.Range(0, allowedRange.Count);
                Transform section = parentRoom.GetComponent<DungeonRoom>().mainPanel.transform.Find($"routeHolder{allowedRange[range]}");
                if (allowedRange.Count == 1)
                {
                    allowedRange.RemoveAt(0);
                } else
                {
                    allowedRange.RemoveAt(range);
                }
                
                if (section.childCount == 0)
                {
                    result = new SectionObject()
                    {
                        transform = section,
                        position = range
                    };
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        SectionObject GetFreeSectionFromTopRoom(GameObject room = null, List<int> allowedRange = null)
        {
            GameObject parentRoom = room ? room : this.gameObject;
            bool freeSpot = false;
            SectionObject result = null;
            int count = 0;
            allowedRange = allowedRange == null ? new List<int> { 1, 3 } : allowedRange;
            while (!freeSpot && count < 10 && allowedRange.Count > 0)
            {
                int range = Random.Range(0, allowedRange.Count);
                Transform section = parentRoom.GetComponent<DungeonRoom>().topPanel.transform.Find($"keyareaHolder{allowedRange[range]}");
                if (allowedRange.Count == 1)
                {
                    allowedRange.RemoveAt(0);
                }
                else
                {
                    allowedRange.RemoveAt(range);
                }

                if (section.childCount == 0)
                {
                    result = new SectionObject()
                    {
                        transform = section,
                        position = range
                    };
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        Transform GetFreeSection(GameObject room, int position)
        {
            bool freeSpot = false;
            Transform result = null;
            int count = 0;
            while (!freeSpot && count < 10)
            {
                Transform section = room.GetComponent<DungeonRoom>().mainPanel.transform.GetChild(position);
                if (section && section.childCount == 0)
                {
                    result = section;
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        public void UpdateCustomRoute(string roomLocation, CustomRouteController route, string suffix)
        {
            route.routeTag = route.gameObject.name = $"{roomLocation}_{suffix}";
            route.roomLocation = roomLocation;
        }

        public void AddRoute(DungeonRoom room, Route route, string suffix)
        {
            route.routeTag = route.gameObject.name = $"{room.gameObject.name}_{suffix}";
            route.location = room.gameObject.name;
            /*if (room.lockObj)
            {
                AddLockToolTip(room, route);
            }*/
            routes.Add(route);
        }

        void AddLockToolTip(DungeonRoom room, Route route)
        {
            ToolTipTriggerController tooltip = route.gameObject.GetComponent<ToolTipTriggerController>();
            tooltip.AddtoolTip("lock", room.lockObj.lockName, room.lockObj.lockDesc);
            //tooltip.toolTipName = room.lockObj.lockName;
            //tooltip.toolTipDesc = room.lockObj.lockDesc;
            tooltip.enabled = room.lockObj.locked;
            route.lockObj = room.lockObj;
        }

        public void SetVisited()
        {
            this.visited = true;
            if(roomIcon != null) roomIcon.SetVisited();
        }
    }
}