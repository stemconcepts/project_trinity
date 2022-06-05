using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class BattleManager : MonoBehaviour
    {
        //public static BattleModel battleModel = new BattleModel();
        public static Event_Manager eventManager;
        public static GameManager gameManager;
        public static GameObject UICanvas;
        public static GameObject tooltipCanvas;
        public static List<PanelsManager> allPanelManagers;
        public static Sound_Manager soundManager;
        public static Game_Effects_Manager gameEffectManager;
        public static Task_Manager taskManager;
        public static BattleDetailsManager battleDetailsManager;
        public static List<BattleInterfaceManager> battleInterfaceManager = new List<BattleInterfaceManager>();
        public static Character_Select_Manager characterSelectManager;
        public static AssetFinder assetFinder;
        public static bool waitingForSkillTarget;
        public static bool offensiveSkill;
        public static bool battleStarted = false;
        public static bool battleOver = false;
        public static GameObject pauseScreenHolder;
        public static bool disableActions = true;
        public static float vigor = 6;
        public static float originalvigor = 1;
        public static float actionPoints = 6;
        public static float maxActionPoints = 6;
        public static float originalActionPoints;
        static Text actionPointsText;
        private static int totalEXP;
        private static List<ItemBase> loot = new List<ItemBase>();
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
            gameManager = gameObject.GetComponent<GameManager>();
            battleDetailsManager = gameManager.battleDetailsManager;
            taskManager = gameManager.TaskManager;
            characterSelectManager = gameManager.characterSelectManager;
            soundManager = gameManager.SoundManager;
            gameEffectManager = gameManager.GameEffectsManager;
            eventManager = gameManager.EventManager;
            assetFinder = gameManager.AssetFinder;
            UICanvas = GameObject.Find("Canvas - UI");
            tooltipCanvas = GameObject.Find("Canvas - Tooltip");
            //pauseScreenHolder = GameObject.Find("PauseOverlayUI");
            turnTimer = GameObject.Find("TurnTimer").GetComponent<Image>();
            if(pauseScreenHolder != null)
            {
                pauseScreenHolder.SetActive(false);
            }
        }

        void Start()
        {
            if (MainGameManager.instance.ShowTutorialText())
            {
                MainGameManager.instance.gameMessanger.DisplayMessage(MainGameManager.instance.GetText("Battle"), headerText: "A Battle has begun", waitTime : 2f, pauseGame: true);
            }
            LoadEnemies();
            allPanelManagers = GameObject.FindGameObjectsWithTag("movementPanels").Select(o => o.GetComponent<PanelsManager>()).ToList();
            taskManager.battleDetailsManager = battleDetailsManager;
            var bi = GameObject.FindGameObjectsWithTag("skillDisplayControl").ToList();
            //characterSelectManager.UpdateCharacters();
            battleInterfaceManager = bi.Select( x => x.GetComponent<BattleInterfaceManager>() ).ToList();
            battleInterfaceManager.Capacity = battleInterfaceManager.Count;
            originalActionPoints = actionPoints;
            actionPointsText = GameObject.Find("SkillPointsText").GetComponent<Text>();
            playerTurnText = GameObject.Find("PlayerTurnText").GetComponent<Text>();
            LoadEquipment();
            UpdateAPAmount();
        }

        void Update()
        {
            if (turn == activeTurn && battleStarted && CheckIfActionsComplete())
            {
                battleStarted = false;
                taskManager.CallTask(1f, () =>
                {
                    battleStarted = true;
                    ResetTurnTimer();
                });
            }
            CheckBattleOver();
        }

        public static int GetEXPValue()
        {
            return totalEXP;
        }

        public static List<ItemBase> GetLoot()
        {
            return loot;
        }

        public static void AddToEXP(int value)
        {
            totalEXP += value;
        }

        public static void AddToLoot(ItemBase l)
        {
            loot.Add(l);
        }

        void CheckBattleOver()
        {
            if (characterSelectManager.enemyCharacters.Count == 0 && !BattleManager.battleOver)
            {
                foreach (KeyValuePair<string, Task> t in taskManager.taskList)
                {
                    t.Value.Stop();
                   // taskManager.taskList[t.Key].Stop();
                }
                battleOver = true;
                MainGameManager.instance.gameMessanger.DisplayBattleResults(closeAction: () => UnloadBattle());
            }
        }

        void UnloadBattle()
        {
            MainGameManager.instance.SceneManager.UnLoadScene("battle");
           // GameObject.FindGameObjectWithTag("ExplorerCamera").SetActive(true);
        }

        void LoadEnemies()
        {
            var enemies = MainGameManager.instance.SceneManager.enemies;
            if (enemies.Count() > 0)
            {
                SummonCreatures(enemies, "monster", false);
            }
            
            /*foreach (var e in gameManager.SceneManager.enemies)
            {
                GameObject enemy = Instantiate(e, GameObject.Find("enemyHolder").transform);
            }   */
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
                    .Where(o => (o.gameObject.GetComponent<EnemySkillManager>().copiedSkillList.Count() > 0 && o.gameObject.GetComponent<EnemySkillManager>().copiedSkillList.Any(s => s.turnToReset == 0) && !o.gameObject.GetComponent<EnemySkillManager>().hasCasted))
                    .Any(o => !o.gameObject.GetComponent<EnemySkillManager>().isCasting && !o.gameObject.GetComponent<StatusManager>().DoesStatusExist("stun"));
                var enemiesCanAttack = characterSelectManager.enemyCharacters.Where(x => x.characterModel.canAutoAttack).Any(o => !o.gameObject.GetComponent<EnemyAutoAttackManager>().hasAttacked) 
                    && characterSelectManager.enemyCharacters.Any(o => !o.gameObject.GetComponent<EnemySkillManager>().isCasting && !o.gameObject.GetComponent<EnemySkillManager>().hasCasted
                    && !o.gameObject.GetComponent<StatusManager>().DoesStatusExist("stun")); 
                return !enemiesCanAttack && !enemiesCanCast;
            } else
            {
                var hasTurnsLeft = characterSelectManager.friendlyCharacters
                    .Where(o => o.gameObject.GetComponent<CharacterManager>().characterModel.Haste > o.gameObject.GetComponent<PlayerSkillManager>().turnsTaken
                    && !o.gameObject.GetComponent<StatusManager>().DoesStatusExist("stun") && 
                    (o.gameObject.GetComponent<PlayerSkillManager>().primaryWeaponSkills.Any(p => p.skillCost < BattleManager.actionPoints) || 
                    o.gameObject.GetComponent<PlayerSkillManager>().secondaryWeaponSkills.Any(p => p.skillCost < BattleManager.actionPoints) ||
                    o.gameObject.GetComponent<PlayerSkillManager>().skillModel.skillCost < BattleManager.actionPoints)).Any();
                //print(hasTurnsLeft);
                return BattleManager.actionPoints == 0 || !hasTurnsLeft;
            }
        }

        static void ResetTurnsTaken()
        {
            characterSelectManager.friendlyCharacters.ForEach(o => ((PlayerSkillManager)o.baseManager.skillManager).turnsTaken = 0);
        }

        public static void ResetTurnTimer()
        {
            characterSelectManager.enemyCharacters.ForEach(o => { 
                o.gameObject.GetComponent<EnemySkillManager>().hasCasted = false; 
                o.gameObject.GetComponent<EnemyAutoAttackManager>().hasAttacked = false;
            });
            characterSelectManager.friendlyCharacters.ForEach(o => {
                o.gameObject.GetComponent<PlayerSkillManager>().hasCasted = false;
                o.gameObject.GetComponent<PlayerAutoAttackManager>().hasAttacked = false;
            });
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
            BattleManager.battleDetailsManager.BattleWarning($"Turn {turnCount + 1}", 3f);
            BattleManager.gameEffectManager.PanCamera(turn == TurnEnum.PlayerTurn);
            var validChar = BattleManager.characterSelectManager.friendlyCharacters.Where(o => o.characterModel.isAlive && !o.baseManager.statusManager.DoesStatusExist("stun")).FirstOrDefault();
            BattleManager.characterSelectManager.SetSelectedCharacter(validChar.gameObject.name);
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

        public static List<BattleInterfaceManager> GetBattleInterfaces(){
            return battleInterfaceManager;
        }
        
        /*public List<Character_Manager> GetCharacterManagers( List<GameObject> go){
            var y = go.Select(o => o.GetComponent<Character_Manager>()).ToList();
            y.Capacity = y.Count;
            return y;
        }*/

        public static void PauseGame()
        {
            //pauseScreenHolder.SetActive(gamePaused ? false : true);
            Time.timeScale = gamePaused ? 1 : 0;
            gamePaused = !gamePaused;
        }

        /*public List<Character_Manager> GetCharacterManagerByLoyalty(bool getFriendly){
            return getFriendly ? characterSelectManager.friendlyCharacters : characterSelectManager.enemyCharacters;
        }*/

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

        public static List<CharacterManager> GetFriendlyCharacterManagers(){
            return characterSelectManager.friendlyCharacters;
        }

        public static List<EnemyCharacterManager> GetEnemyCharacterManagers()
        {
            return characterSelectManager.enemyCharacters;
        }

        public void LoadEquipment(){
            if (SavedDataManager.SavedDataManagerInstance != null)
            {
                //equips tank skills
                AttachWeapon(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.weapon,
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.secondWeapon,
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.bauble, characterSelectManager.guardianObject);
                AttachClassSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.classSkill, characterSelectManager.guardianObject);
                //equips healer skills
                AttachWeapon(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.weapon,
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.secondWeapon,
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.bauble, characterSelectManager.walkerObject);
                AttachClassSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.classSkill, characterSelectManager.walkerObject);
                //equips dps skills
                AttachWeapon(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.weapon,
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.secondWeapon,
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.bauble, characterSelectManager.stalkerObject);
                AttachClassSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.classSkill, characterSelectManager.stalkerObject);
            }
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
                if (!panel.GetComponent<PanelsManager>().currentOccupier)
                {
                    chosenPanels.Add(panel);
                }
            }
            var randomPanelNumber = UnityEngine.Random.Range(0, chosenPanels.Count);
            return chosenPanels[randomPanelNumber];
        }

        public static void HitBoxControl(bool hitBoxSwitch, CharacterModel.RoleEnum role = CharacterModel.RoleEnum.none)
        {
            if (role == CharacterModel.RoleEnum.none)
            {
                foreach (CharacterManager character in characterSelectManager.friendlyCharacters)
                {
                    character.gameObject.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
                }
            }
            else
            {
                foreach (CharacterManager character in characterSelectManager.friendlyCharacters)
                {
                    if (character.characterModel.role == role)
                    {
                        character.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
                    }
                }
            }
        }

        public void AttachClassSkill(SkillModel equipedSkill, GameObject charData)
        {
            charData.GetComponent<EquipmentManager>().classSkill = equipedSkill;
           //charData.GetComponent<EquipmentManager>().PopulateSkills();
        }

        public void AttachWeapon(weaponModel equipedWeapon, weaponModel secondEquipedWeapon, bauble equipedBauble, GameObject charData)
        {
            charData.GetComponent<EquipmentManager>().primaryWeapon = equipedWeapon;
            charData.GetComponent<EquipmentManager>().secondaryWeapon = secondEquipedWeapon;
            charData.GetComponent<EquipmentManager>().bauble = equipedBauble;
        }

        public static void GenerateLifeBars(GameObject creature)
        {
            var singleMinionDataItem = assetFinder.GetGameObjectFromPath("Assets/prefabs/combatInfo/character_info/singleMinionData.prefab");
            var creatureData = Instantiate(singleMinionDataItem, GameObject.Find("Panel MinionData").transform);

            var minionBaseManager = creature.GetComponent<EnemyCharacterManagerGroup>();
            minionBaseManager.autoAttackManager.hasAttacked = true;
            minionBaseManager.characterManager.healthBar = creatureData.transform.Find("Panel Minion HP").Find("Slider_enemy").gameObject;
            minionBaseManager.statusManager.statusHolderObject = creatureData.transform.Find("Panel Minion Status").Find("minionstatus").gameObject;
        }

        public void SummonCreatures(List<GameObject> summonedObjects, string creaturTag, bool friendly, GameObject panel = null)
        {
            Transform postion = friendly ? GameObject.Find("playerHolder").transform : GameObject.Find("enemyHolder").transform;
            for (int i = 0; i < summonedObjects.Count; i++)
            {
                var enemyIndex = BattleManager.characterSelectManager.enemyCharacters.Count() + i;
                //var singleMinionDataItem = BattleManager.assetFinder.GetGameObjectFromPath("Assets/prefabs/combatInfo/character_info/singleMinionData.prefab");
                //var creatureData = Instantiate(singleMinionDataItem, GameObject.Find("Panel MinionData").transform);
                //creatureData.name = $"{creaturTag}_{enemyIndex}_data";

                var newCreature = Instantiate(summonedObjects[i], postion);
                newCreature.name = $"{creaturTag}_{enemyIndex}";
                /*var minionBaseManager = newCreature.GetComponent<BaseCharacterManagerGroup>();
                minionBaseManager.autoAttackManager.hasAttacked = true;
                minionBaseManager.characterManager.healthBar = creatureData.transform.Find("Panel Minion HP").Find("Slider_enemy").gameObject;
                minionBaseManager.statusManager.statusHolderObject = creatureData.transform.Find("Panel Minion Status").Find("minionstatus").gameObject;*/

               // GenerateLifeBars(newCreature);

                if (panel)
                {
                    var panelManager = panel.GetComponent<PanelsManager>();
                    panelManager.currentOccupier = newCreature;
                    panelManager.SetStartingPanel(newCreature, true);
                    panelManager.SaveCharacterPositionFromPanel();
                }
                else
                {
                    Debug.Log("No Panel");
                }
            }
        }

        public static bool IsTankInThreatZone()
        {
            return (characterSelectManager.guardianObject.GetComponent<CharacterManagerGroup>().characterManager.characterModel as CharacterModel).inThreatZone;
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
                BattleManager.battleStarted = true;
                CheckAndSetTurn();
            });
        }
    }
}