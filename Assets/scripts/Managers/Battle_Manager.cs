using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Battle_Manager : MonoBehaviour
    {
        //public static BattleModel battleModel = new BattleModel();
        public static Event_Manager eventManager;
        public Game_Manager gameManager;
        public static GameObject UICanvas;
        public static GameObject tooltipCanvas;
        public static List<Panels_Manager> allPanelManagers;
        public static Sound_Manager soundManager;
        public static Game_Effects_Manager gameEffectManager;
        public static Task_Manager taskManager;
        public static Battle_Details_Manager battleDetailsManager;
        public static List<Battle_Interface_Manager> battleInterfaceManager  = new List<Battle_Interface_Manager>();
        public static Character_Select_Manager characterSelectManager;
        public static AssetFinder assetFinder;
        public static bool waitingForSkillTarget;
        public static bool offensiveSkill;
        public static bool battleStarted;
        public static GameObject pauseScreenHolder;
        public static bool disableActions = true;
        public static float vigor = 6;
        public static float originalvigor = 1;
        public static float actionPoints = 6;
        public static float maxActionPoints = 6;
        public static float originalActionPoints;
        static Text actionPointsText;
        //Turn properties
        //public static GameObject turnScreenHolder;
        static Image turnTimer;
        static Text playerTurnText;
        public static bool gamePaused;
        public static float turnTime = 30f;
        public static TurnEnum turn;
        static TurnEnum activeTurn;
        public enum TurnEnum
        {
            EnemyTurn,
            PlayerTurn
        }

        public static int turnCount = 0;

        void Awake(){
            gameManager = gameObject.GetComponent<Game_Manager>();
            battleDetailsManager = gameManager.battleDetailsManager;
            taskManager = gameManager.TaskManager;
            characterSelectManager = gameManager.characterSelectManager;
            soundManager = gameManager.SoundManager;
            gameEffectManager = gameManager.GameEffectsManager;
            eventManager = gameManager.EventManager;
            assetFinder = gameManager.AssetFinder;
            UICanvas = GameObject.Find("Canvas - UI");
            tooltipCanvas = GameObject.Find("Canvas - Tooltip");
            pauseScreenHolder = GameObject.Find("PauseOverlayUI");
            turnTimer = GameObject.Find("TurnTimer").GetComponent<Image>();
            if(pauseScreenHolder != null)
            {
                pauseScreenHolder.SetActive(false);
            }
        }

        void Start()
        {
            allPanelManagers = GameObject.FindGameObjectsWithTag("movementPanels").Select(o => o.GetComponent<Panels_Manager>()).ToList();
            taskManager.battleDetailsManager = battleDetailsManager;
            var bi = GameObject.FindGameObjectsWithTag("skillDisplayControl").ToList();
            characterSelectManager.UpdateCharacters();
            battleInterfaceManager = bi.Select( x => x.GetComponent<Battle_Interface_Manager>() ).ToList();
            battleInterfaceManager.Capacity = battleInterfaceManager.Count;
            originalActionPoints = actionPoints;
            actionPointsText = GameObject.Find("SkillPointsText").GetComponent<Text>();
            playerTurnText = GameObject.Find("PlayerTurnText").GetComponent<Text>();
            UpdateAPAmount();
        }

        void Update()
        {
            if (turn == activeTurn && battleStarted && CheckIfActionsComplete())
            {
                battleStarted = false;
                taskManager.CallTask(3f, () =>
                {
                    battleStarted = true;
                    ResetTurnTimer();
                });
            }
        }

        static void RegenerateAp()
        {
            if (actionPoints < maxActionPoints)
            {
                actionPoints += vigor;
                UpdateAPAmount();
            }
        }

        public static void UpdateAPAmount()
        {
            if (actionPoints > maxActionPoints)
            {
                actionPoints = maxActionPoints;
            }
            actionPointsText.text = actionPoints.ToString();
        }

        static bool CheckIfActionsComplete()
        {
            if (turn == TurnEnum.EnemyTurn)
            {
                var enemiesCanCast = characterSelectManager.enemyCharacters
                    .Where(o => (o.gameObject.GetComponent<Enemy_Skill_Manager>().copiedSkillList.Count() > 0 && !o.gameObject.GetComponent<Enemy_Skill_Manager>().hasCasted))
                    .Any(o => !o.gameObject.GetComponent<Enemy_Skill_Manager>().isCasting && !o.gameObject.GetComponent<Status_Manager>().DoesStatusExist("stun"));
                var enemiesCanAttack = characterSelectManager.enemyCharacters.Where(x => x.characterModel.canAutoAttack).Any(o => !o.gameObject.GetComponent<Auto_Attack_Manager>().hasAttacked); 
                return !enemiesCanAttack && !enemiesCanCast;
            } else
            {
                var hasTurnsLeft = characterSelectManager.friendlyCharacters
                    .Where(o => o.gameObject.GetComponent<Character_Manager>().characterModel.Haste > o.gameObject.GetComponent<Player_Skill_Manager>().turnsTaken
                    && !o.gameObject.GetComponent<Status_Manager>().DoesStatusExist("stun")).Any();
                //print(hasTurnsLeft);
                return Battle_Manager.actionPoints == 0 || !hasTurnsLeft;
            }
        }

        static void ResetTurnsTaken()
        {
            characterSelectManager.friendlyCharacters.ForEach(o => ((Player_Skill_Manager)o.baseManager.skillManager).turnsTaken = 0);
        }

        public static void ResetTurnTimer()
        {
            characterSelectManager.enemyCharacters.ForEach(o => { o.gameObject.GetComponent<Enemy_Skill_Manager>().hasCasted = false; o.gameObject.GetComponent<Auto_Attack_Manager>().hasAttacked = false;});
            if (taskManager.taskList.ContainsKey("turnTimerTask"))
            {
                taskManager.taskList["timerDisplayTask"].Stop();
                taskManager.taskList.Remove("timerDisplayTask");
                taskManager.taskList["turnTimerTask"].Stop();
                taskManager.taskList.Remove("turnTimerTask");
            }
            ++turnCount;
            characterSelectManager.UpdateCharacters();
            RegenerateAp();
            ResetTurnsTaken();
            CheckAndSetTurn();
        }

        public static void CheckAndSetTurn()
        {
            disableActions = true;
            turn = (turn == TurnEnum.EnemyTurn) ? TurnEnum.PlayerTurn : TurnEnum.EnemyTurn;
            playerTurnText.text = (turn == TurnEnum.EnemyTurn) ? "Enemy Turn" : "Player Turn";
            taskManager.TimerDisplayTask(turnTime, turnTimer, "timerDisplayTask");
            Battle_Manager.battleDetailsManager.BattleWarning($"Turn {turnCount + 1}", 3f);
            Battle_Manager.gameEffectManager.PanCamera(turn == TurnEnum.PlayerTurn);
            battleInterfaceManager.ForEach(o =>
            {
                o.KeyPressCancelSkill();
            });
            taskManager.CallTask(2f, () =>
            {
                activeTurn = turn;
                disableActions = false;
            });

            taskManager.CallTask(turnTime, () => {
                ResetTurnTimer();
            }, "turnTimerTask");
        }

        public static List<Battle_Interface_Manager> GetBattleInterfaces(){
            return battleInterfaceManager;
        }
        
        /*public List<Character_Manager> GetCharacterManagers( List<GameObject> go){
            var y = go.Select(o => o.GetComponent<Character_Manager>()).ToList();
            y.Capacity = y.Count;
            return y;
        }*/

        public static void PauseGame()
        {
            pauseScreenHolder.SetActive(gamePaused ? false : true);
            Time.timeScale = gamePaused ? 1 : 0;
            gamePaused = !gamePaused;
        }

        public List<Character_Manager> GetCharacterManagerByLoyalty(Boolean getFriendly){
            return getFriendly ? characterSelectManager.friendlyCharacters : characterSelectManager.enemyCharacters;
        }

        public class classState{
            public string Name;
            public bool Alive;
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

        public static GameObject GetRandomPanel(bool playerPanels)
        {
            var allPanels = playerPanels ? GameObject.FindGameObjectsWithTag("movementPanels") : GameObject.FindGameObjectsWithTag("enemyMovementPanels");
            var chosenPanels = new List<GameObject>();
            foreach (var panel in allPanels)
            {
                if (!panel.GetComponent<Panels_Manager>().currentOccupier)
                {
                    chosenPanels.Add(panel);
                }
            }
            var randomPanelNumber = UnityEngine.Random.Range(0, chosenPanels.Count);
            return chosenPanels[randomPanelNumber];
        }

        public static void HitBoxControl(bool hitBoxSwitch, Character_Model.RoleEnum role = Character_Model.RoleEnum.none)
        {
            if (role == Character_Model.RoleEnum.none)
            {
                foreach (Character_Manager character in characterSelectManager.friendlyCharacters)
                {
                    character.gameObject.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
                }
            }
            else
            {
                foreach (Character_Manager character in characterSelectManager.friendlyCharacters)
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

        public void EndTurn()
        {
            if (turn == TurnEnum.PlayerTurn)
            {
                ResetTurnTimer();
            }
        }

        public void StartBattle( float waitTime){

            taskManager.CallTask( waitTime, () => {
                Battle_Manager.battleStarted = true;
                CheckAndSetTurn();
                //friendlyCharacters.ForEach(o => o.baseManager.autoAttackManager.RunAttackLoop());
                //enemyCharacters.ForEach(o => o.baseManager.autoAttackManager.RunAttackLoop());
            });
        }
    }
}