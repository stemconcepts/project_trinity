using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class CharacterModel : BaseCharacterModel
    {
        public void SetUp()
        {
            attackedPos = new Vector2();
            Health = Health == 0 ? fullHealth : Health;
            maxHealth = fullHealth;
            isAlive = true;
            originalthornsDmg = 0;
            originalPDef = PDef;
            originalMDef = MDef;
            originalPAtk = PAtk;
            originalMAtk = MAtk;
            originalMDef = MDef;
            originalHaste = Haste;
            originalAccuracy = Accuracy;
            originalCritChance = critChance;
            originalEvasion = evasion;
        }

        public Sprite characterIcon;
        public bool inVoidCounter;
        public bool inThreatZone;
        public StatusModel deathStatus;
    }
}

