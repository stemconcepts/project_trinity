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
    public class StatusFinder : MonoBehaviour
    {
        public List<SingleStatusModel> AllStatuses = new List<SingleStatusModel>();

        #if UNITY_EDITOR
        public void GetAndAssignStatuses()
        {
            AllStatuses = AssetFinder.GetAllStatuses();
        }
        #endif
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(StatusFinder))]
    internal class StatusFinderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            StatusFinder finder = (StatusFinder)target;
            if (GUILayout.Button("Update Statuses"))
            {
                finder.GetAndAssignStatuses();
            }
        }
    }
    #endif
}
