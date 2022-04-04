using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class SavedDataManager : MonoBehaviour
    {
        //private sceneManager sceneManager;
        public static SavedDataManager _savedDataManagerInstance;
        public PersistentData persistentData = new PersistentData();
        private string path = "";
        private string persistentPath = "";

        public static SavedDataManager SavedDataManagerInstance
        {
			get
			{
				if (_savedDataManagerInstance == null)
				{
                    _savedDataManagerInstance = Object.FindObjectOfType<SavedDataManager>();

					//Don't destroy on load
					if (_savedDataManagerInstance != null)
					{
						DontDestroyOnLoad(_savedDataManagerInstance.gameObject);
					}
				}

				return _savedDataManagerInstance;
			}
		}

        // Use this for initialization
        void Start()
        {
            SetPaths();
            PlayersReady();
        }

        void Awake()
        {
            //sceneManager = GetComponent<sceneManager>();

            if (_savedDataManagerInstance == null)
			{
                //If this is the first instance then make this the singleton
                _savedDataManagerInstance = this;
				DontDestroyOnLoad(this);
			}
			else
			{
				//A singleton already exists and it is not this
				if (this != _savedDataManagerInstance)
					Destroy(this.gameObject);
			}
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddWeaponModel(weaponModel weaponModel, bool mainWeapon)
        {
            switch (weaponModel.type)
            {
                case weaponModel.weaponType.bladeAndBoard: case weaponModel.weaponType.heavyHanded:
                        if (mainWeapon)
                        {
                            persistentData.playerData.tankEquipment.weapon = weaponModel;
                        } else
                        {
                            persistentData.playerData.tankEquipment.secondWeapon = weaponModel;
                        }
                    break;
                case weaponModel.weaponType.clawAndCannon: case weaponModel.weaponType.dualBlades:
                    if (mainWeapon)
                    {
                        persistentData.playerData.dpsEquipment.weapon = weaponModel;
                    }
                    else
                    {
                        persistentData.playerData.dpsEquipment.secondWeapon = weaponModel;
                    }
                    break;
                case weaponModel.weaponType.cursedGlove: case weaponModel.weaponType.glove:
                    if (mainWeapon)
                    {
                        persistentData.playerData.healerEquipment.weapon = weaponModel;
                    }
                    else
                    {
                        persistentData.playerData.healerEquipment.secondWeapon = weaponModel;
                    }
                    break;
                default:
                    break;
            }
            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
            PlayersReady();
        }

        public void AddBauble(bauble bauble, string character)
        {
            switch (character)
            {
                case "guardian":
                        persistentData.playerData.tankEquipment.bauble = bauble;
                    break;
                case "walker":
                    persistentData.playerData.healerEquipment.bauble = bauble;
                    break;
                case "stalker":
                    persistentData.playerData.dpsEquipment.bauble = bauble;
                    break;
            }
            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
            PlayersReady();
        }

        public void AddSkill(SkillModel skill, string character)
        {
            switch (character)
            {
                case "guardian":
                    persistentData.playerData.tankEquipment.classSkill = skill;
                    break;
                case "walker":
                    persistentData.playerData.healerEquipment.classSkill = skill;
                    break;
                case "stalker":
                    persistentData.playerData.dpsEquipment.classSkill = skill;
                    break;
            }
            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
            PlayersReady();
        }

        private void SetPaths()
        {
            path = $"{Application.dataPath}{Path.AltDirectorySeparatorChar}PersistentData.json";
            persistentPath = $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}PersistentData.json";
        }

        public void SaveData()
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(persistentPath))
            {
                SetPaths();
            }
            string savePath = path;
            Debug.Log($"Saving data to {path}");
            string json = JsonUtility.ToJson(persistentData);
            using StreamWriter writer = new StreamWriter(savePath);
            writer.Write(json);
        }

        private void PlayersReady()
        {
            PlayerData playerData = LoadPlayerData();
            if(playerData != null)
            {
                persistentData.playerData = playerData;
                SavedDataManagerInstance.persistentData.playerData = playerData;
                MainGameManager.instance.SceneManager.healerReady = playerData.healerEquipment.weapon && playerData.healerEquipment.secondWeapon;
                MainGameManager.instance.SceneManager.dpsReady = playerData.dpsEquipment.weapon && playerData.dpsEquipment.secondWeapon;
                MainGameManager.instance.SceneManager.tankReady = playerData.tankEquipment.weapon && playerData.tankEquipment.secondWeapon;
            }
        }

        public PlayerData LoadPlayerData()
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(persistentPath))
            {
                SetPaths();
            }
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            PersistentData data = JsonUtility.FromJson<PersistentData>(json);
            return data != null && data.playerData != null ? data.playerData : null;
        }
    }
}