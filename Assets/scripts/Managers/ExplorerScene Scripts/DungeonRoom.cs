using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class DungeonRoom : MonoBehaviour
    {
        public bool isLocked;
        public int currentRoutes;
        public List<Route> routes;

        public void InheritRouteFromParent(DungeonRoom parentRoom)
        {
            routes.Add(
                new Route()
                {
                    routeTag = $"{parentRoom.gameObject.tag}_Route_Main",
                    location = parentRoom.gameObject.tag,
                    locked = parentRoom.isLocked
                }

            );
        }
    }
}