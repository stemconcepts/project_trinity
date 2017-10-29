using UnityEngine;
using System.Collections;

public class collisions_counter : MonoBehaviour {

	void OnTriggerStay2D(Collider2D coll) {
		if (coll.gameObject.GetComponent<character_data>().role == "tank"){
			coll.gameObject.GetComponent<character_data>().inVoidCounter = true;
		}
	}
	//confirms that the player character has left the voidzone
	void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.GetComponent<character_data>().role == "tank"){
			coll.gameObject.GetComponent<character_data>().inVoidCounter = false;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
