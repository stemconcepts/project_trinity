using UnityEngine;
using System.Collections.Generic;
using Assets.scripts.Managers.ExplorerScene_Scripts;

namespace AssemblyCSharp
{
    /*[System.Serializable]
    public class environmentStatuses
    {
        public bool affectAll;
        public List<SingleStatusModel> status = new List<SingleStatusModel>();
    }*/

    public class DungeonSettings : ScriptableObject
    {
        [Header("Custom Start and End Room")]
        public GameObject StartRoom;
        public GameObject EndRoom;
        [Header("Environment Status Settings")]
        public List<ExplorerStatus> playertStatuses = new List<ExplorerStatus>();
        public List<ExplorerStatus> enemyStatuses = new List<ExplorerStatus>();
        [Header("Custom Route Settings")]
        public List<GameObject> customRouteObjects;
        [Range(0.0f, 1.0f)]
        public float chanceToGenerateCustomRoute;
        public int minRoomsBeforeCustomRoutes;
        [Header("Detour Settings")]
        public detourAmount MaxDetourPerMainRoute;
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
        [Range(0.0f, 1.0f)]
        public float chanceToGenerateResources;
        public int maxResourcePerRoom;
        public List<GenericItem> resources;
        [Range(0.0f, 1.0f)]
        public float chanceToGenerateChests;
        public int maxChestsPerRoom;
        public List<GenericItem> chestResources;
        /*[Header("Exploration Ambience Sound")]
        public AudioClip ambientSounds;
        public AudioClip backgroundExplorerMusic;*/

        public GameObject ReturnRandomRoom()
        {
            int roomIndex = MainGameManager.instance.ReturnRandom(roomTemplates.Count);
            return roomTemplates[roomIndex];
        }
    }
}