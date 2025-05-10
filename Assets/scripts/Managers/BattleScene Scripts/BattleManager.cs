using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Spine;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using Assets.scripts.Managers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Assets.scripts.Models.skillModels.swapSkills;
using Assets.scripts.Models.statusModels;
using UnityEditor;
using Assets.scripts.Helpers.Assets;
using DG.Tweening.Core.Easing;
using UnityEngine.TextCore.Text;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class BattleManager : MonoBehaviour
    {
        static int startingTankHealth;
        static int startingHealerHealth;
        static int startingDpsHealth;
        static List<StatusItem> tankStatus = new List<StatusItem>();
        static List<StatusItem> healerStatus = new List<StatusItem>();
        static List<StatusItem> dpsStatus = new List<StatusItem>();
        public static EyeSkill eyeSkill;

        public static Event_Manager eventManager;
        public static GenericEventManager<GenericEventEnum> GenericEventManager = new GenericEventManager<GenericEventEnum>();

        public static GameManager gameManager;
        public static GameObject UICanvas;
        public static GameObject tooltipCanvas;
        public GameObject tooltipCanvasTarget;
        public static List<PanelsManager> allPanelManagers;
        public static List<PanelsManager> AllEnemyPanelManagers;
        public static Sound_Manager soundManager;
        public static Game_Effects_Manager gameEffectManager;
        public static Task_Manager taskManager;
        public static BattleDetailsManager battleDetailsManager;
        public static List<BattleInterfaceManager> battleInterfaceManager = new List<BattleInterfaceManager>();
        public static Character_Select_Manager characterSelectManager;
        public static CombatLogManager combatLogManager;
        public static bool waitingForSkillTarget;
        public static bool offensiveSkill;
        public static bool battleStarted = false;
        public static GameObject pauseScreenHolder;
        public static bool disableActions = true;
        public static bool turnTimerOn = false;

        //Used to balance how defense works
        public static float DefenceConstant = 10;
        public static float ThornsWeight = 25;

        //Player ActionPoints
        public static float vigor = 6;
        public static float originalvigor = 6;
        public static float actionPoints = 6;
        public static float maxActionPoints = 6;
        public static float originalActionPoints;

        //Enemy ActionPoints
        public static float enemyVigor = 6;
        public static float originalEnemyVigor = 6;
        public static float enemyActionPoints = 6;
        public static float maxEnemyActionPoints = 6;
        public static float originalEnemyActionPoints;

        static Text actionPointsText;
        private static int totalEXP;
        private static List<ItemBase> loot = new List<ItemBase>();
        //Turn properties
        static Text playerTurnText;
        public static bool gamePaused;
        public static float turnTime = 30f;
        public static TurnEnum turn;
        public GearSwapManager _gearSwapManager;
        public static GearSwapManager GearSwapManager;
        public enum TurnEnum
        {
            EnemyTurn,
            PlayerTurn
        }
        public static int turnCount = 0;

        [Header("Use to add enemies to test")]
        public List<GameObject> SpawnEnemyForTesting;

        void Awake() {
            gameManager = gameObject.GetComponent<GameManager>();
            battleDetailsManager = gameManager.battleDetailsManager;
            taskManager = gameManager.TaskManager;
            characterSelectManager = gameManager.characterSelectManager;
            combatLogManager = gameObject.GetComponent<CombatLogManager>();
            gameEffectManager = gameManager.GameEffectsManager;
            eventManager = gameManager.EventManager;
            UICanvas = GameObject.Find("Canvas - UI");
            tooltipCanvas = tooltipCanvasTarget;
            _gearSwapManager = gameObject.GetComponent<GearSwapManager>();
            GearSwapManager = _gearSwapManager;
            //pauseScreenHolder = GameObject.Find("PauseOverlayUI");
            /*turnTimer = GameObject.Find("TurnTimer").GetComponent<Image>();
            turnTimer.gameObject.SetActive(false);*/
            if (pauseScreenHolder != null)
            {
                pauseScreenHolder.SetActive(false);
            }
            turn = TurnEnum.PlayerTurn;

            PlayerData playerData = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
            startingTankHealth = playerData.tankHealth;
            startingDpsHealth = playerData.dpsHealth;
            startingHealerHealth = playerData.healerHealth;
            LoadEnemies();
        }

        void Start()
        {
            if (MainGameManager.instance.ShowTutorialText("Battle"))
            {
                MainGameManager.instance.gameMessanger.DisplayMessage(MainGameManager.instance.GetText("Battle"), headerText: "A Battle has begun", waitTime: 2f, pauseGame: true);
            }

            allPanelManagers = GameObject.FindGameObjectsWithTag("movementPanels").Select(o => o.GetComponent<PanelsManager>()).ToList();
            AllEnemyPanelManagers = GameObject.FindGameObjectsWithTag("enemyMovementPanels").Select(o => o.GetComponent<PanelsManager>()).ToList();

            taskManager.battleDetailsManager = battleDetailsManager;
            var bi = GameObject.FindGameObjectsWithTag("skillDisplayControl").ToList();
            battleInterfaceManager = bi.Select(x => x.GetComponent<BattleInterfaceManager>()).ToList();
            battleInterfaceManager.Capacity = battleInterfaceManager.Count;
            SetActionPoints();
            LoadEquipment();
            UpdateAPAmount();
            SetStartingHealthAndStats();
            taskManager.taskList.Add("actionQueue", new Task(MainGameManager.instance.StartActionQueue()));
            characterSelectManager.UpdateCharacters();

            GenericEventManager.CreateGenericEventOrTriggerEvent(GenericEventEnum.GameOver);
            GenericEventManager.AddDelegateToEvent(GenericEventEnum.GameOver, DoGameOver);

            GenericEventManager.CreateGenericEventOrTriggerEvent(GenericEventEnum.PlayerTurn);
        }

        public void DamagePlayer(float value, bool trueDamage = false)
        {
            var character = characterSelectManager.walkerObject.GetComponent<CharacterManager>();

            var damageController = character.baseManager.damageManager;

            var damageModel = new BaseDamageModel();
            damageModel.damageImmidiately = true;
            damageModel.element = trueDamage ? elementType.trueDmg : elementType.none;
            damageModel.damageTaken = value;
            damageModel.baseManager = character.baseManager;
            damageModel.dueDmgTargets = new List<BaseCharacterManager>(){
                                    character
                                };
            damageModel.dmgSource = character;
            damageController.TakeDmg(damageModel, "DamageHealerTest");
        }

        public void DamageAllFriendly(float value, bool trueDamage = false)
        {
            var friendly = GetFriendlyCharacterManagers();

            foreach (var character in friendly)
            {
                var damageController = character.baseManager.damageManager;

                var damageModel = new BaseDamageModel();
                damageModel.damageImmidiately = true;
                damageModel.element = trueDamage ? elementType.trueDmg : elementType.none;
                damageModel.damageTaken = value;
                damageModel.baseManager = character.baseManager;
                damageModel.dueDmgTargets = new List<BaseCharacterManager>(){
                                    character
                                };
                damageModel.dmgSource = character;
                damageController.TakeDmg(damageModel, "DamageAllFriendlyTest");
            }
        }

        public void DamageAllEnemy(float value, bool trueDamage = false)
        {
            var friendly = GetFriendlyCharacterManagers();

            foreach (var character in friendly)
            {
                var damageController = character.baseManager.damageManager;

                var damageModel = new BaseDamageModel();
                damageModel.damageImmidiately = true;
                damageModel.element = trueDamage ? elementType.trueDmg : elementType.none;
                damageModel.incomingDmg = value;
                damageModel.baseManager = character.baseManager;
                damageModel.dueDmgTargets = new List<BaseCharacterManager>(){
                                    character
                                };
                damageModel.dmgSource = character;
                damageController.TakeDmg(damageModel, "DamageAllEnemyTest");
            }
        }

        void SetActionPoints()
        {
            originalEnemyActionPoints = enemyActionPoints;
            originalActionPoints = actionPoints;
            actionPointsText = GameObject.Find("SkillPointsText").GetComponent<Text>();
            playerTurnText = GameObject.Find("PlayerTurnText").GetComponent<Text>();
        }

        static void TriggerStartTurnEvents()
        {
            //Reset status actions after each turn
            if (turn == TurnEnum.EnemyTurn)
            {
                foreach (var character in characterSelectManager.enemyCharacters)
                {
                    character.baseManager.statusManager.RunStatusActions(turn);
                }
                GenericEventManager.CreateGenericEventOrTriggerEvent(GenericEventEnum.EnemyTurn);
            } else
            {
                foreach (var character in characterSelectManager.friendlyCharacters)
                {
                    character.baseManager.statusManager.RunStatusActions(turn);
                }
                GenericEventManager.CreateGenericEventOrTriggerEvent(GenericEventEnum.PlayerTurn);
            }

        }

        void Update()
        {
            if (battleStarted && CheckIfActionsComplete())
            {
                GenericEventManager.CreateGenericEventOrTriggerEvent(GenericEventEnum.GameOver);

                if (GetFriendlyCharacterManagers().Count > 0)
                {
                    battleStarted = false;
                    taskManager.ScheduleAction(() => {
                        battleStarted = true;
                        ResetCharactersAndSetTurn();
                    }, 2f);
                }
            }
            CheckBattleOver();
        }

        public static int GetEXPValue()
        {
            return totalEXP;
        }

        public static void AddToGearSwapPoints(int value)
        {
            GearSwapManager.AddGearSwapPoints(value);
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
            if (characterSelectManager && characterSelectManager.enemyCharacters.Count == 0 && battleStarted)
            {
                foreach (KeyValuePair<string, Task> t in taskManager.taskList)
                {
                    t.Value.Stop();
                }
                enemyActionPoints = 6;
                actionPoints = 6;
                battleStarted = false;
                MainGameManager.instance.gameMessanger.DisplayBattleResults(closeAction: () => UnloadBattle());
            }
        }

        void UnloadBattle()
        {
            print($"saved tasks : {taskManager.taskList.Count()}");
            foreach (var taskDictionary in taskManager.taskList)
            {
                print(taskDictionary.Key);
                taskManager.taskList[taskDictionary.Key].Stop();
            }

            taskManager.taskList.Clear();

            battleStarted = false;
            turn = TurnEnum.PlayerTurn;
            turnCount = 0;
            loot.ForEach(l =>
            {
                if (l.GetType() == typeof(GenericItem))
                {
                    MainGameManager.instance.exploreManager.AddToObtainedItems(l);
                } else
                {
                    gameManager.AddGearToInventory(l);
                }
            });

            var characters = characterSelectManager.GetCharacterScript<CharacterManager>(true);
            int tankHealth = 0;
            int healerHealth = 0;
            int dpsHealth = 0;
            characters.ForEach(c =>
            {
                int currentHealth = (int)Math.Round(c.characterModel.current_health, 0);
                switch (c.characterModel.role)
                {
                    case RoleEnum.tank:
                        tankHealth = currentHealth;
                        MainGameManager.instance.exploreManager.UpdateSliderHealth(currentHealth, RoleEnum.tank);
                        MainGameManager.instance.exploreManager.characterInfoDisplayController.SetHealthInfo(classType.guardian, currentHealth);
                        break;
                    case RoleEnum.healer:
                        healerHealth = currentHealth;
                        MainGameManager.instance.exploreManager.UpdateSliderHealth(currentHealth, RoleEnum.healer);
                        MainGameManager.instance.exploreManager.characterInfoDisplayController.SetHealthInfo(classType.walker, currentHealth);
                        break;
                    case RoleEnum.dps:
                        dpsHealth = currentHealth;
                        MainGameManager.instance.exploreManager.UpdateSliderHealth(currentHealth, RoleEnum.dps);
                        MainGameManager.instance.exploreManager.characterInfoDisplayController.SetHealthInfo(classType.stalker, currentHealth);
                        break;
                    default:
                        break;
                }
            });
            loot.Clear();
            SavedDataManager.SavedDataManagerInstance.SavePlayerHealth(tankHealth, dpsHealth, healerHealth);
            StopAllCoroutines();
            combatLogManager.ClearLogItems();
            MainGameManager.instance.SceneManager.UnLoadScene("battle");
        }

        void LoadEnemies()
        {
            var enemies = MainGameManager.instance.SceneManager.enemies;
            enemies.AddRange(SpawnEnemyForTesting);
            if (enemies.Count() > 0)
            {
                SummonCreatures(enemies, "monster", false);
            }
        }

        void LoadExplorerStatuses(bool player, List<ExplorerStatus> explorerStatuses)
        {
            var targets = player ? characterSelectManager.GetCharacterScript<CharacterManager>(true).Select(character => character.baseManager).ToList() 
                : characterSelectManager.GetCharacterScript<EnemyCharacterManager>(false).Select(character => character.baseManager).ToList();

            targets.ForEach(baseManager =>
            {
                explorerStatuses.ForEach(explorerStatus =>
                {
                    var statusModel = new StatusModel
                    {
                        singleStatus = explorerStatus.status,
                        power = MainGameManager.instance.exploreManager.GetCurroption(),
                        turnDuration = explorerStatus.duration,
                        baseManager = baseManager
                    };
                    statusModel.singleStatus.dispellable = explorerStatus.dispellable;
                    /*var statusModels = new List<StatusModel>();
                    explorerStatus.statuses.ForEach(status =>
                    {
                        var sm = new StatusModel
                        {
                            singleStatus = status,
                            power = MainGameManager.instance.exploreManager.GetCurroption(),
                            turnDuration = explorerStatus.duration,
                            baseManager = baseManager
                        };
                        sm.singleStatus.dispellable = explorerStatus.dispellable;
                        statusModels.Add(sm);
                        //baseManager.statusManager.RunStatusFunction(sm);
                    });*/
                    baseManager.statusManager.RunStatusFunction(statusModel);
                });
                //var curroptionEffect = MainGameManager.instance.assetFinder.GetGameObjectFromPath("Assets/prefabs/effects/2D_Magic_Attack/Prefabs/Darkness_2.prefab");
                //baseManager.effectsManager.CallEffect(curroptionEffect, "center");
                MainGameManager.instance.soundManager.playSound(baseManager.characterManager.idleSound, 0.5f);
            });
        }

        static void RegenerateAp()
        {
            if (enemyActionPoints < maxEnemyActionPoints)
            {
                enemyActionPoints += enemyVigor;
                if (enemyActionPoints > maxEnemyActionPoints)
                {
                    enemyActionPoints = maxEnemyActionPoints;
                }
            }
            if (actionPoints < maxActionPoints)
            {
                actionPoints += vigor;
                UpdateAPAmount();
            }
        }

        public static void EditStartingHealth(RoleEnum role, int addAmount)
        {
            switch (role)
            {
                case RoleEnum.tank:
                    startingTankHealth += addAmount;
                    MainGameManager.instance.exploreManager.UpdateSliderHealth(startingTankHealth, RoleEnum.tank);
                    break;
                case RoleEnum.dps:
                    startingDpsHealth += addAmount;
                    MainGameManager.instance.exploreManager.UpdateSliderHealth(startingDpsHealth, RoleEnum.dps);
                    break;
                case RoleEnum.healer:
                    startingHealerHealth += addAmount;
                    MainGameManager.instance.exploreManager.UpdateSliderHealth(startingHealerHealth, RoleEnum.healer);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds statuses to starting status that will be active at the start of the battle
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="status"></param>
        public static void AddToPlayerStatus(List<RoleEnum> roles, List<StatusItem> status)
        {
            roles.ForEach(o =>
            {
                switch (o)
                {
                    case RoleEnum.tank:
                        tankStatus = status;
                        break;
                    case RoleEnum.healer:
                        healerStatus = status;
                        break;
                    case RoleEnum.dps:
                        dpsStatus = status;
                        break;
                    default:
                        break;
                }
            });
        }

        public void SetStartingHealthAndStats()
        {
            var characters = characterSelectManager.GetCharacterScript<CharacterManager>(true);
            characters.ForEach(c =>
            {
                switch (c.characterModel.role)
                {
                    case RoleEnum.tank:
                        if (startingTankHealth > 0)
                        {
                            c.characterModel.Health = startingTankHealth;
                            //MainGameManager.instance.exploreManager.SetMaxHealth(c.characterModel.maxHealth, RoleEnum.tank);
                        }
                        tankStatus.ForEach(statusItem =>
                        {
                            var sm = new StatusModel
                            {
                                singleStatus = statusItem.status,
                                power = statusItem.power,
                                turnDuration = statusItem.duration,
                                baseManager = c.baseManager
                            };
                            c.baseManager.statusManager.RunStatusFunction(sm);
                        });
                        break;
                    case RoleEnum.healer:
                        if (startingHealerHealth > 0)
                        {
                            c.characterModel.Health = startingHealerHealth;
                            //MainGameManager.instance.exploreManager.SetMaxHealth(c.characterModel.maxHealth, RoleEnum.healer);
                        }
                        healerStatus.ForEach(statusItem =>
                        {
                            var sm = new StatusModel
                            {
                                singleStatus = statusItem.status,
                                power = statusItem.power,
                                turnDuration = statusItem.duration,
                                baseManager = c.baseManager
                            };
                            c.baseManager.statusManager.RunStatusFunction(sm);
                        });
                        break;
                    case RoleEnum.dps:
                        if (startingDpsHealth > 0)
                        {
                            c.characterModel.Health = startingDpsHealth;
                            //MainGameManager.instance.exploreManager.SetMaxHealth(c.characterModel.maxHealth, RoleEnum.dps);
                        }
                        dpsStatus.ForEach(statusItem =>
                        {
                            var sm = new StatusModel
                            {
                                singleStatus = statusItem.status,
                                power = statusItem.power,
                                turnDuration = statusItem.duration,
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
                // Ensure all conditions are correctly handled for EnemyTurn
                var enemiesWithUseableSkills = characterSelectManager.enemyCharacters
                    .Where(o => o.gameObject.GetComponent<EnemySkillManager>().copiedSkillList
                        .Any(s => s.turnToReset == 0 && s.skillCost <= enemyActionPoints && s.CanCastFromPosition(s.compatibleRows, o.baseManager))).ToList();

                var enemiesThatHaventCasted = enemiesWithUseableSkills
                    .Where(o => !(o.gameObject.GetComponent<EnemySkillManager>().hasCasted || o.gameObject.GetComponent<EnemySkillManager>().isCasting)).ToList();

                var enemiesNotStunned = enemiesThatHaventCasted
                    .Where(o => !o.gameObject.GetComponent<StatusManager>().DoesStatusExist(StatusNameEnum.Stun)).ToList();

                var enemiesCanCast = enemiesNotStunned.Count > 0;

                var canAffordSkills = enemiesNotStunned
                    .Where(o => o.gameObject.GetComponent<EnemySkillManager>().copiedSkillList
                        .Any(p => p.skillCost <= enemyActionPoints && p.CanCastFromPosition(p.compatibleRows, o.baseManager))).Any();

                var enemiesCanAttack = characterSelectManager.enemyCharacters
                    .Where(x => x.characterModel.canAutoAttack && enemyActionPoints >= 1 && !x.baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun))
                    .Any(o => !o.gameObject.GetComponent<EnemyAutoAttackManager>().hasAttacked) 
                    && characterSelectManager.enemyCharacters.Any(o => !o.gameObject.GetComponent<EnemySkillManager>().isCasting);

                var enemiesAnimating = characterSelectManager.enemyCharacters
                    .All(enemy => !enemy.GetComponent<Animation_Manager>().inAnimation);

                return (!enemiesAnimating && enemyActionPoints == 0) || (!enemiesCanAttack && !enemiesCanCast && !canAffordSkills);
            }
            else
            {
                // Ensure all conditions are correctly handled for PlayerTurn
                var charsWithTurnsLeft = characterSelectManager.friendlyCharacters
                    .Where(o => o.gameObject.GetComponent<CharacterManager>().characterModel.Haste > o.gameObject.GetComponent<PlayerSkillManager>().turnsTaken 
                        && !o.gameObject.GetComponent<PlayerSkillManager>().isCasting 
                        && !o.gameObject.GetComponent<StatusManager>().DoesStatusExist(StatusNameEnum.Stun)).ToList();

                var hasTurnsLeft = charsWithTurnsLeft.Count > 0;

                var canUseClassSkill = charsWithTurnsLeft
                    .Any(o => o.gameObject.GetComponent<PlayerSkillManager>().classSkillModel.skillCost <= actionPoints);

                var canAffordPrimarySkills = charsWithTurnsLeft
                    .Any(o => o.gameObject.GetComponent<PlayerSkillManager>().primaryWeaponSkills
                        .Any(p => p.skillCost <= actionPoints && p.CanCastFromPosition(p.compatibleRows, o.baseManager)));

                var canAffordSecondarySkills = charsWithTurnsLeft
                    .Any(o => o.gameObject.GetComponent<PlayerSkillManager>().secondaryWeaponSkills
                        .Any(p => p.skillCost <= actionPoints && p.CanCastFromPosition(p.compatibleRows, o.baseManager)));

                var canUseEquippedSkill = charsWithTurnsLeft
                    .Any(o => o.gameObject.GetComponent<EquipmentManager>().currentWeaponEnum == EquipmentManager.currentWeapon.Primary 
                        ? canAffordPrimarySkills 
                        : canAffordSecondarySkills);

                var charactersAnimating = characterSelectManager.friendlyCharacters
                    .All(character => !character.GetComponent<Animation_Manager>().inAnimation);

                return (!charactersAnimating && actionPoints == 0) || (!canUseClassSkill && !canUseEquippedSkill && !(hasTurnsLeft && actionPoints > 0));
            }
        }

        static void ResetTurnsTaken()
        {
            characterSelectManager.friendlyCharacters.ForEach(o => ((PlayerSkillManager)o.baseManager.skillManager).turnsTaken = 0);
        }

        public static void ResetCharactersAndSetTurn()
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

        /// <summary>
        /// Checks the turn and changes UI to reflect which side is going next. Also makes sure that a stunned character isn't selected on switch, 
        /// cancels all live actions starts enemy action queues
        /// </summary>
        public static void CheckAndSetTurn()
        {
            disableActions = true;

            if (turnCount > 0)
            {
                turn = (turn == TurnEnum.EnemyTurn) ? TurnEnum.PlayerTurn : TurnEnum.EnemyTurn;
            }

            //Show Next turn
            playerTurnText.text = (turn == TurnEnum.EnemyTurn) ? "Enemy Turn" : "Player Turn";

            //Run events that happen at the start of relevant turn, like status effects
            TriggerStartTurnEvents();

            taskManager.CallTask(2f, () =>
            {
                battleDetailsManager.BattleWarning($"Turn {turnCount + 1}", 3f);
                gameEffectManager.PanCamera(turn == TurnEnum.PlayerTurn);
                if (characterSelectManager.GetSelectedClassObject().GetComponent<StatusManager>().DoesStatusExist(StatusNameEnum.Stun))
                {
                    var validChar = characterSelectManager.friendlyCharacters.Where(o => o.characterModel.isAlive && !o.baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun)).FirstOrDefault();
                    characterSelectManager.SetSelectedCharacter(validChar.gameObject.name);
                }

                //Used to cancel all abilities that are in progress.. is breaking code if method runs before skill is complete
                /*battleInterfaceManager.ForEach(o =>
                {
                    o.KeyPressCancelSkill();
                });*/

                disableActions = false;
                if (turn == TurnEnum.EnemyTurn)
                {
                    CheckAndChangePhase();
                    StartEnemyAutoAttacks();
                    StartEnemyAbilities();
                }
            });
            if (turnTimerOn)
            {
                taskManager.CallTask(turnTime, () =>
                {
                    ResetCharactersAndSetTurn();
                }, "turnTimerTask");
            }
        }

        public static List<BattleInterfaceManager> GetBattleInterfaces() {
            return battleInterfaceManager;
        }

        public static void PauseGame()
        {
            Time.timeScale = gamePaused ? 1 : 0;
            gamePaused = !gamePaused;
        }

        public class classState {
            public string Name;
            public bool Alive;
            public bool Selected;
            public bool LastSelected;
            public classState(string name, bool alive, bool selected, bool lastSelected) {
                Name = name;
                Alive = alive;
                Selected = selected;
                LastSelected = lastSelected;
            }
        }

        public void LoadArea() {

        }

        public void LoadStatBonuses() {

        }

        public static List<CharacterManager> GetFriendlyCharacterManagers() {
            return characterSelectManager.friendlyCharacters;
        }

        public static List<EnemyCharacterManager> GetEnemyCharacterManagers()
        {
            return characterSelectManager.enemyCharacters;
        }

        static void CheckAndChangePhase()
        {
            GetEnemyCharacterManagers().ForEach(o =>
            {
                o.baseManager.GetComponent<PhaseManager>().CheckAndChangePhase();
            });
        }

        /// <summary>
        /// Starts 1 round of enemy auto attacks
        /// </summary>
        static void StartEnemyAutoAttacks()
        {
            GetEnemyCharacterManagers().ForEach(o =>
            {
                ((EnemyAutoAttackManager)o.baseManager.autoAttackManager).StartAutoAttack();
            });
        }

        /// <summary>
        /// Starts 1 round of enemy abilities
        /// </summary>
        static void StartEnemyAbilities()
        {
            GetEnemyCharacterManagers().ForEach(o =>
            {
                ((EnemySkillManager)o.baseManager.skillManager).StartSkill();
            });
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
                //Attach Eye Skill
                AttachEyeSkill(MainGameManager.instance.SkillFinder.GetEyeSkill(SavedDataManager.SavedDataManagerInstance.persistentData.playerData.eyeSkillName));
            }
        }

        public static void ClearAllVoidZones()
        {
            foreach (var panel in allPanelManagers)
            {
                panel.ClearVoidZone();
            }
        }

        public static void SetFadeOnAllPanels(float amount, float duration)
        {
            BattleManager.allPanelManagers.ForEach(o =>
            {
                o.fadeAmount = amount;
                o.SetFade(amount, duration);
            });
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

        public static List<PanelsManager> GetAllFreePanels(bool playerPanels)
        {
            var allPanels = playerPanels ? allPanelManagers : AllEnemyPanelManagers;

            return allPanels.Where(panel => panel.currentOccupier == null)
                .ToList();
        }

        public static void HitBoxControl(bool hitBoxSwitch, RoleEnum role = RoleEnum.none)
        {
            if (role == RoleEnum.none)
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

        public void AttachEyeSkill(EyeSkill equipedSkill)
        {
            eyeSkill = equipedSkill;
        }

        public void AttachClassSkill(SkillModel equipedSkill, GameObject charData)
        {
            charData.GetComponent<EquipmentManager>().classSkill = equipedSkill;
        }

        public void AttachWeapon(WeaponModel equipedWeapon, WeaponModel secondEquipedWeapon, Bauble equipedBauble, GameObject charData)
        {
            if (equipedWeapon)
            {
                charData.GetComponent<EquipmentManager>().primaryWeapon = equipedWeapon;
            }
            if (secondEquipedWeapon)
            {
                charData.GetComponent<EquipmentManager>().secondaryWeapon = secondEquipedWeapon;
            }
            if (equipedBauble)
            {
                charData.GetComponent<EquipmentManager>().bauble = equipedBauble;
            }
        }

        public static void GenerateLifeBars(GameObject creature)
        {
            var singleMinionDataItem = MainGameManager.instance.ItemFinder.MinionDataItem;

            var minionBaseManager = creature.GetComponent<EnemyCharacterManagerGroup>();
            var selectorController = (minionBaseManager.characterManager as EnemyCharacterManager).enemyCharacterSelector.GetComponent<EnemySelectorController>();
            var lifeBarLocation = selectorController.healthHolder;

            if (lifeBarLocation == null)
            {
                throw new Exception("EnemyData object missing in character selector");
            }

            var creatureData = Instantiate(singleMinionDataItem, lifeBarLocation.transform);

            minionBaseManager.autoAttackManager.hasAttacked = true;
            minionBaseManager.characterManager.healthBar = creatureData.transform.Find("Panel Minion HP").Find("Slider_enemy").gameObject;
        }

        public static void GenerateEnemySelector(GameObject creature)
        {
            var characterManager = creature.GetComponent<EnemyCharacterManager>();
            var enemySelector = Instantiate(characterSelectManager.EnemyButtonTemplate,
                characterSelectManager.enemySelectorTransform);
            if (enemySelector)
            {
                characterManager.enemyCharacterSelector = enemySelector;
                var selectorController = enemySelector.GetComponent<EnemySelectorController>();
                selectorController.imageHolder.sprite = characterManager.enemySprite != null ? characterManager.enemySprite : selectorController.imageHolder.sprite;
                var buttonController = enemySelector.GetComponent<Button>();
                buttonController.onClick.AddListener(
                    () => characterManager.baseManager.characterInteractionManager.SelectUnit(creature)
                );
            }
        }

        public void SummonCreatures(List<GameObject> summonedObjects, string creaturTag, bool friendly, GameObject panel = null)
        {
            Transform postion = friendly ? GameObject.Find("playerHolder").transform : GameObject.Find("enemyHolder").transform;
            for (int i = 0; i < summonedObjects.Count; i++)
            {
                var enemyIndex = characterSelectManager.enemyCharacters.Count() + i;
                var newCreature = Instantiate(summonedObjects[i], postion);
                newCreature.name = $"{creaturTag}_{enemyIndex}";
                if (panel)
                {
                    var panelManager = panel.GetComponent<PanelsManager>();
                    panelManager.currentOccupier = newCreature;
                    panelManager.SetStartingPanel(newCreature);
                    panelManager.SaveCharacterPositionFromPanel();
                }
            }
        }

        public static bool IsTankInThreatZone()
        {
            var tank = characterSelectManager.guardianObject.GetComponent<CharacterManagerGroup>().characterManager.characterModel as CharacterModel;
            return tank.isAlive && tank.inThreatZone;
        }

        public void EndTurn()
        {
            if (turn == TurnEnum.PlayerTurn)
            {
                ResetCharactersAndSetTurn();
            }
        }

        public void StartBattle(float waitTime)
        {
            taskManager.CallTask(waitTime, () =>
            {
                LoadExplorerStatuses(true, MainGameManager.instance.SceneManager.playerStatuses);
                LoadExplorerStatuses(false, MainGameManager.instance.SceneManager.enemyStatuses);
                _gearSwapManager.CheckGearType();
                battleStarted = true;
                CheckAndSetTurn();
            });
        }

        void DoGameOver()
        {
            if (GetFriendlyCharacterManagers().Count == 0)
            {
                StopAllCoroutines();
                battleStarted = false;
                MainGameManager.instance.DisableEnableLiveBoxColliders(false);
                taskManager.CallTask(1f, () =>
                {
                    MainGameManager.instance.SceneManager.LoadScene("GameOver", true);
                });
            }
        }
    }
}