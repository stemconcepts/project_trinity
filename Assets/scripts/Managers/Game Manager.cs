using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Game_Manager : BasicManager
    {
        List<string> FriendlyCharacters {get; set;}
        List<string> EmenyCharacters {get; set;}
        string Backgrounds {get; set;}         
        Battle_Manager BattleManager {get; set;}       
        Inventory_Manager InventoryManager {get; set;}
        Explore_Manager ExploreManager {get; set;}
        public Sound_Manager SoundManager {get; set;}
        public Game_Effects_Manager GameEffectsManager {get; set;}
        public static ILogger logger = Debug.unityLogger;

        public Game_Manager()
        {
            InventoryManager = new Inventory_Manager();
            ExploreManager = new Explore_Manager();
            SoundManager = new Sound_Manager();
            BattleManager = new Battle_Manager();
            GameEffectsManager = new Game_Effects_Manager();
            BattleManager.gameManager = this;
        }
    }
}

