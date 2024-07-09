using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class CharacterManagerGroup : BaseCharacterManagerGroup
    {
        //public Base_Character_Manager<Character_Model, Character_Manager_Group> characterManager;
        //public Character_Manager characterManager;
        //public Auto_Attack_Manager autoAttackManager;

        void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            this.EventManagerV2 = new EventManagerV2();
            this.characterManager = this.gameObject.GetComponent<CharacterManager>();
            this.statusManager = this.gameObject.GetComponent<StatusManager>();
            this.animationManager = this.gameObject.GetComponent<Animation_Manager>();
            this.movementManager = this.gameObject.GetComponent<PlayerMovementManager>();
            this.effectsManager = this.gameObject.GetComponent<Effects_Manager>();
            this.damageManager = this.gameObject.GetComponent<PlayerDamageManager>();
            this.characterInteractionManager = this.gameObject.GetComponent<CharacterInteractionManager>();
            this.equipmentManager = this.gameObject.GetComponent<EquipmentManager>();
            this.skillManager = this.gameObject.GetComponent<PlayerSkillManager>();
            autoAttackManager = this.gameObject.GetComponent<PlayerAutoAttackManager>();
        }
    }
}
