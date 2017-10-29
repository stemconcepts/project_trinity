using UnityEngine;
using System.Collections;

public class selectionOverlapControl : MonoBehaviour {
	public GameObject overlappedObj;

	void OnTriggerStay2D(Collider2D coll) {
		var charObj = coll.gameObject.GetComponent<character_data>();
		if( charObj != null && charObj.characterType == "friendly" ){
			overlappedObj = coll.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D coll) {
		overlappedObj = null;
	}

	public void BuildOverlapList( GameObject CharSelectUI ){
		var overlapUIScript = CharSelectUI.GetComponent<OverlapUI>();
		overlapUIScript.overlappedObjects.Clear();
		overlapUIScript.overlappedObjects.Add( this.gameObject );
		overlapUIScript.overlappedObjects.Add( overlappedObj );
		overlapUIScript.DisplayCharChoice( CharSelectUI );
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
