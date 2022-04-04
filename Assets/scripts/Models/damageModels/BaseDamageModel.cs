using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class BaseDamageModel
    {
        public BaseDamageModel()
        {
            hitAnimNormal = "hit";
        }
        public BattleDetailsManager combatDisplayScript;
        public float damageTaken;
        public float remainingAbsorbAmount;
        public float damageAbsorbed;
        public float flatDmgTaken;
        public float MdamageTaken;
        public float incomingDmg;
        public float incomingMDmg;
        public float incomingHeal;
        public float healAmount;
        public float healAmountTaken;
        public string skillSource;
        public GameObject customHitFX;
        public Transform hitEffectPositionScript;
        public GameObject hitEffectPosition;
        public GameObject effectObject;
        public BaseCharacterManager dmgSource;
        public string hitAnimation;
        public string hitAnimNormal;
        public bool animationHold;
        //public bool useResistances;
        public string holdAnimation;
        public List<BaseCharacterManager> dueDmgTargets;
        public SkillModel skillModel;
        public enemySkill enemySkillModel;
        public BaseCharacterManagerGroup baseManager;
        public bool isMagicDmg = false;
        public bool isMiss;
        public bool damageImmidiately;
        public bool modifiedDamage;
        public bool showDmgNumber = true;
        public bool showExtraInfo = false;
        public DamageColorEnum textColor;
        public elementType element;
        public int fontSize;
    }
}