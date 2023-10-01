using AssemblyCSharp;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.scripts.Helpers.Utility.Testing
{
    [CustomEditor(typeof(fieldInventoryController))]
    internal class Tests : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            fieldInventoryController fi = (fieldInventoryController)target;
            if(GUILayout.Button("Add Field Item"))
            {
                fi.AddFieldItem();
            }
        }
    }
}
