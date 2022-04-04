using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class EnemyCharacterManagerGroup : BaseCharacterManagerGroup
    {
        //public Base_Character_Manager<Enemy_Character_Model, Enemy_Character_Manager_Group> characterManager;
        //public Enemy_Character_Manager characterManager;
        //public EnemyAutoAttackManager autoAttackManager;

        public PhaseManager phaseManager;

        void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            this.characterManager = this.gameObject.GetComponent<EnemyCharacterManager>();
            this.statusManager = this.gameObject.GetComponent<StatusManager>();
            this.animationManager = this.gameObject.GetComponent<Animation_Manager>();
            this.movementManager = this.gameObject.GetComponent<BaseMovementManager>();
            this.effectsManager = this.gameObject.GetComponent<Effects_Manager>();
            this.damageManager = this.gameObject.GetComponent<BaseDamageManager>();
            this.characterInteractionManager = this.gameObject.GetComponent<CharacterInteractionManager>();
            this.skillManager = this.gameObject.GetComponent<EnemySkillManager>();
            this.autoAttackManager = this.gameObject.GetComponent<EnemyAutoAttackManager>();
            this.phaseManager = this.gameObject.GetComponent<PhaseManager>();
        }
    }
}