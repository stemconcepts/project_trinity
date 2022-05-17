using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class DungeonSettings : ScriptableObject
    {
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
        public List<ItemBase> spawnableItems;
        public int roomsBeforeLockedDoor;
        [Header("Enemy Settings")]
        public List<enemyEncounter> enemyEncounters;
        public int maxSmallEncounters;
    }
}