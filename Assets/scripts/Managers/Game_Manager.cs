using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Game_Manager : MonoBehaviour
    {
        //public List<string> FriendlyCharacters = new List<string>();
        //public List<string> EmenyCharacters = new List<string>();
        string Backgrounds;        
        public enum GameState {
           Inventory,
           Exploration,
           Battle  
        };
        public GameState State;
        public Battle_Manager BattleManager;
        public Inventory_Manager InventoryManager;
        public Explore_Manager ExploreManager;
        public Task_Manager TaskManager;
        public Sound_Manager SoundManager;
        public Battle_Details_Manager battleDetailsManager;
        public Game_Effects_Manager GameEffectsManager;
        public Event_Manager EventManager;
        public sceneManager SceneManager;
        public AssetFinder AssetFinder;
        public SavedDataManager SavedDataManager;
        public Character_Select_Manager characterSelectManager;
        public static ILogger logger = Debug.unityLogger;
        public Camera camera;
        public Vector3 dragOrigin;
        Vector3 lastMousePosition = new Vector3();

        int rand;

        void Awake(){
            SoundManager = gameObject.GetComponent<Sound_Manager>();
            TaskManager = gameObject.GetComponent<Task_Manager>();
            battleDetailsManager = gameObject.GetComponent<Battle_Details_Manager>();
            characterSelectManager = gameObject.GetComponent<Character_Select_Manager>();
            GameEffectsManager = gameObject.GetComponent<Game_Effects_Manager>();
            EventManager = gameObject.GetComponent<Event_Manager>();
            BattleManager = gameObject.GetComponent<Battle_Manager>();
            AssetFinder = gameObject.GetComponent<AssetFinder>();
            SceneManager = gameObject.GetComponent<sceneManager>();
            SavedDataManager = gameObject.GetComponent<SavedDataManager>();
            //camera = transform.Find("Main Camera").gameObject.GetComponent<Camera>();
        }

        void Update()
        {

        }

        void Start(){
            if( State == GameState.Battle ){
                BattleManager.StartBattle(5f);
            }
        }

        public bool GetChance(int maxChance)
        {
            rand = UnityEngine.Random.Range(0, (maxChance + 1));
            return rand == 1;
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
            rand = UnityEngine.Random.Range(0, maxNumber);
            return rand;
        }
    }
}

