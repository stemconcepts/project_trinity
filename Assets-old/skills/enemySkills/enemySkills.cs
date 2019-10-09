using UnityEngine;
using System.Collections;

[System.Serializable]
public class voidzoneData{
	public float voidZoneDuration;
	//public GameObject target;
}

public class enemySkills : MonoBehaviour {

	public character_data characterScript;
	private enemyskill_cd enemyskillCdScript;
	private GameObject[] randomPlayer;
	private status statusScript;
	private enemyskill_confirm enemyskillConfirmScript;
	private enemySkillSelection enemyskillSelectionScript;
	//public voidzoneSpawner voidSpawnScript;

//	//cast timer
//	void CastTime( float casttime, string skillname ){
//		StartCoroutine ( casttimer( casttime, skillname ) );
//	}
//
//	IEnumerator casttimer(float waitTime, string skillname)
//	{
//		yield return new WaitForSeconds(waitTime);
//		if( skillname == "Wide Swing" ){
//			WideSwingStart();
//		} else if( skillname == "Unholy Armor" ){
//			UnholyArmorStart();
//		} else if( skillname == "Dead Grasp" ){
//			DeadGraspStart();
//		}
//	}
//	
//	//Dead Grasp
//	public void DeadGrasp(){;
//		CastTime ( 1f, "Dead Grasp");
//		//DeadGraspStart();
//	}
//
//	public void DeadGraspStart(){
//		randomPlayer = GameObject.FindGameObjectsWithTag("Player"); 
//		var randomNumber = Random.Range (1,3);
//		var currentTarget = randomPlayer[randomNumber];
//		randomPlayer[randomNumber].GetComponent<status>().StunOn( 3, false, 5 );
//		//currentTarget.GetComponent<status>.StunOn();
//		print ( randomPlayer[randomNumber].GetComponent<character_data>().objectName + " takes Dead Grasp" );
//		skillinprogress = true;
//		SkillInProgress();
//		//checkdeadgraspactive = false;
//		enemyskillConfirmScript.deadgraspActive = false;
//		enemyskillCdScript.RefreshSkill( enemyskillConfirmScript.deadgraspActive, enemyskillCdScript.deadGrasp, "Dead Grasp" );
//	}
//
//	//Unholy Armor
//	public void UnholyArmor(){;
//		CastTime ( 1f, "Unholy Armor");
//		//UnholyArmorStart();
//	}
//	
//	public void UnholyArmorStart(){
//		//var currentPDef	= characterScript.PDef;
//		//var buffedPDef = currentPDef * 0.3;
//		//characterScript.PDef = currentPDef + buffedPDef;
//		statusScript.armorBuffon( 100f, false, 0 );
//		skillinprogress = true;
//		SkillInProgress();
//		//checkunholyarmoractive = false;
//		enemyskillConfirmScript.unholyarmorActive = false;
//		enemyskillCdScript.RefreshSkill( enemyskillConfirmScript.unholyarmorActive, enemyskillCdScript.unholyArmor, "Unholy Armor" );
//	}

	//Runs a skill
	public void AttachEndSkill( int skillNumberVar ){
		print ("Attach Skill Attempt");
		//enemyskillSelectionScript.PrepSkillNew( "Boss", skillNumberVar);
	}

	//skill in progress
//	public void SkillInProgress(){
//		if( skillinprogress == true ){
//			StartCoroutine( skillinprogresstimer( 20f ) );
//		} 
//	}
//	
//	IEnumerator skillinprogresstimer(float waitTime)
//	{
//		yield return new WaitForSeconds(waitTime);
//		skillinprogress = false;
//		enemyskillSelectionScript.SkillChoice();
//	}

	// Use this for initialization
	void Awake () {
		characterScript = GetComponent<character_data>();
		enemyskillConfirmScript = GetComponent<enemyskill_confirm>();
		statusScript = GetComponent<status>();
		enemyskillCdScript = GetComponent<enemyskill_cd>();
		//voidSpawnScript = GetComponent<voidzoneSpawner>();
	}

	void Start () {
		enemyskillSelectionScript = GetComponent<enemySkillSelection>();
	}

	// Update is called once per frame
	void Update () {
		//SkillInProgress();
	}

}
