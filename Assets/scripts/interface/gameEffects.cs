using UnityEngine;
using System.Collections;

public class gameEffects : MonoBehaviour {
	private Task SloMoTask;
	Vector3 originalCameraPosition;
	float shakeAmt = 0;
	private Camera mainCamera;
	private Task CancelShake;

	//Slowmotion
	public void SlowMo( float slowAmount ){
		Time.timeScale = slowAmount;
		SloMoTask = new Task( SlowMoFade( 1f ) );
	}

	IEnumerator SlowMoFade(float waitTime ) {
		while( Time.timeScale < 1f ){
			Time.timeScale += 0.001f;
			yield return null;
		}
		Time.timeScale = 1f;
	}

	//ScreenShake
	public void ScreenShake( float shakeAmt ){
		float quakeAmt = Random.value*shakeAmt*2 - shakeAmt;
		Vector3 pp = mainCamera.transform.position;
		pp.y+= quakeAmt; // can also add to x and/or z
		mainCamera.transform.position = pp;
		CancelShake = new Task( StopShaking( 1f ) );
		//print ("shake start" + quakeAmt );
	}
		
	IEnumerator StopShaking( float waitTime ){
		yield return null;
		mainCamera.transform.position = originalCameraPosition;
	}
	

	// Use this for initialization
	void Awake () {
		mainCamera = GetComponent<Camera>();
		originalCameraPosition = mainCamera.GetComponent<Transform>().position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
