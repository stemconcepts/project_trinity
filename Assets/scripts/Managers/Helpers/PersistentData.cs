using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class CharacterData
    {
        public weaponModel weapon;
        public weaponModel secondWeapon;
        public bauble bauble;
        public SkillModel classSkill;
    }

    [System.Serializable]
    public class RoomObjectData
    {
        public string name;
        public int position;
        public string[] items;
    }

    [System.Serializable]
    public class RouteData
    {
        public string name;
        public string location;
        public int position;
        public LockObject lockObject;
    }

    [System.Serializable]
    public class RoomData
    {
        public string id;
        public string name;
        public bool isDetour, isCustomRoom, isStartingRoom;
        public bool visited;
        public string encounterId;
        public string parentRoomName;
        public RoomObjectData[] roomObjects;
        public string[] routeLocations;
        public RouteData[] routes;
        //public RoomData(string id, string name, bool visited, bool isDetour, bool isCustomRoom, bool isStartingRoom, string parentRoomId, RoomObjectData[] roomObjects, RouteData[] routes)
        public RoomData(DungeonRoom room, RoomObjectData[] roomObjects, RouteData[] routes)
        {
            this.id = room.id;
            this.name = room.gameObject.name;
            this.visited = room.visited;
            this.parentRoomName = room.parentRoom ? room.parentRoom.gameObject.name : "";
            this.isDetour = room.isDetour;
            this.isCustomRoom = room.isCustomRoom;
            this.isStartingRoom = room.isStartingRoom;
            this.roomObjects = roomObjects;
            this.routes = routes;
            this.routeLocations = room.routeLocations.ToArray();
        }
    }

    [System.Serializable]
    public class IconData
    {
        public string label;
        public Vector2 position;
        public Quaternion rotation;
        public int direction;
        public IconData(string label, Vector2 position, Quaternion rotation, int direction)
        {
            this.label = label;
            this.position = position;
            this.rotation = rotation;
            this.direction = direction;
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public int tankHealth;
        public int tankMaxHealth;
        public CharacterData tankEquipment;
        public int dpsHealth;
        public int dpsMaxHealth;
        public CharacterData dpsEquipment;
        public int healerHealth;
        public int healerMaxHealth;
        public CharacterData healerEquipment;

        public PlayerData()
        {
            this.tankEquipment = new CharacterData();
            this.dpsEquipment = new CharacterData();
            this.healerEquipment = new CharacterData();
        }

        public PlayerData(CharacterData tankEquipment, CharacterData dpsEquipment, CharacterData healerEquipment)
        {
            this.tankEquipment = tankEquipment;
            this.dpsEquipment = dpsEquipment;
            this.healerEquipment = healerEquipment;
        }
    }

    [System.Serializable]
    public class DungeonData
    {
        public string currentRoomId;
        public string[] previousRooms;
        public string[] items;
        public string[] enemysKilled;
        public IconData[] miniMapIconData;
        public RoomData[] allRooms;

        public DungeonData()
        {
            currentRoomId = "";
            this.previousRooms = new string[] { };
            this.items = new string[] { };
            this.enemysKilled = new string[] { };
        }

        public DungeonData(string roomName)
        {
            //this.currentRoomId = roomName;
            //this.roomsVisited = new List<string>();
            //this.items = new List<string>();
            //this.enemysKilled = new List<string>();
        }
    }

    [System.Serializable]
    public class PersistentData
    {
        public PlayerData playerData;
        public DungeonData dungeonData;

        public PersistentData()
        {
            this.playerData = new PlayerData();
            this.dungeonData = new DungeonData();
        }
    }
}