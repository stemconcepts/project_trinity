using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class enemyEncounter : ScriptableObject
    {
        public bool spawnOnce;
        public List<GameObject> enemies;
    }
}