using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class baubleItems : MonoBehaviour {
    public List<bauble> baubles = new List<bauble>();
    private string[] folders = new string[] {"Assets/scripts/gear/baubles"}; 

    public void GetAllBaubles(){
        var baubleList = AssetDatabase.FindAssets("t:bauble", folders);
        foreach (var baubleItem in baubleList)
        {
            string item = AssetDatabase.GUIDToAssetPath(baubleItem);
            var bauble = AssetDatabase.LoadAssetAtPath<bauble>( item );
            if( bauble.owned ){
                baubles.Add( bauble );
            }
        } 
    }

	// Use this for initialization
	void Awake () {
		//GetAllBaubles();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
