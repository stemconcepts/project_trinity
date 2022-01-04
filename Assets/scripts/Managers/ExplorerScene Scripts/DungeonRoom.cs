using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class DungeonRoom : MonoBehaviour
    {
        public bool isDetour, isCustomRoom, isStartingRoom;
        public LockObject lockObj;
        public GameObject mainPanel;
        public GameObject encounterHolder;
        public GameObject foregroundHolder;
        public DungeonRoom parentRoom;
        public List<DungeonRoom> detourRooms;
        public List<Route> routes;
        public enemyEncounter encounter;
        public miniMapIconBase roomIcon;

        public void InheritRouteFromParent(DungeonRoom parentRoom, GameObject routeTemplate)
        {
            GameObject route = Instantiate(routeTemplate, GetFreeRouteSection());
            Route r = route.GetComponent<Route>();
            AddRoute(parentRoom, r, "Route_Main");
        }

        public void CreateRouteFromRoom(DungeonRoom room, GameObject routeTemplate)
        {
            GameObject route = Instantiate(routeTemplate, GetFreeSection(room.gameObject));
            Route r = route.GetComponent<Route>();
            AddRoute(this, r, "Route_Detour");
        }

        public void CheckEncounterAndStart()
        {
            if (encounter)
            {
                Explore_Manager.gameManager.TaskManager.CallTask(3f, () =>
                {
                    Explore_Manager.gameManager.SceneManager.LoadBattle();
                });
            }
        }

        public void AddEncounter(enemyEncounter encounter, GameObject encounterTemplate)
        {
            GameObject encounterObj = Instantiate(encounterTemplate, encounterHolder.transform);
            enemyEncounterController encounterController = encounterObj.GetComponent<enemyEncounterController>();
            this.encounter = encounter;
        }

        public void AddKey(KeyItem key, GameObject explorerItemTemplate)
        {
            GameObject keyObj = Instantiate(explorerItemTemplate, GetFreeSection(this.gameObject));
            keyObj.name = key.name;
            ExplorerItemsController itemController = keyObj.GetComponent<ExplorerItemsController>();
            itemController.itemBase = key;
            itemController.SetUpItem();
            Debug.Log($"Key spawned at {this.name}");
        }

        Transform GetFreeRouteSection()
        {
            bool freeSpot = false;
            Transform result = null;
            int count = 0;
            while (!freeSpot && count < 5)
            {
                Transform section = mainPanel.transform.Find($"routeHolder{Random.Range(0, 4)}");
                if (section.childCount == 0)
                {
                    result = section;
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        Transform GetFreeSection(GameObject room)
        {
            bool freeSpot = false;
            Transform result = null;
            int count = 0;
            while (!freeSpot && count < 5)
            {
                Transform section = room.GetComponent<DungeonRoom>().mainPanel.transform.Find($"routeHolder{Random.Range(0, 4)}");
                if (section.childCount == 0)
                {
                    result = section;
                    freeSpot = true;
                };
                count++;
            }
            return result;
        }

        void AddRoute(DungeonRoom room, Route route, string suffix)
        {
            route.routeTag = route.gameObject.name = $"{room.gameObject.name}_{suffix}";
            route.location = room.gameObject.name;
            if (room.lockObj)
            {
                ToolTipTriggerController tooltip = route.gameObject.GetComponent<ToolTipTriggerController>();
                tooltip.toolTipName = room.lockObj.lockName;
                tooltip.toolTipDesc = room.lockObj.lockDesc;
                tooltip.enabled = room.lockObj.locked;
                route.lockObj = room.lockObj;
            }
            routes.Add(route);
        }
    }
}