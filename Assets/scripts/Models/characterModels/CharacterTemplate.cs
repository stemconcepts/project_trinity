﻿using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class CharacterTemplate : ScriptableObject
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
        public EnemyCharacterModel characterModel;
        [Header("Resistances")]
        public Resistances resistances;
    }
}
