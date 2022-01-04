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
    public class PlayerData
    {
        public CharacterData tankEquipment;
        public CharacterData dpsEquipment;
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

    public class DungeonData
    {
        public string roomName { get; set; }

        public DungeonData()
        {
        }

        public DungeonData(string roomName)
        {
            this.roomName = roomName;
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

        /*PersistentData(PlayerData playerData, DungeonData dungeonData)
        {
            this.playerData = playerData;
            this.dungeonData = dungeonData;
        }*/
    }
}