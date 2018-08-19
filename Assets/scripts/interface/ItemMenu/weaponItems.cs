using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class weaponItems : MonoBehaviour {
	//Get skill data
	//private itemData itemDataScript;
	//public List<itemData> itemList = new List<itemData>();
	public List<weapons> weapons = new List<weapons>(); 
    private string[] folders = new string[] {"Assets/scripts/gear/dpsWeapons", "Assets/scripts/gear/tankWeapons", "Assets/scripts/gear/healerWeapons"}; 

    public void GetAllWeapons(){
        var weaponList = AssetDatabase.FindAssets("t:weapons", folders);
        foreach (var weaponItem in weaponList)
        {
            string item = AssetDatabase.GUIDToAssetPath(weaponItem);
            var weapon = AssetDatabase.LoadAssetAtPath<weapons>( item );
            if( weapon.owned ){   
                weapons.Add( weapon );
            }
        } 
    }

    void Awake( ){
        //GetAllWeapons();
    }
}
