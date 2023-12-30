using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class miniMapCustomIcon : miniMapIconBase
    {
        public mapPositionEnum mapPosition;
        public bool mainPathOnly;
        public enum mapPositionEnum
        {
            none,
            start,
            end
        }
        public GameObject customRoom;
        public miniMapCustomIcon nextRoomIcon;
    }
}