using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Button_Click_Manager : BasicManager
    {
        public GameObject globalScript { get; set; }
        private Task holdTimeTask { get; set; }
        private character_select characterSelectScript { get; set; }
        private character_data characterScript { get; set; }
        private skill_cd skillCdScript;
        private Task overlapUITimer;
        private soundController soundContScript;

        public selectionOverlapControl selectionOverlapScript {get; set;}
        public Character_Interactions characterInteractions {get; set;}
        public character_Manager characterManager {get; set;}

        public Button_Click_Manager()
        {
            selectionOverlapScript = new selectionOverlapControl();
            characterInteractions = new Character_Interactions();
            characterInteractions.buttonClickManager = this;
            characterManager = this.gameObject.GetComponent<character_Manager>();
            characterInteractions.characterManager = characterManager;
        }

        public string GetClassRole(){
            return characterManager.characterModel.role;
        }
            
        public bool ConvertRoleToBool( string role, string classSelected ){
            var selectedRole = role + "Selected";
            if( selectedRole == classSelected ){
                return true;
            } else {
                return false;
            }
        }
    }
}

