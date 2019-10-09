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
        [SpineAnimation]
        public SkeletonAnimation skeletonAnimation;
        public Animation_Manager playerAnimationManager;
        public Character_Manager characterManager;
        public Battle_Details_Manager combatDisplayScript;
        private Skill_Manager SkillScript;
        private Status_Manager statusScript;
        public float damageTaken;
        public float absorbAmount;
        public float flatDmgTaken;
        public float MdamageTaken;
        public float incomingDmg;
        public float incomingMDmg;
        public float incomingHeal;
        public float healAmount;
        public float healAmountTaken;
        public string skillSource;
        public GameObject hitEffect;
        public GameObject customHitFX;
        public Transform hitEffectPositionScript;
        public GameObject hitEffectPosition;
        public GameObject effectObject;
        public GameObject dmgSource;
        public string hitAnimation;
        public string hitAnimNormal;
        public bool animationHold;
        public bool trueDmg;
        public string holdAnimation;
        public List<GameObject> dueDmgTargets;
        //private soundController soundContScript;
        public SkillModel skillModel;
        //public SkillModel enemySkill;
    }
}
