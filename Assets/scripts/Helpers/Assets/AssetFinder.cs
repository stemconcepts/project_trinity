using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.scripts.Models.skillModels.swapSkills;
using System.Linq;

#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;

namespace AssemblyCSharp
{
    public static class AssetFinder
    {
        private static string[] skillFolders = new string[]{
            "Assets/scripts/Models/skillModels/tankSkills/ClassSkills",
            "Assets/scripts/Models/skillModels/dpsSkills/ClassSkills", 
            "Assets/scripts/Models/skillModels/healerSkills/ClassSkills"
        };

        private static string[] eyeSkillsFolders = new string[]{
            "Assets/scripts/Models/skillModels/eyeSkills/Offensive",
            "Assets/scripts/Models/skillModels/eyeSkills/Defensive",
            "Assets/scripts/Models/skillModels/eyeSkills/Mixed"
        };

        private static string[] weaponFolders = new string[] { 
            "Assets/scripts/Models/equipmentModels/dpsWeapons",
            "Assets/scripts/Models/equipmentModels/tankWeapons", 
            "Assets/scripts/Models/equipmentModels/healerWeapons" 
        };

        private static string[] statusFolders = new string[]{
            "Assets/scripts/Models/statusModels/status"
        };

        private static string[] equipmentFolders = new string[]{
            "Assets/scripts/Models/equipmentModels/baubles"
        };

        private static string[] helperFolders = new string[]{
            "Assets/prefabs/status"
        };

        private static string[] genericItems = new string[]{
            "Assets/prefabs/explorer/items/genericItems/"
        };

        public static GameObject GetGameObjectFromPath(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab;
        }

        public static List<SingleStatusModel> GetAllStatuses(){
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

        public static List<EyeSkill> GetEyeSkills()
        {
            List<EyeSkill> skills = new List<EyeSkill>();
            var skillsList = AssetDatabase.FindAssets("t:EyeSkill", eyeSkillsFolders);
            foreach (var skillItem in skillsList)
            {
                string item = AssetDatabase.GUIDToAssetPath(skillItem);
                var skill = AssetDatabase.LoadAssetAtPath<EyeSkill>(item);
                skills.Add(skill);
            }
            return skills;
        }

        public static List<SkillModel> GetAllSkills(){
            List<SkillModel> skills = new List<SkillModel>();
            var skillsList = AssetDatabase.FindAssets("t:SkillModel",skillFolders);
            foreach (var skillItem in skillsList)
            {
                string item = AssetDatabase.GUIDToAssetPath(skillItem);
                var skill = AssetDatabase.LoadAssetAtPath<SkillModel>( item );
                skills.Add(skill);
            } 
            return skills;
        }

        public static List<Bauble> GetAllBaubles()
        {
            List<Bauble> baubles = new List<Bauble>();
            var baubleList = AssetDatabase.FindAssets("t:bauble", equipmentFolders);
            foreach (var baubleItem in baubleList)
            {
                string item = AssetDatabase.GUIDToAssetPath(baubleItem);
                var bauble = AssetDatabase.LoadAssetAtPath<Bauble>(item);
                baubles.Add(bauble);
            }
            return baubles;
        }

        public static List<WeaponModel> GetAllWeapons()
        {
            List<WeaponModel> weapons = new List<WeaponModel>();
            var weaponList = AssetDatabase.FindAssets("t:weaponModel", weaponFolders);
            foreach (var weaponItem in weaponList)
            {
                string item = AssetDatabase.GUIDToAssetPath(weaponItem);
                var weapon = AssetDatabase.LoadAssetAtPath<WeaponModel>(item);
                weapons.Add(weapon);
            }
            return weapons;
        }

        public static List<GenericItem> GetAllGenericItems()
        {
            List<GenericItem> items = new List<GenericItem>();
            var itemsList = AssetDatabase.FindAssets("t:ItemBase", genericItems);
            foreach (var i in itemsList)
            {
                string item = AssetDatabase.GUIDToAssetPath(i);
                var genericItem = AssetDatabase.LoadAssetAtPath<GenericItem>(item);
                if (genericItem?.name == "itemName")
                {
                    items.Add(genericItem);
                }
            }
            return items;
        }
    }
}

#endif