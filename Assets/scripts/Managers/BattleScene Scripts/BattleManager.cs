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
        static int startingTankHealth;
        static int startingHealerHealth;
        static int startingDpsHealth;
        static List<SingleStatusModel> tankStatus = new List<SingleStatusModel>();
        static List<SingleStatusModel> healerStatus = new List<SingleStatusModel>();
        static List<SingleStatusModel> dpsStatus = new List<SingleStatusModel>();

        public static Event_Manager eventManager;
        public static GameManager gameManager;
        public static GameObject UICanvas;
        public static GameObject tooltipCanvas;
        public GameObject tooltipCanvasTarget;
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
        //public static bool battleOver = false;
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
            tooltipCanvas = tooltipCanvasTarget;
            //pauseScreenHolder = GameObject.Find("PauseOverlayUI");
            turnTimer = GameObject.Find("TurnTimer").GetComponent<Image>();
            if(pauseScreenHolder != null)
            {
                pauseScreenHolder.SetActive(false);
            }
            LoadEnemies();
        }

        void Start()
        {
            if (MainGameManager.instance.ShowTutorialText())
            {
                MainGameManager.instance.gameMessanger.DisplayMessage(MainGameManager.instance.GetText("Battle"), headerText: "A Battle has begun", waitTime : 2f, pauseGame: true);
            }
            allPanelManagers = GameObject.FindGameObjectsWithTag("movementPanels").Select(o => o.GetComponent<PanelsManager>()).ToList();
            taskManager.battleDetailsManager = battleDetailsManager;
            var bi = GameObject.FindGameObjectsWithTag("skillDisplayControl").ToList();
            battleInterfaceManager = bi.Select( x => x.GetComponent<BattleInterfaceManager>() ).ToList();
            battleInterfaceManager.Capacity = battleInterfaceManager.Count;
            originalActionPoints = actionPoints;
            actionPointsText = GameObject.Find("SkillPointsText").GetComponent<Text>();
            playerTurnText = GameObject.Find("PlayerTurnText").GetComponent<Text>();
            actionPoints = 6;
            LoadEquipment();
            UpdateAPAmount();
            SetStartingHealthAndStats();
            //battleOver = false;
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

        /// <summary>
        /// Adds statuses to starting status that will be active at the start of the battle
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="status"></param>
        public static void AddToPlayerStatus(List<BaseCharacterModel.RoleEnum> roles, List<SingleStatusModel> status)
        {
            roles.ForEach(o =>
            {
                switch (o)
                {
                    case BaseCharacterModel.RoleEnum.tank:
                        tankStatus = status;
                        break;
                    case BaseCharacterModel.RoleEnum.healer:
                        healerStatus = status;
                        break;
                    case BaseCharacterModel.RoleEnum.dps:
                        dpsStatus = status;
                        break;
                    default:
                        break;
                }
            });
        }

        void CheckBattleOver()
        {
            if (characterSelectManager.enemyCharacters.Count == 0 && /*!battleOver &&*/ battleStarted)
            {
                foreach (KeyValuePair<string, Task> t in taskManager.taskList)
                {
                    t.Value.Stop();
                    //Debug.Log(t.Key);
                }
                //battleOver = true;
                loot.Clear();
                battleStarted = false;
                MainGameManager.instance.gameMessanger.DisplayBattleResults(closeAction: () => UnloadBattle());
            }
        }

        void UnloadBattle()
        {
            battleStarted = false;
            turn = TurnEnum.PlayerTurn;
            loot.ForEach(l =>
            {
                if (l.GetType() == typeof(GenericItem))
                {
                    ExploreManager.AddToObtainedItems(l);
                } else
                {
                    gameManager.AddGearToInventory(l);
                }
            });

            var characters = characterSelectManager.GetCharacterManagers<CharacterManager>(GameObject.FindGameObjectsWithTag("Player").ToList());
            characters.ForEach(c =>
            {
                switch (c.characterModel.role)
                {
                    case BaseCharacterModel.RoleEnum.tank:
                        startingTankHealth = (int)Math.Round(c.characterModel.current_health, 0);
                        break;
                    case BaseCharacterModel.RoleEnum.healer:
                        startingHealerHealth = (int)Math.Round(c.characterModel.current_health, 0);
                        break;
                    case BaseCharacterModel.RoleEnum.dps:
                        startingDpsHealth = (int)Math.Round(c.characterModel.current_health, 0);
                        break;
                    default:
                        break;
                }
            });
            MainGameManager.instance.SceneManager.UnLoadScene("battle");
        }

        void LoadEnemies()
        {
            var enemies = MainGameManager.instance.SceneManager.enemies;
            if (enemies.Count() > 0)
            {
                SummonCreatures(enemies, "monster", false);
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

        public void SetStartingHealthAndStats()
        {
            var characters = characterSelectManager.GetCharacterManagers<CharacterManager>(GameObject.FindGameObjectsWithTag("Player").ToList());
            characters.ForEach(c =>
            {
                switch (c.characterModel.role)
                {
                    case BaseCharacterModel.RoleEnum.tank:
                        if (startingTankHealth > 0)
                        {
                            c.characterModel.Health = startingTankHealth;
                        }
                        tankStatus.ForEach(o =>
                        {
                            var sm = new StatusModel
                            {
                                singleStatus = o,
                                power = 2,
                                turnDuration = 4,
                                baseManager = c.baseManager
                            };
                            c.baseManager.statusManager.RunStatusFunction(sm);
                        });
                        break;
                    case BaseCharacterModel.RoleEnum.healer:
                        if (startingHealerHealth > 0)
                        {
                            c.characterModel.Health = startingHealerHealth;
                        }
                        healerStatus.ForEach(o =>
                        {
                            var sm = new StatusModel
                            {
                                singleStatus = o,
                                power = 2,
                                turnDuration = 4,
                                baseManager = c.baseManager
                            };
                            c.baseManager.statusManager.RunStatusFunction(sm);
                        });
                        break;
                    case BaseCharacterModel.RoleEnum.dps:
                        if (startingDpsHealth > 0)
                        {
                            c.characterModel.Health = startingDpsHealth;
                        }
                        dpsStatus.ForEach(o =>
                        {
                            var sm = new StatusModel
                            {
                                singleStatus = o,
                                power = 2,
                                turnDuration = 4,
                                baseManager = c.baseManager
                            };
                            c.baseManager.statusManager.RunStatusFunction(sm);
                        });
                        break;
                    default:
                        break;
                }
            });
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

        public static void PauseGame()
        {
            Time.timeScale = gamePaused ? 1 : 0;
            gamePaused = !gamePaused;
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