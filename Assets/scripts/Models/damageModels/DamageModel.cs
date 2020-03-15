using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class DamageModel
    {
        public DamageModel()
        {
            hitAnimNormal = "hit";
        }
        public Battle_Details_Manager combatDisplayScript;
        private Skill_Manager SkillScript;
        private Status_Manager statusScript;
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
        public Character_Manager dmgSource;
        public string hitAnimation;
        public string hitAnimNormal;
        public bool animationHold;
        //public bool useResistances;
        public string holdAnimation;
        public List<Character_Manager> dueDmgTargets;
        public SkillModel skillModel;
        public enemySkill enemySkillModel;
        public Base_Character_Manager baseManager;
        public bool isMagicDmg = false;
        public bool damageImmidiately;
        public bool modifiedDamage;
        public elementType element;
        public int fontSize;
    }
}
