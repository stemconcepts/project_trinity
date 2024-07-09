using AssemblyCSharp;
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
    public class ItemFinder : MonoBehaviour
    {
        public List<GenericItem> AllGenericItems = new List<GenericItem>();
        public GameObject ExplorerItem;
        public GameObject MinionDataItem;
        public GameObject ToolTipItem;

        #if UNITY_EDITOR
        public void GetAndAssignItems()
        {
            AllGenericItems = AssetFinder.GetAllGenericItems();
        }

        public void GetAndAssignPrefabs()
        {
            ExplorerItem = AssetFinder.GetGameObjectFromPath("Assets/prefabs/explorer/items/explorerItem.prefab");
            MinionDataItem = AssetFinder.GetGameObjectFromPath("Assets/prefabs/combatInfo/character_info/singleMinionData.prefab");
            ToolTipItem = AssetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/tooltipHoverObject.prefab");
        }
        #endif
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ItemFinder))]
    internal class ItemFinderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ItemFinder finder = (ItemFinder)target;
            if (GUILayout.Button("Update Generic Items"))
            {
                finder.GetAndAssignItems();
            }

            if (GUILayout.Button("Update Prefabs"))
            {
                finder.GetAndAssignPrefabs();
            }
        }
    }
    #endif
}
