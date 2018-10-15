using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class GameManager : BasicManager
    {
        List<string> FriendlyCharacters {get; set;}
        List<string> EmenyCharacters {get; set;}
        string Backgrounds {get; set;}         
        Battle_Manager BattleManager {get; set;}       
        Inventory_Manager InventoryManager {get; set;}
        Explore_Manager ExploreManager {get; set;}

        public GameManager()
        {
            BattleManager = new Battle_Manager();
            InventoryManager = new Inventory_Manager();
            ExploreManager = new Explore_Manager();
        }
    }
}

