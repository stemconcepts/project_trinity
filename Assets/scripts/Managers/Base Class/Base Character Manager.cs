using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public abstract class Base_Character_Manager : MonoBehaviour
    {
        public Character_Manager characterManager { get; set;} /*{ 
                this.gameObject.GetComponent<character_Manager>();
        }}*/
        public Status_Manager statusManager {get; set;} /*{
                this.gameObject.GetComponent<Status_Manager>();
        }}*/
        public Animation_Manager animationManager {get; set;} /*{
                this.gameObject.GetComponent<Status_Manager>();
        }}*/
        public Movement_Manager movementManager {get; set;}
        public Effects_Manager effectsManager {get; set;}
        public Damage_Manager damageManager {get; set;}
        public Skill_Manager skillManager {get; set;}
        public Character_Interaction_Manager characterInteractionManager {get; set;}
        public Base_Character_Manager()
        {
            this.characterManager = this.gameObject.GetComponent<Character_Manager>();
            this.statusManager = this.gameObject.GetComponent<Status_Manager>();
            this.animationManager = this.gameObject.GetComponent<Status_Manager>();
            this.movementManager = this.gameObject.GetComponent<Movement_Manager>();
            this.effectsManager = this.gameObject.GetComponent<Effects_Manager>();
            this.damageManager = this.gameObject.GetComponent<Damage_Manager>();
            this.characterInteractionManager = this.gameObject.GetComponent<Character_Interaction_Manager>();
            this.skillManager = this.gameObject.GetComponent<Skill_Manager>();
        }
    }
}

