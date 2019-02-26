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

        public Character_Select_Manager()
        {
            guardianObject = GameObject.Find("Guardian");
            walkerObject = GameObject.Find("Walker");
            stalkerObject = GameObject.Find("Stalker");
            Battle_Manager.guardian = new Battle_Manager.classState( "guardian", guardianObject.GetComponent<character_Manager>().characterModel.isAlive, guardianSelected, false );
            Battle_Manager.stalker = new Battle_Manager.classState( "stalker", stalkerObject.GetComponent<character_Manager>().characterModel.isAlive, stalkerSelected, false );
            Battle_Manager.walker = new Battle_Manager.classState( "walker", walkerObject.GetComponent<character_Manager>().characterModel.isAlive, walkerSelected, true );
        }

        //returns what character is selected as a ?
        public string GetSelectedClassRole(){
            if( guardianSelected ){
                return "Guardian";
            } else if( walkerSelected){
                return "Walker";
            } else if( stalkerObject ){
                return "Stalker";
            }
            return null;
        }
    
        public string GetSelectedClassRoleCaps(){
            if( guardianSelected ){
                return "Tank";
            } else if( walkerSelected ){
                return "Healer";
            } else if( stalkerObject ){
                return "Dps";
            }
            return null;
        }
    
        public GameObject GetSelectedClassObject(){
            if( guardianSelected ){
                return GameObject.Find("Guardian");
            } else if( walkerSelected){
                return GameObject.Find("Walker");
            } else if( stalkerObject ){
                return GameObject.Find("Stalker");
            }
            return null;
        }
    }
}

