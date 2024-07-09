using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class BaseCharacterManagerGroup : MonoBehaviour
    {
        public BaseCharacterManager characterManager;
        public StatusManager statusManager;
        public Animation_Manager animationManager;
        public BaseMovementManager movementManager;
        public Effects_Manager effectsManager;
        public BaseDamageManager damageManager;
        public EquipmentManager equipmentManager;
        public BaseSkillManager skillManager;
        public EventManagerV2 EventManagerV2;
        public BaseAutoAttackManager autoAttackManager;
        public CharacterInteractionManager characterInteractionManager;
    }
}

