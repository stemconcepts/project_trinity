using UnityEngine;
using System.Collections;

public class enemyskill_cd : MonoBehaviour {

	private enemyskill_confirm enemyskillConfirmScript;

	//enemy skill cooldowns
//	public float deadGrasp = 15f;
//	public float wideSwing = 30f;
//	public float unholyArmor = 20f;
//	public float unholyStrength = 20f;
//
//	//enemy cast times
//	public float wideswingCT = 4f;
	
	//CD timer
	public void RefreshSkill( bool skillnameactive, float skillnamecd, string skilllabel ){
		print ( "Refreshing " + skilllabel);
		if( skillnameactive == false ){
			CDtime( skillnamecd, skilllabel );
		}
	}

	void CDtime( float cdtime, string skillcdname ){
		StartCoroutine ( cdtimer( cdtime, skillcdname ) );
	}
	
	IEnumerator cdtimer(float waitTime, string skillcdname)
	{
		yield return new WaitForSeconds(waitTime);
		if( skillcdname == "Wide Swing" ){
			enemyskillConfirmScript.wideswingActive = true;
		} else if ( skillcdname == "Dead Grasp" ){
			enemyskillConfirmScript.deadgraspActive = true;
		} else if ( skillcdname == "Unholy Armor" ){
			enemyskillConfirmScript.unholyarmorActive = true;
		}
	}

	// Use this for initialization
	void Awake () {
		enemyskillConfirmScript = GetComponent<enemyskill_confirm>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
