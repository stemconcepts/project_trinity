using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Battle_Manager : BasicManager
    {
        public static Event_Manager eventManager {get; set;}
        BattleModel battleModel {get; set;}
        public Game_Manager gameManager {get; set;}
        public static Sound_Manager soundManager { get; set; }
        public static Game_Effects_Manager gameEffectManager {get; set;}
        public static Task_Manager taskManager {get; set;}
        public static Battle_Details_Manager battleDetailsManager { get; set; }
        public static Character_Select_Manager characterSelectManager { get; set; }
        public static List<character_Manager> friendlyCharacters { get; set; } 
        public static List<character_Manager> enemyCharacters { get; set; } 
        public static classState guardian {get; set;} 
        public static classState walker {get; set;}
        public static classState stalker {get; set;}
        public static List<classState> charClass {get; set;}

        public Battle_Manager()
        {
            taskManager = new Task_Manager();
            battleDetailsManager = new Battle_Details_Manager();
            characterSelectManager = new Character_Select_Manager();
            soundManager = gameManager.SoundManager;
            gameEffectManager = gameManager.GameEffectsManager;
            charClass.Add( guardian );
            charClass.Add( walker );
            charClass.Add( stalker );
        }

        public class classState{
            public string Name; //{ get; set; } 
            public bool Alive; //{ get; set; } 
            public bool Selected;
            public bool LastSelected;
            public classState( string name, bool alive, bool selected, bool lastSelected ){
                Name = name;
                Alive = alive;
                Selected = selected;
                LastSelected = lastSelected;
            } 
        }

        public void LoadArea(){

        }
        
        public void LoadStatBonuses(){
            
        }

        public string GetAlive( classState[] charClass ){
            for( int i=0; i < charClass.Length ; i++ ){
                if( charClass[i].Alive && !charClass[i].Selected && !charClass[i].LastSelected ){
                    return charClass[i].Name;
                }
            }
            return "bla";
        }

        public void LoadCharacters( List<character_Manager> characters ){
            foreach (var character in characters)
            {
                character.characterModel.currentPanel.GetComponent<Panels_Manager>().SetStartingPanel( character.gameObject );
            }
        }

        public void LoadSkillDisplay(){
            
        }

        public void StartBattle( float waitTime){
            LoadCharacters(friendlyCharacters);
            LoadCharacters(enemyCharacters);
            taskManager.CallTask( waitTime );
        }
    }
}