using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Character_Select_Manager: BasicManager
    {
        public bool guardianSelected = true;
        public bool walkerSelected = false;
        public bool stalkerSelected = false; 
        public GameObject guardianObject {get; set;}
        public GameObject stalkerObject {get; set;}
        public GameObject walkerObject {get; set;}
        public characterSelect characterSelected;
        public enum characterSelect {
            guardianSelected,
            walkerSelected,
            stalkerSelected
        }

        public Character_Select_Manager()
        {
            guardianObject = GameObject.Find("Guardian");
            walkerObject = GameObject.Find("Walker");
            stalkerObject = GameObject.Find("Stalker");
            Battle_Manager.guardian = new Battle_Manager.classState( "guardian", guardianObject.GetComponent<Character_Manager>().characterModel.isAlive, guardianSelected, false );
            Battle_Manager.stalker = new Battle_Manager.classState( "stalker", stalkerObject.GetComponent<Character_Manager>().characterModel.isAlive, stalkerSelected, false );
            Battle_Manager.walker = new Battle_Manager.classState( "walker", walkerObject.GetComponent<Character_Manager>().characterModel.isAlive, walkerSelected, true );
        }

        public void SetSelectedCharacter( string characterClass ){
            if( characterClass == "Guardian" ){
                characterSelected = characterSelect.guardianSelected;
            } else if( characterClass == "Walker" ){
                characterSelected = characterSelect.walkerSelected;
            } else if( characterClass == "Stalker" ){
                characterSelected = characterSelect.stalkerSelected;
            }
        }

        //returns what character is selected as a ?
        public string GetSelectedClassRole(){
            if( characterSelect.guardianSelected ){
                return "Guardian";
            } else if( characterSelect.walkerSelected){
                return "Walker";
            } else if( characterSelect.stalkerSelected ){
                return "Stalker";
            }
            return null;
        }
    
        public string GetSelectedClassRoleCaps(){
            if( characterSelect.guardianSelected ){
                return "Tank";
            } else if( characterSelect.walkerSelected){
                return "Healer";
            } else if( characterSelect.guardianSelected ){
                return "Dps";
            }
            return null;
        }
    
        public GameObject GetSelectedClassObject(){
            if( characterSelect.guardianSelected ){
                return guardianObject;
            } else if( characterSelect.walkerSelected){
                return walkerObject;
            } else if( characterSelect.guardianSelected ){
                return stalkerObject;
            }
            return null;
        }
    }
}

