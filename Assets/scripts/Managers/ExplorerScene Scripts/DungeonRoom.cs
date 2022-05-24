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
        }

        public void CreateRouteFromRoom(DungeonRoom room, GameObject routeTemplate)
        {
            SectionObject section = GetFreeSectionFromRoom(room.gameObject);
            GameObject route = Instantiate(routeTemplate, section.transform);
            Route r = route.GetComponent<Route>();
            r.position = section.position;
            AddRoute(this, r, "Route_Detour");
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
                ExploreManager.gameManager.TaskManager.CallTask(3f, () =>
                {
                    ExploreManager.gameManager.SceneManager.LoadBattle(encounter.enemies);
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
            var section = GetFreeSectionFromRoom(this.gameObject);
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

        SectionObject GetFreeSectionFromRoom(GameObject room = null)
        {
            GameObject parentRoom = room ? room : this.gameObject;
            bool freeSpot = false;
            SectionObject result = null;
            int count = 0;
            while (!freeSpot && count < 5)
            {
                int range = Random.Range(0, 4);
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
            while (!freeSpot && count < 5)
            {
                Transform section = room.GetComponent<DungeonRoom>().mainPanel.transform.GetChild(position);
                if (section.childCount == 0)
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

        public void AddBackRoute()
        {
            if(routes.Count > 0)
            {
                ExploreManager.mainRooms.ForEach(o =>
                {
                    if (o.gameObject.name == routes[0].location && o.routes.Count > 0)
                    {
                        o.routes[0].backwardLocation = this.gameObject.name;
                    }
                });
            }
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