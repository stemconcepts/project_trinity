using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class weaponItems : MonoBehaviour
    {
        //Get skill data
        //private itemData itemDataScript;
        //public List<itemData> itemList = new List<itemData>();
        public List<weaponModel> weapons = new List<weaponModel>();
        private string[] folders = new string[] { "Assets/scripts/Models/equipmentModels/dpsWeapons", "Assets/scripts/Models/equipmentModels/tankWeapons", "Assets/scripts/Models/equipmentModels/healerWeapons" };

        public void GetAllWeapons()
        {
            var weaponList = AssetDatabase.FindAssets("t:weapons", folders);
            foreach (var weaponItem in weaponList)
            {
                string item = AssetDatabase.GUIDToAssetPath(weaponItem);
                var weapon = AssetDatabase.LoadAssetAtPath<weaponModel>(item);
                if (weapon.owned)
                {
                    weapons.Add(weapon);
                }
            }
        }

        void Awake()
        {
            //GetAllWeapons();
        }
    }
}