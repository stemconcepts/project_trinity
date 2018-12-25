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
        SkeletonAnimation skeletonAnimation;
        animationControl playerAnimationControl { get; set; }
        private gameEffects gameEffectsScript { get; set; }
        private character_data characterScript { get; set; }
        private characterMovementController characterMovementScript { get; set; }
        public combatDisplay combatDisplayScript { get; set; }
        private enemySkillSelection enemySkillScript { get; set; }
        private status statusScript { get; set; }
        public float damageTaken { get; set; }
        public float MdamageTaken { get; set; }
        public float healAmountTaken { get; set; }
        private string skillSource { get; set; }
        public GameObject hitEffect { get; set; }
        public GameObject customHitFX { get; set; }
        private Transform hitEffectPositionScript { get; set; }
        public GameObject hitEffectPosition { get; set; }
        private GameObject effectObject { get; set; }
        public GameObject dmgSource { get; set; }
        public string hitAnimation { get; set; }
        public string hitAnimNormal { get; set; } = "hit";
        public bool animationHold { get; set; }
        public string holdAnimation { get; set; }
        public List<GameObject> dueDmgTargets { get; set; }
        private soundController soundContScript { get; set; }
        private classSkills classSkill { get; set; }
        private enemySkill enemySkill { get; set; }
    }
}