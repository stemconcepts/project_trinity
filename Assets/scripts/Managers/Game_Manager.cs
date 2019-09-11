using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Game_Manager
    {
        List<string> FriendlyCharacters = new List<string>();
        List<string> EmenyCharacters = new List<string>();
        string Backgrounds;        
        Battle_Manager BattleManager;
        Inventory_Manager InventoryManager;
        Explore_Manager ExploreManager;
        public Sound_Manager SoundManager;
        public Game_Effects_Manager GameEffectsManager;
        public static ILogger logger = Debug.unityLogger;

        public Game_Manager()
        {
            BattleManager = new Battle_Manager( this );
            SoundManager = new Sound_Manager();
            GameEffectsManager = new Game_Effects_Manager();
            /*InventoryManager = gameObject.AddComponent(typeof(Inventory_Manager)) as Inventory_Manager;
            ExploreManager = gameObject.AddComponent(typeof(Explore_Manager)) as Explore_Manager;*/
            //SoundManager = gameObject.AddComponent(typeof(Sound_Manager)) as Sound_Manager;
            //BattleManager = gameObject.AddComponent(typeof(Battle_Manager)) as Battle_Manager;
            //BattleManager = BattleManager.MakeBattleManager( this.gameObject );
            //GameEffectsManager = gameObject.AddComponent(typeof(Game_Effects_Manager)) as Game_Effects_Manager;
            //BattleManager.gameManager = this;
        }
    }
}

