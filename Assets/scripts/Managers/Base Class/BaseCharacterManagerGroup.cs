using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class BaseCharacterManagerGroup : MonoBehaviour
        //where TCharModel : Base_Character_Model where TManagerGrp : Base_Character_Manager_Group<TCharModel, TManagerGrp>
    {
        public BaseCharacterManager characterManager;
        public StatusManager statusManager;
        public Animation_Manager animationManager;
        public BaseMovementManager movementManager;
        public Effects_Manager effectsManager;
        public BaseDamageManager damageManager;
        public EquipmentManager equipmentManager;
        public BaseSkillManager skillManager;
        public BaseAutoAttackManager autoAttackManager;
        //public Auto_Attack_Manager<TCharModel, TManagerGrp, Base_Character_Model, Base_Character_Manager_Group<TCharModel, TManagerGrp>> autoAttackManager;
        public CharacterInteractionManager characterInteractionManager;
    }
}

