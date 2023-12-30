using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class LockObject : ScriptableObject
    {
        public KeyItem key;
        public string lockName = "Locked";
        [Multiline]
        public string lockDesc = "Path is locked or blocked, you'll need to find another way through.";
        public bool locked;
    }
}