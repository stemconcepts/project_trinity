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
        public static List<BattleManager.classState> classStates = new List<BattleManager.classState>();
        public characterSelect characterSelected;
        public enum characterSelect {
            guardianSelected,
            walkerSelected,
            stalkerSelected
        }
        public List<CharacterManager> friendlyCharacters = new List<CharacterManager>();
        public List<EnemyCharacterManager> enemyCharacters = new List<EnemyCharacterManager>();


        void Awake()
        {
            guardianObject = GameObject.Find("Guardian");
            walkerObject = GameObject.Find("Walker");
            stalkerObject = GameObject.Find("Stalker");
            /*enemyCharacters = GetCharacterManagers(GameObject.FindGameObjectsWithTag("Enemy").ToList());
            enemyCharacters.Capacity = enemyCharacters.Count;
            friendlyCharacters = GetCharacterManagers(GameObject.FindGameObjectsWithTag("Player").ToList());
            friendlyCharacters.Capacity = friendlyCharacters.Count;*/
        }

        void Start(){
            UpdateCharacters();
            
            classStates.Add( new BattleManager.classState( "Guardian", guardianObject.GetComponent<CharacterManager>().characterModel.isAlive, characterSelected == characterSelect.guardianSelected, false ) );
            classStates.Add( new BattleManager.classState( "Stalker", stalkerObject.GetComponent<CharacterManager>().characterModel.isAlive, characterSelected == characterSelect.stalkerSelected, false ) );
            classStates.Add( new BattleManager.classState( "Walker", walkerObject.GetComponent<CharacterManager>().characterModel.isAlive, characterSelected == characterSelect.walkerSelected, true ) );
        }

        public List<T> GetCharacterManagers<T>(List<GameObject> go){
            var y = go.Select(o => o.GetComponent<T>()).ToList();
            //var y = go.Select(o => o.GetComponent<T>()).Where(c => c.characterModel.isAlive).ToList();
            y.Capacity = y.Count;
            return y;
        }

        public void SelectCharacterWithTurnsLeft()
        {
            var selectedChar = GetSelectedClassObject().GetComponent<BaseCharacterManagerGroup>();
            if (((PlayerSkillManager)selectedChar.skillManager).turnsTaken >= selectedChar.characterManager.characterModel.Haste)
            {
                friendlyCharacters.ForEach(o =>
                {
                    if (((PlayerSkillManager)o.baseManager.skillManager).turnsTaken < o.characterModel.Haste)
                    {
                        SetSelectedCharacter(o.gameObject.name);
                        return;
                    }
                });
            }
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
            var selectedChar = GetSelectedClassObject().GetComponent<CharacterInteractionManager>();
            selectedChar.DisplaySkills();
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
            var swapTo = BattleManager.characterSelectManager.GetAlive();
            BattleManager.characterSelectManager.SetSelectedCharacter(swapTo);
            GetSelectedClassObject().GetComponent<CharacterInteractionManager>().DisplaySkills();
            BattleManager.soundManager.playSound(BattleManager.soundManager.charSwapSound);
        }

        public void UpdateCharacters(string deadCharacterName = null)
        {
            /*var x = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            var y = GameObject.FindGameObjectsWithTag("Player").ToList();
            enemyCharacters = x.Select(o => o.GetComponent<EnemyCharacterManager>()).ToList();
            friendlyCharacters = y.Select(o => o.GetComponent<CharacterManager>()).ToList();*/
            enemyCharacters = GetCharacterManagers<EnemyCharacterManager>(GameObject.FindGameObjectsWithTag("Enemy").ToList()).Where(o => o.characterModel.isAlive).ToList();
            enemyCharacters.Capacity = enemyCharacters.Count;
            friendlyCharacters = GetCharacterManagers<CharacterManager>(GameObject.FindGameObjectsWithTag("Player").ToList()).Where(o => o.characterModel.isAlive).ToList();
            friendlyCharacters.Capacity = friendlyCharacters.Count;

            if (deadCharacterName != null)
            {
                enemyCharacters.ForEach(o => o.baseManager.damageManager.autoAttackDmgModels.Remove(deadCharacterName));
                friendlyCharacters.ForEach(o => o.baseManager.damageManager.autoAttackDmgModels.Remove(deadCharacterName));
            }
        }

        void Update()
        {
            SelectCharacterWithTurnsLeft();
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                ToggleThroughLiveCharacters();
            }
        }
    }
}

