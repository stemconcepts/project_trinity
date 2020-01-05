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
        public AssetFinder AssetFinder;
        public Character_Select_Manager characterSelectManager;
        public static ILogger logger = Debug.unityLogger;

        void Awake(){
            SoundManager = gameObject.GetComponent<Sound_Manager>();
            TaskManager = gameObject.GetComponent<Task_Manager>();
            battleDetailsManager = gameObject.GetComponent<Battle_Details_Manager>();
            characterSelectManager = gameObject.GetComponent<Character_Select_Manager>();
            GameEffectsManager = gameObject.GetComponent<Game_Effects_Manager>();
            EventManager = gameObject.GetComponent<Event_Manager>();
            BattleManager = gameObject.GetComponent<Battle_Manager>();
            AssetFinder = gameObject.GetComponent<AssetFinder>();
        }

        void Start(){
            if( State == GameState.Battle ){
                BattleManager.StartBattle(5f);
            }
        }
    }
}

