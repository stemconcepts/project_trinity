using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Character_Template : ScriptableObject
    {
        [Header("Creature Type")]
        public creatureType type;
        public enum creatureType
        {
            Neutral,
            Undead,
            Fire,
            Ice,
            Nature,
            Air
        };
        [Header("Stats")]
        public Character_Model characterModel;
        [Header("Resistances")]
        public Resistances resistances;
    }
}
