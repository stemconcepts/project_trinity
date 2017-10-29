using UnityEngine;
using System.Collections;

public class collisions : MonoBehaviour {

	//Void zone character confirmation
	//confirms that the player character is within the voidzone
	void OnTriggerStay2D(Collider2D coll) {
		if (coll.gameObject.GetComponent<character_data>().characterType == "friendly"){
			coll.gameObject.GetComponent<character_data>().inVoidZone = true;
		}
	}
	//confirms that the player character has left the voidzone
	void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.GetComponent<character_data>().characterType == "friendly"){
			coll.gameObject.GetComponent<character_data>().inVoidZone = false;
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
