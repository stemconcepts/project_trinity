using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class skillItems : MonoBehaviour {
	public List<classSkills> skills = new List<classSkills>();
    private string[] folders = new string[] {"Assets/scripts/skills/mainSkills/tankSkills/ClassSkills", "Assets/scripts/skills/mainSkills/dpsSkills/ClassSkills", "Assets/scripts/skills/mainSkills/healerSkills/ClassSkills"}; 

    public void GetAllSkills(){
        var skillsList = AssetDatabase.FindAssets("t:classSkills", folders);
        foreach (var skillItem in skillsList)
        {
            string item = AssetDatabase.GUIDToAssetPath(skillItem);
            var skill = AssetDatabase.LoadAssetAtPath<classSkills>( item );
            if( skill.learned ){   
                skills.Add( skill );
            }
        } 
    }
}
