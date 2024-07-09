using AssemblyCSharp;
using Assets.scripts.Models.skillModels.swapSkills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;
#endif

namespace Assets.scripts.Helpers.Assets
{
    public class SkillFinder : MonoBehaviour
    {
        public List<EyeSkill> AllEyeSkills = new List<EyeSkill>();
        public List<SkillModel> AllClassSkills = new List<SkillModel>();

        #if UNITY_EDITOR
        public void GetAndAssignSkills()
        {
            AllEyeSkills = AssetFinder.GetEyeSkills();
            AllClassSkills = AssetFinder.GetAllSkills();
        }
        #endif

        public List<SkillModel> GetAllSkills(bool returnLearned)
        {
            return AllClassSkills.Where(skill => skill.learned == returnLearned).ToList();
        }

        public List<EyeSkill> GetAllEyeSkills(bool returnLearned)
        {
            return AllEyeSkills.Where(skill => skill.learned == returnLearned).ToList();
        }

        public EyeSkill GetEyeSkill(string skillName)
        {
            return AllEyeSkills.FirstOrDefault(skill => skill.skillName == skillName);
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(SkillFinder))]
    internal class SkillFinderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SkillFinder finder = (SkillFinder)target;
            if (GUILayout.Button("Update Skills"))
            { 
                finder.GetAndAssignSkills();
            }
        }
    }
    #endif
}
