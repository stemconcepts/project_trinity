using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Game_Manager : MonoBehaviour
    {
        //public List<string> FriendlyCharacters = new List<string>();
        //public List<string> EmenyCharacters = new List<string>();
        string Backgrounds;        
        public enum GameState {
           Inventory,
           Exploration,
           Battle  
        };
        public GameState State;
        public Battle_Manager BattleManager;
        public Inventory_Manager InventoryManager;
        public Explore_Manager ExploreManager;
        public Task_Manager TaskManager;
        public Sound_Manager SoundManager;
        public Battle_Details_Manager battleDetailsManager;
        public Game_Effects_Manager GameEffectsManager;
        public Event_Manager EventManager;
        public Character_Select_Manager characterSelectManager;
        public static ILogger logger = Debug.unityLogger;

        /*public Game_Manager()
        {
            BattleManager = new Battle_Manager( this );
            SoundManager = new Sound_Manager();
            GameEffectsManager = new Game_Effects_Manager();
            //InventoryManager = gameObject.AddComponent(typeof(Inventory_Manager)) as Inventory_Manager;
            ExploreManager = gameObject.AddComponent(typeof(Explore_Manager)) as Explore_Manager;
            //SoundManager = gameObject.AddComponent(typeof(Sound_Manager)) as Sound_Manager;
            //BattleManager = gameObject.AddComponent(typeof(Battle_Manager)) as Battle_Manager;
            //BattleManager = BattleManager.MakeBattleManager( this.gameObject );
            //GameEffectsManager = gameObject.AddComponent(typeof(Game_Effects_Manager)) as Game_Effects_Manager;
            //BattleManager.gameManager = this;
        }*/

        void Awake(){
            SoundManager = gameObject.GetComponent<Sound_Manager>();
            TaskManager = gameObject.GetComponent<Task_Manager>();
            battleDetailsManager = gameObject.GetComponent<Battle_Details_Manager>();
            characterSelectManager = gameObject.GetComponent<Character_Select_Manager>();
            GameEffectsManager = gameObject.GetComponent<Game_Effects_Manager>();
            EventManager = gameObject.GetComponent<Event_Manager>();
            BattleManager = gameObject.GetComponent<Battle_Manager>();
            //BattleManager = new Battle_Manager(this);
        }

        void Start(){
            //BattleManager = new Battle_Manager(this);
            if( State == GameState.Battle ){
                BattleManager.StartBattle(5f);
            }
        }
    }
}

