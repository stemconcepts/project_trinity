﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public enum PowerLevelEnum
    {
        Weak,
        Normal,
        Strong
    };

    [System.Serializable]
    public class EnemyCharacterModel : BaseCharacterModel
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
        public int experience;
        public List<ItemBase> loot;
        public PowerLevelEnum powerLevel = PowerLevelEnum.Normal;
    }
}