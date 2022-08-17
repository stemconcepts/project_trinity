using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssemblyCSharp
{
    public class AssetFinder : MonoBehaviour
    {
        private string[] skillFolders = new string[]{
            "Assets/scripts/Models/skillModels/tankSkills/ClassSkills",
            "Assets/scripts/Models/skillModels/dpsSkills/ClassSkills", 
            "Assets/scripts/Models/skillModels/healerSkills/ClassSkills"
        };

        private string[] weaponFolders = new string[] { 
            "Assets/scripts/Models/equipmentModels/dpsWeapons",
            "Assets/scripts/Models/equipmentModels/tankWeapons", 
            "Assets/scripts/Models/equipmentModels/healerWeapons" 
        };

        private string[] statusFolders = new string[]{
            "Assets/scripts/Models/statusModels/status"
        };

        private string[] equipmentFolders = new string[]{
            "Assets/scripts/Models/equipmentModels/baubles"
        };

        private string[] helperFolders = new string[]{
            "Assets/prefabs/status"
        };

        private string[] genericItems = new string[]{
            "Assets/prefabs/explorer/items/genericItems/"
        };

        public GameObject GetGameObjectFromPath(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab;
        }

        public List<SingleStatusModel> GetAllStatuses(){
            List<SingleStatusModel> statuses = new List<SingleStatusModel>();
            var statusList = AssetDatabase.FindAssets("t:SingleStatusModel",statusFolders);
            foreach (var s in statusList)
            {
                string item = AssetDatabase.GUIDToAssetPath(s);
                var status = AssetDatabase.LoadAssetAtPath<SingleStatusModel>( item );
                statuses.Add( status );
            } 
            return statuses;
        }

        public List<SkillModel> GetAllSkills(){
            List<SkillModel> skills = new List<SkillModel>();
            var skillsList = AssetDatabase.FindAssets("t:SkillModel",skillFolders);
            foreach (var skillItem in skillsList)
            {
                string item = AssetDatabase.GUIDToAssetPath(skillItem);
                var skill = AssetDatabase.LoadAssetAtPath<SkillModel>( item );
                if( skill.learned ){   
                    skills.Add( skill );
                }
            } 
            return skills;
        }

        public List<bauble> GetAllBaubles()
        {
            List<bauble> baubles = new List<bauble>();
            var baubleList = AssetDatabase.FindAssets("t:bauble", equipmentFolders);
            foreach (var baubleItem in baubleList)
            {
                string item = AssetDatabase.GUIDToAssetPath(baubleItem);
                var bauble = AssetDatabase.LoadAssetAtPath<bauble>(item);
                if (bauble.owned)
                {
                        baubles.Add(bauble);
                }
            }
            return baubles;
        }

        public List<weaponModel> GetAllWeapons()
        {
            List<weaponModel> weapons = new List<weaponModel>();
            var weaponList = AssetDatabase.FindAssets("t:weaponModel", weaponFolders);
            foreach (var weaponItem in weaponList)
            {
                string item = AssetDatabase.GUIDToAssetPath(weaponItem);
                var weapon = AssetDatabase.LoadAssetAtPath<weaponModel>(item);
                if (weapon.owned)
                {
                    weapons.Add(weapon);
                }
            }
            return weapons;
        }

        public List<GenericItem> GetGenericItem(string itemName)
        {
            List<GenericItem> items = new List<GenericItem>();
            var itemsList = AssetDatabase.FindAssets("t:ItemBase", genericItems);
            foreach (var i in itemsList)
            {
                string item = AssetDatabase.GUIDToAssetPath(i);
                var genericItem = AssetDatabase.LoadAssetAtPath<GenericItem>(item);
                if (genericItem.name == "itemName")
                {
                    items.Add(genericItem);
                }
            }
            return items;
        }
    }
}

