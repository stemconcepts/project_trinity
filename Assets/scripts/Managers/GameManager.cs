using Assets.scripts.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.scripts.Helpers.Assets;

namespace AssemblyCSharp
{
#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
    [CustomEditor(typeof(GameManager))]
    internal class GameManagerGUITriggers : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GameManager gameManager = (GameManager)target;

            GUILayout.Space(30);
            //GUILayout.Label("Damage Value");

            //string damage = "";
            //damage = GUILayout.TextField(damage.ToString(), 4, "textfield");

            if (GUILayout.Button("Damage Player"))
            {
                gameManager.BattleManager.DamagePlayer(50, true);
            }

            if (GUILayout.Button("Damage All Friendly"))
            {
                gameManager.BattleManager.DamageAllFriendly(10, true);
            }

            if (GUILayout.Button("Damage All Enemy"))
            {
                gameManager.BattleManager.DamageAllEnemy(10, true);
            }
        }
    }
#endif

    public class GameManager : MonoBehaviour
    {
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
        public ExploreManagerV2 ExploreManager;
        public Task_Manager TaskManager;
        public Sound_Manager SoundManager;
        public BattleDetailsManager battleDetailsManager;
        public Game_Effects_Manager GameEffectsManager;
        public Event_Manager EventManager;
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
        }

        void Update()
        {

        }

        void Start()
        {
            if (State == GameState.Battle)
            {
                BattleManager.StartBattle(3f);
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

