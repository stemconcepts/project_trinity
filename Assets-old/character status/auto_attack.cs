using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class auto_attack : MonoBehaviour {
	[SpineAnimation]
	SkeletonAnimation skeletonAnimation;
	private animationControl playerAnimationControl;
	private attackMovement attackMovementScript;
	public float attackMovementSpeed;
	private character_data characterScript;
	private calculateDmg calculateDmgScript;
	public int damage;
	private status statusScript;
	public GameObject skillConfirmObject;
	private skill_cd skillCdScript;
    private Task AABusyTask;
    private Task AATask;
	public string AAanimation;
	//private attackChecker attackCheckerScript;
	//private soundController soundContScript;
	//temp way to check stun - needs a function that pulls specific status
	//public singleStatus stunSO;

	// Use this for initialization
	void Start(){
		RunAttackLoop();
	}

	void Awake() {
		// After naming the object, get the components within those game objects.
		characterScript = GetComponent<character_data>();
		calculateDmgScript = GetComponent<calculateDmg>();
		statusScript = GetComponent<status>();
		attackMovementScript = GetComponent<attackMovement>();
		//skillCdScript = skillConfirmObject.GetComponent<skill_cd>();
		// Use this to do timed events -attack- is the name of the timer 
		skillCdScript = skillConfirmObject.GetComponent<skill_cd>();
		//attackCheckerScript = GetComponent<attackChecker>();
		//soundContScript = GetComponent<soundController>();
		if ( this.transform.Find("Animations") ){
			skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			playerAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
		}
	}


	
	// Update is called once per frame
	void Update () {
	
	}

	// this is used to break down what happens when the timer is complete.
	/*IEnumerator attack(float waitTime)
	{
		while( characterScript.target.Health > 0 && !statusScript.DoesStatusExist("stun") && !characterScript.isAttacking ){
			yield return new WaitForSeconds(waitTime);
			//if not disabled or busy in some way then attack as normal*/
	void RunAttackLoop(){	
        if( true ){
            List<GameObject> targets = new List<GameObject>{
                characterScript.target.gameObject
            };
            calculateDmgScript.dueDmgTargets = targets;
			if( characterScript.isAlive && characterScript.canAutoAttack && !characterScript.isAttacking && !statusScript.DoesStatusExist( "stun" ) && !skillCdScript.skillActive ){
					characterScript.isAttacking = true;
					characterScript.target.incomingDmg = characterScript.PAtk;
				    //print ( characterScript.target.incomingDmg );
					var getEnemy = characterScript.target;
					//CheckHit();
					var enemyCalculation = getEnemy.GetComponent<calculateDmg>();
					if ( AAanimation != "" && playerAnimationControl.inAnimation == false ) 
					{
						//call animation variable
						var animationDuration = skeletonAnimation.state.SetAnimation(0, AAanimation, false).Animation.duration;
						attackMovementScript.StartMovement(attackMovementSpeed);
						//play sounds
						//soundContScript.playSound();
						skeletonAnimation.state.SetAnimation(0, AAanimation, false);
						gameObject.transform.Find("Animations").GetComponent<animationControl>().inAnimation = true;
						enemyCalculation.calculatedamage(skeletonAnimationVar:this.transform.Find("Animations").GetComponent<SkeletonAnimation>() );
					//skeletonAnimation.state.AddAnimation(0, "hop", false, -0.06f );
						//skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
                        AABusyTask = new Task( busyAnimation( animationDuration ) );
						//StartCoroutine( busyAnimation( animationDuration ));
					}
					if( characterScript.target.incomingDmg > 0 ){
						//print ( characterScript.objectName + " sending " + characterScript.target.incomingDmg + " damage to " + characterScript.target );
					}
			}
			else {
				print ( characterScript.objectName + " cannot attack");
                AABusyTask = new Task( busyAnimation( 5f ) );
				//StartCoroutine( busyAnimation( 5f ));
			}
        } else {
            AATask = new Task( AutoAttackCD() );
	    }
    }
		//}
	//}

	IEnumerator AutoAttackCD()
	{
		yield return new WaitForSeconds( characterScript.ATKspd );
		RunAttackLoop();
	}

	IEnumerator busyAnimation(float waitTime )
	{
		yield return new WaitForSeconds(waitTime);
		characterScript.isAttacking = false;
		gameObject.transform.Find("Animations").GetComponent<animationControl>().inAnimation = false;
		//StartCoroutine( AutoAttackCD() );
        AATask = new Task( AutoAttackCD() );
	}
	
}
