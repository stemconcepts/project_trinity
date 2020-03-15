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
            "Assets/scripts/skills/mainSkills/tankSkills/ClassSkills", 
            "Assets/scripts/skills/mainSkills/dpsSkills/ClassSkills", 
            "Assets/scripts/skills/mainSkills/healerSkills/ClassSkills"
        }; 

        private string[] statusFolders = new string[]{
            "Assets/scripts/Models/statusModels/status"
        };

        private string[] equipmentFolders = new string[]{
            "Assets/scripts/gear/baubles"
        };

        public AssetFinder()
        {
        }

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
    }
}

