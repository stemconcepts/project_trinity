using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class Base_Generic_Manager : MonoBehaviour
    {
        public BattleManager BattleManager;
        public InventoryManager InventoryManager;
        public ExploreManager ExploreManager;
        public Task_Manager TaskManager;
        public Sound_Manager SoundManager;
        public BattleDetailsManager battleDetailsManager;
        public Game_Effects_Manager GameEffectsManager;
        public Event_Manager EventManager;
        public sceneManager SceneManager;
        public AssetFinder AssetFinder;
        public SavedDataManager SavedDataManager;
        //public SavedDataManager SavedDataManager;
        public Character_Select_Manager characterSelectManager;
        public static ILogger logger = Debug.unityLogger;
        //public Camera Camera;

        void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            //Camera = transform.Find("Main Camera").gameObject.GetComponent<Camera>();
            SoundManager = gameObject.GetComponent<Sound_Manager>();
            TaskManager = gameObject.GetComponent<Task_Manager>();
            battleDetailsManager = gameObject.GetComponent<BattleDetailsManager>();
            characterSelectManager = gameObject.GetComponent<Character_Select_Manager>();
            GameEffectsManager = gameObject.GetComponent<Game_Effects_Manager>();
            EventManager = gameObject.GetComponent<Event_Manager>();
            BattleManager = gameObject.GetComponent<BattleManager>();
            AssetFinder = gameObject.GetComponent<AssetFinder>();
            SceneManager = gameObject.GetComponent<sceneManager>();
            //SavedDataManager = gameObject.GetComponent<SavedDataManager>();
        }
    }
}
