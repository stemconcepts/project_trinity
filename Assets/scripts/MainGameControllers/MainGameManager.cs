using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class MainGameManager : MonoBehaviour
    {
        public AssetFinder assetFinder;
        public sceneManager SceneManager;
        public static MainGameManager instance;

        void Awake()
        {
            MakeSingleton();
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
            if (SceneManager.tankReady && SceneManager.dpsReady && SceneManager.healerReady)
            {
                SceneManager.LoadExploration(false);
            }
            else
            {
                SceneManager.LoadInventory(false);
            }
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

        // Use this for initialization
        void Start()
        {
            CheckPlayerReady();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}