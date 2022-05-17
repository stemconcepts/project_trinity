using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class enemyEncounter : ScriptableObject
    {
        public string id;
        public bool spawnOnce;
        public List<ItemBase> loot;
        public bool lootAdded;
        public List<GameObject> enemies;
        public GameObject instanciatedObject;
    }
}