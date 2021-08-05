using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Explore_Manager : MonoBehaviour
    {
        //public int maxAdditionalRoutes;
        public int minRooms;
        public int maxDetours;
        public int maxDetourLength;
        public List<DungeonRoom> rooms;
        public GameObject roomTemplate;
        public Difficulty difficulty;
        public int rand;
        public enum Difficulty
        {
            Normal,
            Hard,
            Elite
        }

        void Start()
        {
            Invoke("LevelGenerator", 0.1f);
        }

        void GenerateRooms(int numberOfRooms)
        {
            for (int i = 0; i < minRooms; i++)
            {
                GameObject room = Instantiate(roomTemplate);
                DungeonRoom dr = room.GetComponent<DungeonRoom>();
                room.tag = $"Room_{rooms.Count}";
                if (rooms.Count > 1)
                {
                    dr.InheritRouteFromParent(rooms[rooms.Count - 1]);
                }
                rooms.Add(dr);
            }
        }

        void GenerateDetours(int detourLength)
        {
            rooms.ForEach(o =>
            {
                for (int i = 0; i < maxDetours; i++)
                {
                    if (GetChance(1))
                    {
                        DungeonRoom r = AddRoom(o, "Detour");
                        for (int x = 0; x < detourLength; x++)
                        {
                            if (GetChance(1))
                            {
                                AddRoom(r, "Detour");
                            }
                        }
                    }
                }
            });
        }

        bool GetChance(int maxChance)
        {
            rand = UnityEngine.Random.Range(0, maxChance);
            return rand == 1;
        }

        DungeonRoom AddRoom(DungeonRoom parentRoom, string suffix)
        {
            GameObject room = Instantiate(roomTemplate);
            DungeonRoom dr = room.GetComponent<DungeonRoom>();
            room.tag = $"Room_{rooms.Count}_{suffix}";
            Route route = new Route();
            route.UpdateRouteDetails($"{parentRoom.gameObject.tag}_Route_{suffix}", parentRoom.gameObject.tag, parentRoom.isLocked);

            dr.routes.Add(route);
            parentRoom.routes.Add(route);
            ++dr.currentRoutes;
            ++parentRoom.currentRoutes;

            rooms.Add(dr);

            return dr;
        }

        public void LevelGenerator()
        {
            GenerateRooms(minRooms);
            GenerateDetours(maxDetourLength);
        }
    }
}

