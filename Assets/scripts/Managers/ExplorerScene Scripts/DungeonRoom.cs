using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class DungeonRoom : MonoBehaviour
    {
        public string id;
        public bool isDetour, isCustomRoom, isStartingRoom;
        public LockObject lockObj;
        public bool visited;
        public GameObject mainPanel;
        public GameObject encounterHolder;
        public GameObject foregroundHolder;
        public List<RoomObject> roomObjects;
        public DungeonRoom parentRoom;
        public List<DungeonRoom> detourRooms;
        public List<Route> routes;
        public List<string> routeLocations;
        public enemyEncounter encounter;
        public miniMapIconBase roomIcon;
        public int depth;

        public void InheritRouteFromParent(DungeonRoom parentRoom, GameObject routeTemplate)
        {
            var section = GetFreeSectionFromRoom();
            GameObject route = Instantiate(routeTemplate, section.transform);
            Route r = route.GetComponent<Route>();
            r.position = section.position;
            AddRoute(parentRoom, r, "Route_Main");
            routeLocations.Add(parentRoom.gameObject.name);
            parentRoom.routeLocations.Add(this.gameObject.name);
            //parentRoom.routes.Add(r);
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
                ExploreManager.gameManager.TaskManager.CallTask(1f, () =>
                {
                    //ExploreManager.gameManager.SceneManager.LoadBattle(encounter.enemies);
                    MainGameManager.instance.SceneManager.LoadBattle(encounter.enemies);
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

        public void AddKey(KeyItem key, GameObject explorerItemTemplate)
        {
            var section = GetFreeSectionFromRoom(this.gameObject, new List<int> { 0, 4 });
            GameObject keyObj = Instantiate(explorerItemTemplate, section.transform);
            keyObj.name = key.name;
            ExplorerItemsController itemController = keyObj.GetComponent<ExplorerItemsController>();
            itemController.position = section.position;
            key.id = $"key_{this.name}_{key.itemName}";
            itemController.itemBase = key;
            itemController.SetUpItem();
            Debug.Log($"Key spawned at {this.name}");
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
            while (!freeSpot && count < 10)
            {
                int range = allowedRange != null && allowedRange.Count > 0 ? Random.Range(allowedRange[0], allowedRange[1]) : Random.Range(1, 3);
                Transform section = parentRoom.GetComponent<DungeonRoom>().mainPanel.transform.Find($"routeHolder{range}");
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

        public void AddRoute(DungeonRoom room, Route route, string suffix)
        {
            route.routeTag = route.gameObject.name = $"{room.gameObject.name}_{suffix}";
            route.location = room.gameObject.name;
            if (room.lockObj)
            {
                AddLockToolTip(room, route);
            }
            routes.Add(route);
        }

        void AddLockToolTip(DungeonRoom room, Route route)
        {
            ToolTipTriggerController tooltip = route.gameObject.GetComponent<ToolTipTriggerController>();
            tooltip.toolTipName = room.lockObj.lockName;
            tooltip.toolTipDesc = room.lockObj.lockDesc;
            tooltip.enabled = room.lockObj.locked;
            route.lockObj = room.lockObj;
        }

        public void SetVisited()
        {
            this.visited = true;
            roomIcon.SetVisited();
        }
    }
}