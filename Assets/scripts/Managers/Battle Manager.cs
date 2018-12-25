using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Battle_Manager : BasicManager
    {
        public static EventManager eventManager {get; set;}
        BattleModel battleModel {get; set;}
        Game_Manager gameManager {get; set;}
        public static Task_Manager taskManager {get; set;}

        public static List<character_Manager> friendlyCharacters {get; set;} 
        public static List<character_Manager> enemyCharacters {get; set;} 

        public Battle_Manager()
        {
            gameManager = new Game_Manager();
            taskManager = new Task_Manager();
            battleDetailsManager = new Battle_Details_Manager();
        }

        public void LoadArea(){

        }
        
        public void LoadStatBonuses(){
            
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