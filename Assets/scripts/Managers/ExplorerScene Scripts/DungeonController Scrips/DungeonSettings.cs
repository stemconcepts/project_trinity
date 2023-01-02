using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class environmentStatuses
    {
        public bool affectAll;
        public List<SingleStatusModel> status = new List<SingleStatusModel>();
    }

    public class DungeonSettings : ScriptableObject
    {
        [Header("Environment Status Settings")]
        public List<environmentStatuses> environmentStatuses = new List<environmentStatuses>();
        public List<environmentStatuses> enemyEnvironmentStatuses = new List<environmentStatuses>();
        [Header("Custom Route Settings")]
        public List<GameObject> customRouteObjects;
        [Range(0.0f, 1.0f)]
        public float chanceToGenerateCustomRoute;
        public int minRoomsBeforeCustomRoutes;
        [Header("Detour Settings")]
        public detourAmount maxDetours;
        public enum detourAmount
        {
            One = 1,
            Two = 2
        };
        public int maxDetourLength;
        [Header("Map Settings")]
        public int minRooms;
        public Difficulty difficulty;
        public enum Difficulty
        {
            Normal,
            Hard,
            Elite
        }
        public List<LockObject> locks;
        //public List<ItemBase> spawnableItems;
        public int roomsBeforeLockedDoor;
        [Header("Enemy Settings")]
        public List<enemyEncounter> enemyEncounters;
        public int maxSmallEncounters;
        [Header("Templates")]
        public List<GameObject> roomTemplates;
        [Header("Resources")]
        public int maxResourcePerRoom;
        public List<GenericItem> resources;

        public GameObject ReturnRandomRoom()
        {
            int roomIndex = MainGameManager.instance.ReturnRandom(roomTemplates.Count);
            return roomTemplates[roomIndex];
        }
    }
}