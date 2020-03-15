using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Base_Character_Manager : MonoBehaviour
    {
        public Character_Manager characterManager;
        public Status_Manager statusManager;
        public Animation_Manager animationManager;
        public Movement_Manager movementManager;
        public Effects_Manager effectsManager;
        public Damage_Manager damageManager;
        public Equipment_Manager equipmentManager;
        public Skill_Manager skillManager;
        public Auto_Attack_Manager autoAttackManager;
        public Character_Interaction_Manager characterInteractionManager;
        public PhaseManager phaseManager;
        void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            this.characterManager = this.characterManager ?? this.gameObject.GetComponent<Character_Manager>();
            this.statusManager = this.gameObject.GetComponent<Status_Manager>();
            this.animationManager = this.gameObject.GetComponent<Animation_Manager>();
            this.movementManager = this.gameObject.GetComponent<Movement_Manager>();
            this.effectsManager = this.gameObject.GetComponent<Effects_Manager>();
            this.damageManager = this.gameObject.GetComponent<Damage_Manager>();
            this.characterInteractionManager = this.gameObject.GetComponent<Character_Interaction_Manager>();
            this.skillManager = this.gameObject.GetComponent<Skill_Manager>();
            equipmentManager = this.gameObject.GetComponent<Equipment_Manager>();
            autoAttackManager = this.gameObject.GetComponent<Auto_Attack_Manager>();
            if (this.gameObject.tag == "Enemy")
            {
                phaseManager = this.gameObject.GetComponent<PhaseManager>();
            }
        }
    }
}

