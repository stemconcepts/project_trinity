using UnityEngine;
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
            originalinsight = insight;
            originalvigor = vigor;
        }
        public int experience;
        public List<ItemBase> loot;
        public PowerLevelEnum powerLevel = PowerLevelEnum.Normal;
    }
}