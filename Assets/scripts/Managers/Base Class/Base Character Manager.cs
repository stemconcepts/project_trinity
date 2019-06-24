using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public abstract class Base_Character_Manager : MonoBehaviour
    {
        public Character_Manager characterManager { get; set;}
        public Status_Manager statusManager {get; set;}
        public Animation_Manager animationManager {get; set;}
        public Movement_Manager movementManager {get; set;}
        public Effects_Manager effectsManager {get; set;}
        public Damage_Manager damageManager {get; set;}
        public Equipment_Manager equipmentManager {get; set;}
        public Skill_Manager skillManager {get; set;}
        public Auto_Attack_Manager autoAttackManager { get; set;}
        public Character_Interaction_Manager characterInteractionManager {get; set;}
        void Awake()
        {
            this.characterManager = this.gameObject.GetComponent<Character_Manager>();
            this.statusManager = this.gameObject.GetComponent<Status_Manager>();
            this.animationManager = this.gameObject.GetComponent<Animation_Manager>();
            this.movementManager = this.gameObject.GetComponent<Movement_Manager>();
            this.effectsManager = this.gameObject.GetComponent<Effects_Manager>();
            this.damageManager = this.gameObject.GetComponent<Damage_Manager>();
            this.characterInteractionManager = this.gameObject.GetComponent<Character_Interaction_Manager>();
            this.skillManager = this.gameObject.GetComponent<Skill_Manager>();
            equipmentManager = this.gameObject.GetComponent<Equipment_Manager>();
            autoAttackManager = this.gameObject.GetComponent<Auto_Attack_Manager>();
        }
    }
}

