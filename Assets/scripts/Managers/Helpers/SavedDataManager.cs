using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

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
                    _savedDataManagerInstance = UnityEngine.Object.FindObjectOfType<SavedDataManager>();

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

        public void SavePlayerHealth(int tankHealth, int dpsHealth, int healerHealth)
        {
            persistentData.playerData.tankHealth = tankHealth;
            persistentData.playerData.dpsHealth = dpsHealth;
            persistentData.playerData.healerHealth = healerHealth;
            SaveData();
        }

        public void SaveObtainedItem(string itemId)
        {
            //persistentData.dungeonData.items.Add(itemId);
            SaveData();
        }

        public void RemoveObtainedItem(string itemId)
        {
            //persistentData.dungeonData.items = persistentData.dungeonData.items.Where(o => o != itemId).ToList();
            SaveData();
        }

        public void SaveRoomsAndData(DungeonRoom room, List<RoomObject> roomObjects)
        {
            if (persistentData.dungeonData.allRooms == null)
            {
                persistentData.dungeonData.allRooms = new RoomData[] { };
            }

            var objectsData = roomObjects.Select(o =>
            {
                return new RoomObjectData
                {
                    position = o.position
                };
            }).ToArray();

            var routeData = room.routes.Select(o =>
            {
                return new RouteData
                {
                    name = o.name,
                    location = o.location,
                    position = o.position,
                    lockObject = o.lockObj
                };
            }).ToArray();

            persistentData.dungeonData.allRooms = persistentData.dungeonData.allRooms.Append(
                new RoomData(room, objectsData, routeData)
            ).ToArray();
            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
        }

        public void EditPreviousRoom(string roomName, bool add)
        {
            if (add)
            {
                persistentData.dungeonData.previousRooms = persistentData.dungeonData.previousRooms.Append(roomName).ToArray();
            } else
            {
                persistentData.dungeonData.previousRooms.ToList().Remove(roomName);
                persistentData.dungeonData.previousRooms.ToArray();
            }
            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
        }

        public void SaveCurrentRoomData(string currentRoomId)
        {
            persistentData.dungeonData.currentRoomId = currentRoomId;
            //persistentData.dungeonData.roomsVisited = persistentData.dungeonData.roomsVisited.Append(currentRoomId).ToArray();

            persistentData.dungeonData.allRooms = persistentData.dungeonData.allRooms.ToList().Select(v => {
                if (v.id == currentRoomId)
                {
                    v.visited = true;
                }
                return v;
            }).ToArray();

            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
        }

        public void SaveIconPos(List<miniMapIconBase> mapIcons)
        {
            var iconData = mapIcons.Select(o =>
            {
                return new IconData(o.label, (Vector2)o.gameObject.transform.position, o.gameObject.transform.rotation, (int)o.lineDirection);
            }).ToArray();
            persistentData.dungeonData.miniMapIconData = iconData;
            SavedDataManagerInstance.persistentData = persistentData;
            SaveData();
            //SaveData<IconData>(iconData, "testing");
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

        private void SetPaths(string fileName)
        {
            path = $"{Application.dataPath}{Path.AltDirectorySeparatorChar}{fileName}.json";
            persistentPath = $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}{fileName}.json";
        }

        public void SaveData<T>(List<T> data, string fileName)
        {
            SetPaths(fileName);
            string savePath = path;
            //Debug.Log($"Saving data to {path}");
            string json = JsonHelper.ToJson<T>(data.ToArray());
            FileStream fileStream = new FileStream(savePath, FileMode.Create);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
        }

        public void SaveData()
        {
            /*if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(persistentPath))
            {
                SetPaths("PersistentData");
            }*/
            SetPaths("PersistentData");
            string savePath = path;
            //Debug.Log($"Saving data to {path}");
            string json = JsonUtility.ToJson(persistentData);
            using (StreamWriter writer = new StreamWriter(savePath))
            {
                writer.Write(json);
            }
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
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                PersistentData data = JsonUtility.FromJson<PersistentData>(json);
                return data != null && data.playerData != null ? data.playerData : null;
            }
        }

        public void ResetDungeonData()
        {
            DungeonData dd = LoadDungeonData();
            dd = new DungeonData();
            persistentData.dungeonData = dd;
            SaveData();
        }

        public DungeonData LoadDungeonData()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                PersistentData data = JsonUtility.FromJson<PersistentData>(json);
                return data != null && data.dungeonData != null ? data.dungeonData : null;
            }
        }
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}