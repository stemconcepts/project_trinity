using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class sceneControl : MonoBehaviour {
	public GameObject menuManager;
	equipmentManager equipManagerScript;
	public bool tankReady = false;
	public bool healerReady = false;
	public bool dpsReady = false;

	public void LoadBattle(){
		if( tankReady && healerReady && dpsReady ){
			//Application.LoadLevel("mockupbattle");
            SceneManager.LoadScene("mockupbattle", LoadSceneMode.Single);
		} else if( tankReady == false ){
			print ("Please equip a weapon and/or skill to the Guardian");
		} else if( healerReady == false ){
			print ("Please equip a weapon and/or skill to the Walker");
		} else if( dpsReady == false ){
			print ("Please equip a weapon and/or skill to the Stalker");
		}
	}

	// Use this for initialization
	void Awake () {
		equipManagerScript = menuManager.GetComponent<equipmentManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
