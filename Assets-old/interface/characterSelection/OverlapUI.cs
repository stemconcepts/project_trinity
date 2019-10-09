using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OverlapUI : MonoBehaviour {
	public List<GameObject> overlappedObjects = new List<GameObject>();
	public GameObject charBtnPrefab;	

	public void DisplayCharChoice( GameObject CharSelectUI ){
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
		for( int x = 0; x < overlappedObjects.Count; x++ ){
			var charObj = (GameObject)Instantiate( charBtnPrefab );
			charObj.GetComponent<charBtnControl>().charObj = overlappedObjects[x].gameObject;
			charObj.transform.SetParent( CharSelectUI.transform );
			charObj.transform.Find("charIcon").GetComponent<Image>().sprite = overlappedObjects[x].GetComponent<character_data>().characterIcon; 
		}	
	}

	void Start(){
		
				//print( rayPoint );
		//this.gameObject.transform.position = new Vector3( ray.GetPoint( distance ).x, ray.GetPoint( distance ).y, ray.GetPoint( distance ).z );
	}
}
