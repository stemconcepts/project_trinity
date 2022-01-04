using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class LockObject : ScriptableObject
    {
        public KeyItem key;
        public string lockName = "Locked";
        public string lockDesc = "Path is locked or blocked, you'll need to find another way through.";
        public bool locked;
    }
}