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
            maxHealth = Health;
            full_health = Health;
            isAlive = true;
            originalthornsDmg = 0;
            originalPDef = PDef;
            originalMDef = MDef;
            originalPAtk = PAtk;
            originalMAtk = MAtk;
            originalMDef = MDef;
            originalHaste = Haste;
            originalAccuracy = accuracy;
            originalCritChance = critChance;
            originalEvasion = evasion;
        }

        public Sprite characterIcon;
        public bool inVoidCounter;
        public bool inThreatZone;
        public StatusModel deathStatus;
    }
}

