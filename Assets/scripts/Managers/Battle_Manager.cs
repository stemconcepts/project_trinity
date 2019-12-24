using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Battle_Manager : MonoBehaviour
    {
        BattleModel battleModel;
        public static Event_Manager eventManager;
        public Game_Manager gameManager;
        public static List<Panels_Manager> allPanelManagers;
        public static Sound_Manager soundManager;
        public static Game_Effects_Manager gameEffectManager;
        public static Task_Manager taskManager;
        public static Battle_Details_Manager battleDetailsManager;
        public static List<Battle_Interface_Manager> battleInterfaceManager  = new List<Battle_Interface_Manager>();
        public static Character_Select_Manager characterSelectManager;
        public static List<Character_Manager> friendlyCharacters = new List<Character_Manager>();
        public static List<Character_Manager> enemyCharacters = new List<Character_Manager>();
        public static AssetFinder assetFinder;
        public static bool waitingForSkillTarget;
        public static bool offensiveSkill;
        public static bool battleStarted;

        void Awake(){
            gameManager = gameObject.GetComponent<Game_Manager>();
            battleDetailsManager = gameManager.battleDetailsManager;
            taskManager = gameManager.TaskManager;
            characterSelectManager = gameManager.characterSelectManager;
            soundManager = gameManager.SoundManager;
            gameEffectManager = gameManager.GameEffectsManager;
            eventManager = gameManager.EventManager;
            assetFinder = gameManager.AssetFinder;
        }

        void Start()
        {
            allPanelManagers = GameObject.FindGameObjectsWithTag("movementPanels").Select(o => o.GetComponent<Panels_Manager>()).ToList();
            taskManager.battleDetailsManager = battleDetailsManager;
            var bi = GameObject.FindGameObjectsWithTag("skillDisplayControl").ToList();
            friendlyCharacters = GetCharacterManagers(GameObject.FindGameObjectsWithTag("Player").ToList());
            friendlyCharacters.Capacity = friendlyCharacters.Count;
            enemyCharacters = GetCharacterManagers(GameObject.FindGameObjectsWithTag("Enemy").ToList());
            enemyCharacters.Capacity = enemyCharacters.Count;
            battleInterfaceManager = bi.Select( x => x.GetComponent<Battle_Interface_Manager>() ).ToList();
            battleInterfaceManager.Capacity = battleInterfaceManager.Count;
            //bi.ForEach(o => o.GetComponent<Battle_Interface_Manager>().SkillSet(friendlyCharacters[0].baseManager.skillManager));
        }
    
        public static List<Battle_Interface_Manager> GetBattleInterfaces(){
            return battleInterfaceManager;
        }
        
        public List<Character_Manager> GetCharacterManagers( List<GameObject> go){
            var y = go.Select(o => o.GetComponent<Character_Manager>()).ToList();
            y.Capacity = y.Count;
            return y;
        }

        public List<Character_Manager> GetCharacterManagerByLoyalty(Boolean getFriendly){
            return getFriendly ? friendlyCharacters : enemyCharacters;
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

        /*public void LoadCharacters( List<Character_Manager> characters ){
            foreach (var character in characters)
            {
                if( character.characterModel.currentPanel ){
                   character.characterModel.currentPanel.GetComponent<Panels_Manager>().SetStartingPanel( character.gameObject );
                }
            }
        }*/

        public static List<Character_Manager> GetCharacterManagers( Boolean friendly ){
            return !friendly ? characterSelectManager.enemyCharacters : characterSelectManager.friendlyCharacters;
        }

        public void LoadSkillDisplay(){
            
        }

        public static void ClearAllVoidZones()
        {
            foreach (var panel in allPanelManagers)
            {
                panel.ClearVoidZone();
            }
        }

        public static void HitBoxControl(bool hitBoxSwitch, Character_Model.RoleEnum role = Character_Model.RoleEnum.none)
        {
            if (role == Character_Model.RoleEnum.none)
            {
                foreach (Character_Manager character in Battle_Manager.friendlyCharacters)
                {
                    character.gameObject.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
                }
            }
            else
            {
                foreach (Character_Manager character in Battle_Manager.friendlyCharacters)
                {
                    if (character.characterModel.role == role)
                    {
                        character.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
                    }
                }
            }
        }

        public static bool IsTankInThreatZone()
        {
            return characterSelectManager.guardianObject.GetComponent<Base_Character_Manager>().characterManager.characterModel.inThreatZone;
        }

        public void StartBattle( float waitTime){

            taskManager.CallTask( waitTime, () => {
                Battle_Manager.battleStarted = true;
                friendlyCharacters.ForEach(o => o.baseManager.autoAttackManager.RunAttackLoop());
                enemyCharacters.ForEach(o => o.baseManager.autoAttackManager.RunAttackLoop());
            });
        }
    }
}