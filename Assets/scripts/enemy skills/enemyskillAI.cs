using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyskillAI : MonoBehaviour {
	//private skill_effects skillEffectsScript;
	private enemySkill_effects enemySkillEffectsScript;
	private enemySkillSelection skillSelectionScript;
	public bool skillInProgress;	
	private status statusScript;
	private character_data characterScript;
	public List<enemySkill> skillList = new List<enemySkill>();

	//creates a list of skill numbers and returns 1 at random and removes it from the list
	enemySkill SkillToRun( List<enemySkill> bossSkillList ){
			skillList.Clear();
			for( int x = 0; x < bossSkillList.Count; x++ ){
				if( skillSelectionScript.bossphaseone && bossSkillList[x].bossOnlyP1 && !bossSkillList[x].skillActive ){
					skillList.Add( bossSkillList[x] );
				} else
				if( skillSelectionScript.bossphasetwo && bossSkillList[x].bossOnlyP2 && !bossSkillList[x].skillActive  ){
					skillList.Add( bossSkillList[x] );
				} else
				if( skillSelectionScript.bossphasethree && bossSkillList[x].bossOnlyP3 && !bossSkillList[x].skillActive  ){
					skillList.Add( bossSkillList[x] );
				}
			}
		if( skillList.Count == 0 ){
			return null;
		}
		var randomNumber = Random.Range (0, (skillList.Count) );
		var returnedSkill = skillList[randomNumber];
		return returnedSkill;
	}
		
	public void BeginSkillRotation(){
		var boss = GameObject.FindGameObjectWithTag("Boss");
		var bossSkillList = boss.transform.parent.GetComponent<enemySkill_effects>().enemySkilllist;
		var randomSkill = SkillToRun( bossSkillList );
				//print( randomNumber );
		if( !statusScript.DoesStatusExist( "stun" ) && !characterScript.isAttacking && skillSelectionScript.bossphaseone == true && randomSkill != null ){
			//for( int i=0; i < bossSkillList.Count; i++ ){
				if( !skillInProgress && randomSkill.bossOnlyP1 && !skillSelectionScript.skillinprogress ){
				//	print (bossSkillList[randomNumber].displayName + " starting");
					skillInProgress = true;
					StartCoroutine( skillinprogresstimer( 10f, randomSkill ) );
					//break;
				} else if ( skillInProgress == true ){
					StartCoroutine( skillReset( 1f ) );
					//print (" skill in progress");
//					
				} else {
					StartCoroutine( skillReset( 1f ) );
					//print (bossSkillList[randomNumber].displayName + " not ready");
				} 
			//}
		} else 
		if( !statusScript.DoesStatusExist( "stun" ) && !characterScript.isAttacking && skillSelectionScript.bossphasetwo == true && randomSkill != null ){
			//for( int i=0; i < bossSkillList.Count; i++ ){
				if( skillInProgress == false && randomSkill.bossOnlyP2 == true){
					//print (bossSkillList[(int)randomNumber].displayName + " starting");
					skillInProgress = true;
					StartCoroutine( skillinprogresstimer( 10f, randomSkill ) );
					//break;
				} else if ( skillInProgress == true ){
					//print (" skill in progress");
					StartCoroutine( skillReset( 1f ) );
					//break;
				} else {
					StartCoroutine( skillReset( 1f ) );
				}
			//}
		} else
		if( !statusScript.DoesStatusExist( "stun" ) && !characterScript.isAttacking && skillSelectionScript.bossphasethree == true && randomSkill != null ){
			//for( int i=0; i < bossSkillList.Count; i++ ){
				if( skillInProgress == false && randomSkill.bossOnlyP3 == true ){
					//print (bossSkillList[(int)randomNumber].displayName + " starting");
					skillInProgress = true;
					StartCoroutine( skillinprogresstimer( 10f, randomSkill ) );
					//break;
				} else if ( skillInProgress == true ){
					//print (" skill in progress");
					StartCoroutine( skillReset( 1f ) );
					//break;
				} else {
					StartCoroutine( skillReset( 1f ) );
				}
			//}
		}
		else {
										print("skill reset");
			StartCoroutine( skillReset( 5f )); 
		}
	}

	//skill in progress timer
	IEnumerator skillinprogresstimer(float waitTime, enemySkill enemySkill)
	{
		skillSelectionScript.PrepSkillNew(enemySkill);
		yield return new WaitForSeconds(waitTime);
		skillInProgress = false;
		BeginSkillRotation();
	}

	IEnumerator skillReset(float waitTime )
	{
		//skillSelectionScript.PrepSkill("Boss", skillID);
		yield return new WaitForSeconds(waitTime);
		skillInProgress = false;
		print("starting rotation");
		BeginSkillRotation();
	}

//	IEnumerator noSkillFound(float waitTime )
//	{
//		yield return new WaitForSeconds(waitTime);
//		BeginSkillRotation();
//	}

	// Use this for initialization
	void Awake () {
		//skillEffectsScript = GetComponent<skill_effects>();
		skillSelectionScript = GetComponent<enemySkillSelection>();
		characterScript = GetComponent<character_data>();
		statusScript = GetComponent<status>();
	}

	void Start(){
		BeginSkillRotation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
