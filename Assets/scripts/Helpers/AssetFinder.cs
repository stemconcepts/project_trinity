using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

        public AssetFinder()
        {
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
    }
}

