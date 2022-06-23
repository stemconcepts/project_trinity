using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class GameManager : MonoBehaviour
    {
        //public List<string> FriendlyCharacters = new List<string>();
        //public List<string> EmenyCharacters = new List<string>();
        string Backgrounds;
        public enum GameState
        {
            Inventory,
            Exploration,
            Battle,
            MainMenu
        };
        public GameState State;
        public BattleManager BattleManager;
        public InventoryManager InventoryManager;
        public ExploreManager ExploreManager;
        public Task_Manager TaskManager;
        public Sound_Manager SoundManager;
        public BattleDetailsManager battleDetailsManager;
        public Game_Effects_Manager GameEffectsManager;
        public Event_Manager EventManager;
        //public sceneManager SceneManager;
        public AssetFinder AssetFinder;
        //public SavedDataManager SavedDataManager;
        public Character_Select_Manager characterSelectManager;
        public static ILogger logger = Debug.unityLogger;
        public Camera camera;
        public Vector3 dragOrigin;
        Vector3 lastMousePosition = new Vector3();

        //int rand;

        void Awake()
        {
            SoundManager = gameObject.GetComponent<Sound_Manager>();
            TaskManager = gameObject.GetComponent<Task_Manager>();
            battleDetailsManager = gameObject.GetComponent<BattleDetailsManager>();
            characterSelectManager = gameObject.GetComponent<Character_Select_Manager>();
            GameEffectsManager = gameObject.GetComponent<Game_Effects_Manager>();
            EventManager = gameObject.GetComponent<Event_Manager>();
            BattleManager = gameObject.GetComponent<BattleManager>();
            AssetFinder = gameObject.GetComponent<AssetFinder>();
            /*var dataInstance = GameObject.Find("DataInstance");
            if (dataInstance)
            {
                SceneManager = dataInstance.GetComponent<sceneManager>();
                SavedDataManager = dataInstance.GetComponent<SavedDataManager>();
            }*/
        }

        void Update()
        {

        }

        void Start()
        {
            if (State == GameState.Battle)
            {
                BattleManager.StartBattle(5f);
            }
        }

        public bool GetChance(int maxChance)
        {
            var rand = UnityEngine.Random.Range(0, (maxChance + 1));
            return rand == 0;
        }

        public static bool GetChanceByPercentage(float chance)
        {
            var rand = UnityEngine.Random.Range(0.0f, 1.1f);
            return chance >= rand;
        }

        public void SetDragOrigin()
        {
            dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void DragObject(Transform objectTransform)
        {
            if (lastMousePosition != camera.ScreenToWorldPoint(Input.mousePosition))
            {
                Vector3 newMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
                lastMousePosition = newMousePos;
                Vector3 difference = dragOrigin - newMousePos;
                objectTransform.position -= difference;
                var a = objectTransform.gameObject.GetComponent<RectTransform>();
                a.anchoredPosition3D = new Vector3(a.anchoredPosition3D.x, a.anchoredPosition3D.y, Input.mousePosition.z);
                SetDragOrigin();
            }
        }

        public int ReturnRandom(int maxNumber)
        {
            var rand = UnityEngine.Random.Range(0, maxNumber);
            return rand;
        }

        public void AddGearToInventory(ItemBase gearItem)
        {
            switch (gearItem.GetType().Name)
            {
                case nameof(baubleItem):
                    ((baubleItem)gearItem).bauble.owned = true;
                    break;
                case nameof(weaponItem):
                    ((weaponItem)gearItem).weapon.owned = true;
                    break;
                default:
                    break;
            }
            //SavedDataManager.SavedDataManagerInstance.SaveObtainedItem(gearItem.id);
        }
    }
}

