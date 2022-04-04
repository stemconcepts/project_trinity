using UnityEngine;
using UnityEditor;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Resistances
    {
        [Range(-1, 1)]
        public float poison;
        [Range(-1, 1)]
        public float physical;
        [Range(-1, 1)]
        public float fire;
        [Range(-1, 1)]
        public float water;
        [Range(-1, 1)]
        public float shadow;
        [Range(-1, 1)]
        public float wind;
        [Range(-1, 1)]
        public float earth;
        [Range(-1, 1)]
        public float light;
    }
}