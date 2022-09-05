using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using System.Linq;

namespace AssemblyCSharp
{
    public class MainGameManager : MonoBehaviour
    {
        //[HideInInspector]
        bool showTutorial = true;
        [HideInInspector]
        public AssetFinder assetFinder;
        public sceneManager SceneManager;
        public Sound_Manager soundManager;
        public Canvas GlobalCanvas;
        public GameMessanger gameMessanger;
        public MainGameTaskManager taskManager;
        public static MainGameManager instance;
        Dictionary<string, string> TutorialText = new Dictionary<string, string>();
        public Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
        [HideInInspector]
        public Camera currentCamera;

        void Awake()
        {
            MakeSingleton();
        }

        List<int> GetStartingHealth() {
            PlayerData playerData = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
            if (playerData != null && playerData.tankHealth > 0 && playerData.dpsHealth > 0 && playerData.healerHealth > 0)
            {
                return new List<int> { playerData.tankHealth, playerData.dpsHealth, playerData.healerHealth };
            }
            return new List<int> {50, 40, 35};
        }

        public string GetText(string key)
        {
            return TutorialText[key];
        }

        void GenerateTutorialTexts()
        {
            TutorialText.Add("NewGame", "Press New Game to begin.");

            StringBuilder inventoryText = new StringBuilder("You must equip <b>Weapons</b> and a <b> Skill </b> before you start your journey.\n");
            inventoryText.Append("You can swap from the equipment view to the skill view by using the top navigation\n");
            inventoryText.Append("Hover over an item to view details and drag and drop an item to equip it");
            TutorialText.Add("Inventory", inventoryText.ToString());

            StringBuilder battleText = new StringBuilder("Use the abilities from your equipped weapons to survive and defeat the enemies.\n");
            battleText.Append("Each ability has an <b>Action Point</b> cost and cooldown.\n");
            battleText.Append("You can swap to your secondary weapon by selecting the large button in the middle.\n");
            battleText.Append("Drag and drop your characters in the desired panels to move them, this costs <b>1 Action Point</b> and expends the turn for that character");
            TutorialText.Add("Battle", battleText.ToString());
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
            if (SceneManager.TeamReady())
            {
                SavedDataManager.SavedDataManagerInstance.ResetDungeonData();
                SceneManager.LoadExploration(false);
            }
            else
            {
                SceneManager.LoadInventory(false);
            }
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
            var h = GetStartingHealth();
            SavedDataManager.SavedDataManagerInstance.SavePlayerHealth(h[0], h[1], h[2]);
        }

        public bool ShowTutorialText()
        {
            return showTutorial;
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
            return FindObjectsOfType<BoxCollider2D>().ToList();
        }

        public void DisableEnableLiveBoxColliders(bool enable)
        {
            GetActiveBoxColliders().ForEach(o =>
            {
                o.enabled = enable;
            });
        }

        public void SetCurrentCamera(Camera camera)
        {
            currentCamera = camera;
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
            GenerateTutorialTexts();
            CheckPlayerReady();
            if (ShowTutorialText())
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