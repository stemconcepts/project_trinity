using UnityEngine;
using UnityEditor;

namespace AssemblyCSharp
{
    public class Route
    {
        public string routeTag;
        public string location;
        public bool locked;
        //public bool isRightPath;
        //public bool isLeftPath;

        public void UpdateRouteDetails(string tag, string location, bool locked)
        {
            routeTag = tag;
            this.location = location;
            this.locked = locked;
        }
    }
}