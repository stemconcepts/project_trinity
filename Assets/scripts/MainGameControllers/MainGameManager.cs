using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.XR;
using Assets.scripts.Managers;
using System;
using static AssemblyCSharp.PanelsManager;
using Assets.scripts.Helpers.Assets;
using Assets.scripts.Helpers.Utility;
using UnityEngine.SceneManagement;

namespace AssemblyCSharp
{
    public class MainGameManager : MonoBehaviour
    {
        bool ShowTutorial = false;
        bool ShowDialogue = false;

        public GenericEventManager<GenericEventEnum> GenericEventManager = new GenericEventManager<GenericEventEnum>();

        public ItemFinder ItemFinder;
        public SkillFinder SkillFinder;
        public EquipmentFinder EquipmentFinder;
        public StatusFinder StatusFinder;

        public sceneManager SceneManager;
        public Sound_Manager soundManager;
        public Canvas GlobalCanvas;
        public GameMessanger gameMessanger;
        public MainGameTaskManager taskManager;
        public ToolTipManager tooltipManager;
        public Game_Effects_Manager gameEffectManager;
        public ExploreManagerV2 exploreManager;
        public static MainGameManager instance;
        Dictionary<string, string> TutorialText = new Dictionary<string, string>();
        Dictionary<string, bool> TutorialsShown = new Dictionary<string, bool>();
        public Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
        [HideInInspector]
        public Camera currentCamera;

        [Header("Cursors")]
        public Texture2D cursorImage;

        [Header("Music Tracks")]
        public AudioClip TutorialInventoryTrack;
        public AudioClip TutorialExploreTrack;
        public AudioClip TutorialCombatTrack;

        void Awake()
        {
            MakeSingleton();
        }

        void CursorControl()
        {
            Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.ForceSoftware);
        }

        Dictionary<string, int> GetStartingHealth() {
            PlayerData playerData = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
            if (playerData != null && playerData.tankHealth > 0 && playerData.dpsHealth > 0 && playerData.healerHealth > 0)
            {
                return new Dictionary<string, int>() {
                    {"guardian", playerData.tankHealth },
                    {"stalker", playerData.dpsHealth },
                    {"walker", playerData.healerHealth }
                };
            }
            return new Dictionary<string, int> {
                {"guardian", 50 },
                {"stalker", 40 },
                {"walker", 35 }
            };
        }

        /// <summary>
        /// Returns tutorial text
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetText(string key)
        {
            return TutorialText[key];
        }

        /// <summary>
        /// Generates tutorial text
        /// </summary>
        void GenerateTutorialTexts()
        {
            TutorialText.Add("NewGame", "Press New Game to begin.");
            TutorialsShown.Add("NewGame", false);

            StringBuilder inventoryText = new StringBuilder("You must equip <b>Weapons</b> and a <b> Skill </b> before you start your journey.\n");
            inventoryText.Append("You can swap from the equipment view to the skill view by using the top navigation\n");
            inventoryText.Append("Hover over an item to view details and drag and drop an item to equip it");
            TutorialText.Add("Inventory", inventoryText.ToString());
            TutorialsShown.Add("Inventory", false);

            StringBuilder battleText = new StringBuilder("Use the abilities from your equipped weapons to survive and defeat the enemies.\n");
            battleText.Append("Each ability has an <b>Action Point</b> cost and cooldown.\n");
            battleText.Append("You can swap to your secondary weapon by selecting the large button in the middle.\n");
            battleText.Append("Drag and drop your characters in the desired panels to move them, this costs <b>1 Action Point</b> and expends the turn for that character");
            TutorialText.Add("Battle", battleText.ToString());

            StringBuilder corruptionText = new StringBuilder("<b>Corruption</b> grows as you explore the dungeon.\n");
            corruptionText.Append("The higher the <b>Corruption</b> the higher the chance for enemies to gain beneficial buffs at the start of combat,\n");
            corruptionText.Append("but you will also gain access to a single use of your selected <b>Corruption Ability</b>.\n");
            corruptionText.Append("<b>Corruption</b> also increases the chance that a hazard may occur while exploring.");
            TutorialText.Add("CorruptionCounter", corruptionText.ToString());

            
            TutorialsShown.Add("Battle", false);
        }

        void MakeSingleton()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            } else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void NewGame()
        {
            SavedDataManager.SavedDataManagerInstance.ResetPlayerData();
            var health = GetStartingHealth();
            SavedDataManager.SavedDataManagerInstance.SavePlayerHealth(health["guardian"], health["stalker"], health["walker"]);
            SceneManager.LoadInventory(false);
        }

        public void ContinueGame()
        {
            //var health = GetStartingHealth();
            SavedDataManager.SavedDataManagerInstance.SavePlayerHealth(50, 40, 35);
            if (SceneManager.TeamReady())
            {
                //SavedDataManager.SavedDataManagerInstance.ResetDungeonData();
                SceneManager.LoadExploration(false);
            }
            else
            {
                SceneManager.LoadInventory(false);
            }
        }

        public void LoadTestBattle()
        {
            SavedDataManager.SavedDataManagerInstance.ResetPlayerData();
            var health = GetStartingHealth();
            SavedDataManager.SavedDataManagerInstance.SavePlayerHealth(health["guardian"], health["stalker"], health["walker"]);
            SceneManager.LoadScene("battle", false);
        }

        public void GetCanvasAndMainCamera()
        {
            //GlobalCanvas = GameObject.Find("Canvas - Message").GetComponent<Canvas>();
            GlobalCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        public void SaveScene(string scene)
        {
            instance.SceneManager.currentScene = scene;
        }

        void CheckPlayerReady()
        {
            PlayerData playerData = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
            if (playerData != null)
            {
                SceneManager.tankReady = SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.weapon &&
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.secondWeapon && SavedDataManager.SavedDataManagerInstance.persistentData.playerData.tankEquipment.classSkill;
                SceneManager.dpsReady = SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.weapon &&
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.secondWeapon && SavedDataManager.SavedDataManagerInstance.persistentData.playerData.dpsEquipment.classSkill;
                SceneManager.healerReady = SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.weapon &&
                    SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.secondWeapon && SavedDataManager.SavedDataManagerInstance.persistentData.playerData.healerEquipment.classSkill;
            }
        }

        public bool ShowTutorialText(string tutorial)
        {
            var selectedTutorial = TutorialsShown[tutorial];
            var show = ShowTutorial && !selectedTutorial;
            if (show)
            {
                TutorialsShown[tutorial] = true;
            }
            return show;
        }

        public IEnumerator StartActionQueue()
        {
            while (true)
            {
                while (actionQueue.Count > 0)
                    yield return StartCoroutine(actionQueue.Dequeue());

                yield return null;
            }
        }

        public List<BoxCollider2D> GetActiveBoxColliders()
        {
            return FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None).ToList();
        }

        public void DisableEnableLiveBoxColliders(bool enable)
        {
            GetActiveBoxColliders().ForEach(o =>
            {
                if (o.gameObject.tag != "dontDisableBoxCollider")
                {
                    o.enabled = enable;
                }
            });
        }

        public void ResetAnchorPoints(GameObject gameObject, Vector2 padding)
        {
            var panelRectTransform = gameObject.GetComponent<RectTransform>();
            if (panelRectTransform)
            {
                panelRectTransform.anchorMin = new Vector2(0, 0);
                panelRectTransform.anchorMax = new Vector2(1, 1);
                panelRectTransform.offsetMax = new Vector2(-padding.x, -padding.y);
                panelRectTransform.offsetMin = new Vector2(padding.x, padding.y); ;
                panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        public void SetCurrentCamera(Camera camera)
        {
            currentCamera = camera;
        }

        public int ReturnRandom(int maxNumber)
        {
            int rand = UnityEngine.Random.Range(0, maxNumber);
            return rand;
        }

        public FormationData GetCurrentPanelForRole(RoleEnum role)
        {
            var formations = SavedDataManager.SavedDataManagerInstance.persistentData.formations;
            if (formations != null && formations.Length > 0)
            {
                var formationArray = formations.ToList();

                var relevantFormation = formationArray
                    .Where(formation => (RoleEnum)Enum.Parse(typeof(RoleEnum), formation.Occupier) == role)
                    .FirstOrDefault();
                return relevantFormation;
            }
            return null;
        }

        public GameObject GetGameObjectFromFormation(FormationData formationData, List<PanelsManager> panels)
        {
            var relevantPanel = panels
                        .Where(panel => panel.panelNumber == formationData.PanelNumber &&
                            panel.voidZonesTypes == (voidZoneType)Enum.Parse(typeof(voidZoneType), formationData.VerticalFlag))
                        .FirstOrDefault();
            if (relevantPanel != null)
            {
                return relevantPanel.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Returns bool if float sent is great than or equal to random float generated, only send 1f
        /// </summary>
        /// <param name="chance"></param>
        /// <returns></returns>
        public bool GetChanceByPercentage(float chance)
        {
            chance = chance > 1.0f ? 1.0f : chance;
            var rand = UnityEngine.Random.Range(0.0f, 1.1f);
            return chance >= rand;
        }

        /*public Camera GetActiveCamera()
        {
            Camera camera = null;
            if (instance.SceneManager.currentScene.ToLower() == "exploration")
            {
                camera = ExploreManager.explorerCamera;
            }
            else if (instance.SceneManager.currentScene.ToLower() == "inventory")
            {
                camera = equipmentManager.equipmentCamera;
            }
            else if (instance.SceneManager.currentScene.ToLower() == "inventory")
            {
                camera = equipmentManager.equipmentCamera;
            }
            else
            {
                camera = Camera.main;
            }
            return camera;
        }*/

        // Use this for initialization
        void Start()
        {
            CursorControl();
            GenerateTutorialTexts();
            CheckPlayerReady();
            if (ShowTutorialText("NewGame"))
            {
                instance.gameMessanger.DisplayMessage(TutorialText["NewGame"], GlobalCanvas.transform, 0, "Welcome to the <b>Hydra Horn</b> Demo");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}