using AssemblyCSharp;
using Assets.scripts.Managers.ExplorerScene_Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;

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

    [CustomEditor(typeof(ExplorerItemsController))]
    internal class InspectorGUITriggers : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ExplorerItemsController explorerController = (ExplorerItemsController)target;
            if (GUILayout.Button("Trigger SetUp"))
            {
                explorerController.TriggerSetUp();
            }
        }
    }
}

#endif