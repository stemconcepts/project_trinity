using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class auto_attackEnemy : MonoBehaviour {

	[SpineAnimation]
	SkeletonAnimation skeletonAnimation;
	private animationControl enemyAnimationControl;
	private attackMovement attackMovementScript;
	public float attackMovementSpeed;
	// Use this to reference other data in other scripts, First you must get the game object with the scripts you need.
	//public GameObject attack_data;
	private character_data characterScript;
	private calculateDmg calculateDmgScript;
	public int damage;
	private status statusScript;
	private enemyskillAI skillAIScript;
	private enemySkillSelection enemySkillSelectionScripts;
	public string AAanimation;
	private Task autoAttackTask;
	public character_data Target;
	private soundController soundContScript;
	
	// Use this for initialization
	void Awake() {
		if ( this.transform.Find("Animations") ){
			skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			enemyAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
		}
		// After naming the object, get the components within those game objects.
		skillAIScript = GetComponent<enemyskillAI>();
		characterScript = GetComponent<character_data>();
		calculateDmgScript = GetComponent<calculateDmg>();
		statusScript = GetComponent<status>();
		attackMovementScript = GetComponent<attackMovement>();
		enemySkillSelectionScripts = GetComponent<enemySkillSelection>();
		soundContScript = GetComponent<soundController>();
	}

	void Start(){
		// Use this to do timed events -attack- is the name of the timer 
		//autoAttackTask = new Task( attack( characterScript.ATKspd ) );
		//StartCoroutine(attack( characterScript.ATKspd ));
		RunAttackLoop();
	}

	// Update is called once per frame
	void Update () {
		
	}

//	public void OnEventHit(Spine.AnimationState state, int trackIndex, Spine.Event e ){
//		characterScript.target.incomingDmg = characterScript.PAtk;
//		var getEnemy = characterScript.target;
//		var enemyCharData = getEnemy.GetComponent<character_data>();
//		var enemyAnimation = getEnemy.transform.FindChild("Animations").GetComponent<SkeletonAnimation>();
//		var enemyAnimationControl = getEnemy.transform.FindChild("Animations").GetComponent<animationControl>();
//		if( e.Data.name == "hit" ){
//			//print ( e.Data.name);
//			var enemyCalculation = getEnemy.GetComponent<calculateDmg>();
//			enemyCalculation.calculatedamage("N.A");
//			if ( enemyAnimationControl.inAnimation == false && enemyCharData.isAttacking == false ) {
//				enemyAnimation.state.SetAnimation(0, "hit", false);
//				enemyAnimation.state.AddAnimation(0, "idle", true, 0 );
//			}
//		} 
//	}
//
//	void CheckHit(){
//		skeletonAnimation.state.Event -= OnEventHit;
//		skeletonAnimation.state.Event += OnEventHit;
//	}
	
	//Get Auto Attack Target
	GameObject GetTarget(){
		var tankTarget = GameObject.Find("Guardian");
		if( tankTarget.GetComponent<character_data>().inThreatZone ){
			characterScript.target = tankTarget.GetComponent<character_data>();
			return tankTarget.gameObject;		
		} else {
            var playerList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player")); 
            var aliveList = new List<GameObject>();
            for (int i = 0; i < playerList.Count; i++)
            {
                var playerData = playerList[i].GetComponent<character_data>();
                if( playerData.isAlive ){
                    aliveList.Add( playerList[i] );
                }
            }
            if ( aliveList.Count > 0 ){
    			var randomNumber = Random.Range (0, aliveList.Count);
    			var randomTarget = aliveList[randomNumber];
    			characterScript.target = randomTarget.GetComponent<character_data>();
    			return randomTarget.gameObject;
            } else {
                return null;
            }
		}
	}

	// this is used to break down what happens when the timer is complete.
	//IEnumerator attack(float waitTime )
	//{
	//	while( characterScript.Health > 0 ){
	void RunAttackLoop(){
        Target = GetTarget().GetComponent<character_data>();
        List<GameObject> targets = new List<GameObject>{
            Target.gameObject
        };
        calculateDmgScript.dueDmgTargets = targets;
			if( !characterScript.isAttacking && !statusScript.DoesStatusExist( "stun" ) && !enemySkillSelectionScripts.casting && !enemySkillSelectionScripts.skillinprogress ){
				characterScript.isAttacking = true;
				//CheckHit();
				//print("AA on " + Target.role + ":" + Target.incomingDmg);
				Target.incomingDmg = characterScript.PAtk;
				//var getEnemy = characterScript.target;
				var enemyCalculation = Target.GetComponent<calculateDmg>();
				if ( AAanimation != null && enemyAnimationControl.inAnimation == false ) {
					//call animation variable
					var animationDuration = skeletonAnimation.state.SetAnimation(0, AAanimation, false).Animation.duration;
					attackMovementScript.StartMovement(attackMovementSpeed);
					//play sounds
					//soundContScript.playSound();
					skeletonAnimation.state.SetAnimation(0, AAanimation, false);
					enemyCalculation.calculatedamage(skeletonAnimationVar:this.transform.Find("Animations").GetComponent<SkeletonAnimation>() );
					//skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
					StartCoroutine( busyAnimation( animationDuration ));
				} else {
					StartCoroutine( busyAnimation( 5f ));
				}
			}
			else {
				print ( characterScript.objectName + " cannot attack");
				StartCoroutine( busyAnimation( 5f ));
			}
			//yield return new WaitForSeconds(waitTime);
		//}
	}

	IEnumerator AutoAttackCD()
	{
		yield return new WaitForSeconds( characterScript.ATKspd );
		RunAttackLoop();
	}

	IEnumerator busyAnimation(float waitTime )
	{
		yield return new WaitForSeconds(waitTime);
		characterScript.isAttacking = false;
		StartCoroutine( AutoAttackCD() );
	}
	
}
