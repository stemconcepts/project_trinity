using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Character_Select_Manager: MonoBehaviour
    {
        /*public bool guardianSelected = true;
        public bool walkerSelected = false;
        public bool stalkerSelected = false; */
        public GameObject guardianObject;
        public GameObject stalkerObject;
        public GameObject walkerObject;
        public static List<Battle_Manager.classState> classStates = new List<Battle_Manager.classState>();
        public characterSelect characterSelected;
        public enum characterSelect {
            guardianSelected,
            walkerSelected,
            stalkerSelected
        }
        public List<Character_Manager> friendlyCharacters = new List<Character_Manager>();
        public List<Character_Manager> enemyCharacters = new List<Character_Manager>();


        void Awake()
        {
            enemyCharacters = GetCharacterManagers(GameObject.FindGameObjectsWithTag("Enemy").ToList());
            enemyCharacters.Capacity = enemyCharacters.Count;
            friendlyCharacters = GetCharacterManagers(GameObject.FindGameObjectsWithTag("Player").ToList());
            friendlyCharacters.Capacity = friendlyCharacters.Count;
        }

        void Start(){
            guardianObject = GameObject.Find("Guardian");
            walkerObject = GameObject.Find("Walker");
            stalkerObject = GameObject.Find("Stalker");
            classStates.Add( new Battle_Manager.classState( "Guardian", guardianObject.GetComponent<Character_Manager>().characterModel.isAlive, characterSelected == characterSelect.guardianSelected, false ) );
            classStates.Add( new Battle_Manager.classState( "Stalker", stalkerObject.GetComponent<Character_Manager>().characterModel.isAlive, characterSelected == characterSelect.stalkerSelected, false ) );
            classStates.Add( new Battle_Manager.classState( "Walker", walkerObject.GetComponent<Character_Manager>().characterModel.isAlive, characterSelected == characterSelect.walkerSelected, true ) );
        }

        public List<Character_Manager> GetCharacterManagers( List<GameObject> go){
            var y = go.Select(o => o.GetComponent<Character_Manager>()).ToList();
            y.Capacity = y.Count;
            return y;
        }

        public void SetSelectedCharacter( string characterClass ){
            classStates.ForEach(o => o.LastSelected = false);
            classStates.Where(o => o.Name == GetSelectedClassRole()).FirstOrDefault().LastSelected = true;
            if ( characterClass == "Guardian" ){
                characterSelected = characterSelect.guardianSelected;
            } else if( characterClass == "Walker" ){
                characterSelected = characterSelect.walkerSelected;
            } else if( characterClass == "Stalker" ){
                characterSelected = characterSelect.stalkerSelected;
            }
            classStates.ForEach(o => o.Selected = false);
            classStates.Where(o => o.Name == characterClass).FirstOrDefault().Selected = true;
        }

        public string GetAlive(){
            for( int i=0; i < classStates.Count ; i++ ){
                if( classStates[i].Alive && !classStates[i].Selected && !classStates[i].LastSelected ){
                    return classStates[i].Name;
                }
            }
            return "bla";
        }

        //returns what character is selected as a ?
        public string GetSelectedClassRole(){
            if( characterSelected == characterSelect.guardianSelected ){
                return "Guardian";
            } else if( characterSelected == characterSelect.walkerSelected){
                return "Walker";
            } else if( characterSelected == characterSelect.stalkerSelected ){
                return "Stalker";
            }
            return null;
        }
    
        public string GetSelectedClassRoleCaps(){
            if( characterSelected == characterSelect.guardianSelected ){
                return "Tank";
            } else if( characterSelected == characterSelect.walkerSelected){
                return "Healer";
            } else if( characterSelected == characterSelect.stalkerSelected ){
                return "Dps";
            }
            return null;
        }
    
        public GameObject GetSelectedClassObject(){
            if( characterSelected == characterSelect.guardianSelected ){
                return guardianObject;
            } else if( characterSelected == characterSelect.walkerSelected){
                return walkerObject;
            } else if( characterSelected == characterSelect.stalkerSelected ){
                return stalkerObject;
            }
            return null;
        }
        void ToggleThroughLiveCharacters()
        {
            var swapTo = Battle_Manager.characterSelectManager.GetAlive();
            Battle_Manager.characterSelectManager.SetSelectedCharacter(swapTo);
            GetSelectedClassObject().GetComponent<Character_Interaction_Manager>().DisplaySkills();
            Battle_Manager.soundManager.playSound(Battle_Manager.soundManager.charSwapSound);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                ToggleThroughLiveCharacters();
            }
        }
    }
}

