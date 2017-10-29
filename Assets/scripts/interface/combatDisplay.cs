using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class combatDisplay : MonoBehaviour {

	//public GameObject characterObject;
	private character_data characterScript;
	private spawnUI spawnUIScript;

//	//damage text object
//	public GameObject textObject;
//	private Text displayText;
//
//	//heal text object
//	public GameObject textObjectheal;
//	private Text displayTextheal;

	//combat display
	public float displayDamageData;
	public float displayHealData;
	public float previousHealth;
	public float Health;

	public void getDmg( float dmgValue, string skillSource ){
		//Damage text
		//if( previousHealth != Health && previousHealth > Health ){
			//displayDamageData = (int)previousHealth - (int)Health;
		spawnUIScript.ShowDamageNumber( dmgValue, skillSource );
		previousHealth = Health;
		//} 
		Health = characterScript.Health;
	}

	public void getHeal( float healValue ){
		//Heal text
		//if( previousHealth != Health && previousHealth < Health ){
			//displayHealData =  (int)Health - (int)previousHealth;
		spawnUIScript.ShowHealNumber( healValue );
		previousHealth = Health;
		//} 
		Health = characterScript.Health;
	}

	public void getAbsorb( float absorbValue, string skillSource ){
		//Heal text
		//if( previousHealth != Health && previousHealth < Health ){
		//displayHealData =  (int)Health - (int)previousHealth;
		spawnUIScript.ShowAbsorbNumber( absorbValue, skillSource );
		previousHealth = Health;
		//} 
		Health = characterScript.Health;
	}

	public void Immune( string skillSource ){
		spawnUIScript.ShowImmune( skillSource );
	}

	// Use this for initialization
	void Awake () {

		//position
		//textObject.transform.localPosition = new Vector3(0, 0, 0);
		spawnUIScript = GetComponent<spawnUI>();
		characterScript = GetComponent<character_data>();
		Health = characterScript.Health;
		previousHealth = Health;
		//displayText = textObject.GetComponent<Text>();
		//displayTextheal = textObjectheal.GetComponent<Text>();

	}
	
	// Update is called once per frame
	void Update () {
		//getDmg();
	}
}
