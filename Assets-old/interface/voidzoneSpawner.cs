using UnityEngine;
using System.Collections;

public class voidzoneSpawner : MonoBehaviour {
	public GameObject voidzone_object;
	//public GameObject voidzone_curve;
	public GameObject voidzone;
	public Transform spawnposition;

//	public void ShowVoidZone( float xposition, float ypositionplayer, Quaternion rotation, float duration ){
//		Vector3 spawnpoint = transform.position;
//		if( spawnposition ){
//			spawnpoint = spawnposition.position;
//		}
//		voidzone_object = (GameObject)Instantiate( voidzone, spawnpoint , Quaternion.identity );
//		//voidzone_object.transform.SetParent( transform, true );
//		Vector3 newposition = voidzone_object.transform.position;
//		newposition.z = 0f;
//		voidzone_object.transform.position = newposition;
//
//		StartCoroutine( DestroyVoidzoneTimer( duration, voidzone_object ) );
//	}


	IEnumerator DestroyVoidzoneTimer( float waitTime, GameObject voidzone ){
		yield return new WaitForSeconds(waitTime);
		Destroy(voidzone);
	}

	public void DestroyVoidzone(){
		Destroy(voidzone_object);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
