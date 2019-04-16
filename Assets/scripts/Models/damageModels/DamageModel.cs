using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class DamageModel : MonoBehaviour
    {
        [SpineAnimation]
        public SkeletonAnimation skeletonAnimation;
        public Animation_Manager playerAnimationManager { get; set; }
        private gameEffects gameEffectsScript { get; set; }
        public Character_Manager characterManager { get; set; }
        private characterMovementController characterMovementScript { get; set; }
        public combatDisplay combatDisplayScript { get; set; }
        private enemySkillSelection enemySkillScript { get; set; }
        private status statusScript { get; set; }
        public float damageTaken { get; set; }
        public float flatDmgTaken { get; set; }
        public float MdamageTaken { get; set; }
        public float incomingDmg { get; set; }
        public float incomingMDmg { get; set; }
        public float incomingHeal { get; set; }
        public float healAmount { get; set; }
        public float healAmountTaken { get; set; }
        public string skillSource { get; set; }
        public GameObject hitEffect { get; set; }
        public GameObject customHitFX { get; set; }
        public Transform hitEffectPositionScript { get; set; }
        public GameObject hitEffectPosition { get; set; }
        public GameObject effectObject { get; set; }
        public GameObject dmgSource { get; set; }
        public string hitAnimation { get; set; }
        public string hitAnimNormal { get; set; } = "hit";
        public bool animationHold { get; set; }
        public bool trueDmg { get; set; }
        public string holdAnimation { get; set; }
        public List<GameObject> dueDmgTargets { get; set; }
        private soundController soundContScript { get; set; }
        public classSkills classSkill { get; set; }
        public enemySkill enemySkill { get; set; }
    }
}
