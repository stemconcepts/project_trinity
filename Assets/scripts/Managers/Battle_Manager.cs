using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Battle_Manager
    {
        public static Event_Manager eventManager;
        BattleModel battleModel;
        public Game_Manager gameManager;
        public static Sound_Manager soundManager;
        public static Game_Effects_Manager gameEffectManager;
        public static Task_Manager taskManager;
        public static Battle_Details_Manager battleDetailsManager;
        public static List<Battle_Interface_Manager> battleInterfaceManager  = new List<Battle_Interface_Manager>();
        public static Character_Select_Manager characterSelectManager;
        public static List<Character_Manager> friendlyCharacters = new List<Character_Manager>();
        public static List<Character_Manager> enemyCharacters = new List<Character_Manager>();
        //public static List<classState> classStates {get; set;}
        //public static classState guardian {get; set;} 
        //public static classState walker {get; set;}
        // static classState stalker {get; set;}
        //public static List<classState> charClass {get; set;}

        /*public static Battle_Manager MakeBattleManager( GameObject gm ) {
            GameObject x = new GameObject("BMInstance");
            Battle_Manager bm = x.AddComponent<Battle_Manager>();
            bm.gameManager = gm.GetComponent<Game_Manager>();
            return bm;

        }*/

        public Battle_Manager( Game_Manager gm )
        {
            gameManager = gm;
            taskManager = new Task_Manager();
            battleDetailsManager = new Battle_Details_Manager();
            characterSelectManager = new Character_Select_Manager();
            /*taskManager = gameObject.AddComponent(typeof(taskManager)) as Task_Manager;
            battleDetailsManager = gameObject.AddComponent(typeof(Battle_Details_Manager)) as Battle_Details_Manager;
            characterSelectManager = gameObject.AddComponent(typeof(Character_Select_Manager)) as Character_Select_Manager;*/
            GameObject[] bi = GameObject.FindGameObjectsWithTag("skillDisplayControl");
            battleInterfaceManager = bi.Select( x => x.GetComponent<Battle_Interface_Manager>() ).ToList();
            soundManager = gameManager.SoundManager;
            gameEffectManager = gameManager.GameEffectsManager;
            /*charClass.Add( guardian );
            charClass.Add( walker );
            charClass.Add( stalker );*/
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

        public void LoadCharacters( List<Character_Manager> characters ){
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