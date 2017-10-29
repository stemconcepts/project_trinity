using UnityEngine;
using System.Collections;
using Spine.Unity;

public class attackMovement : MonoBehaviour {
	private character_data characterScript;
	private characterMovementController characterMovementScript;
	private effectsController effectControllerScript;
	public GameObject dashEffect;
	public float movementSpeed;
	SkeletonAnimation skeletonAnimation;
	private animationControl playerAnimationControl;
	public int origSortingOrder;
	public int targetSortingOrder;
	private equipedWeapons equipedWeaponScript;
	Task movementTask;
	private soundController soundContScript;

	public void moveToTarget( float movementSpeed ){
		var getEnemy = characterScript.target;
		var enemyAnim = characterScript.target.gameObject.transform.Find("Animations").GetComponent<SkeletonAnimation>();
		targetSortingOrder = enemyAnim.GetComponent<MeshRenderer>().sortingOrder;
		skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingOrder = targetSortingOrder;
		//print( targetSortingOrder );
		var targetpos = getEnemy.GetComponent<character_data>().attackedPos;
		if( movementTask != null ){
			movementTask.Stop();
		} 
		movementTask = new Task( moveForward( 0.009f, targetpos, movementSpeed) );
	}
	IEnumerator moveForward( float waitTime, Vector2 targetPosVar, float movementSpeedVar ){
		Vector3 currentPosition = transform.position;
		
		#region Effects Controller
		effectControllerScript.CallEffect( dashEffect, "bottom" );
		#endregion

		while( characterScript.isAttacking ){
			float step = movementSpeedVar * Time.deltaTime;
			transform.position = Vector2.MoveTowards(transform.position, targetPosVar, step);
			yield return null;
		}
		//moveToHome();
	}

	public void moveToHome(){
		var origPos = characterScript.currentPanel.transform.position;
		skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingOrder = origSortingOrder;
		if( movementTask != null ){
			movementTask.Stop();
		} 
		//movementTask = new Task( moveBackward( 0.009f, origPos ) );

		var panelPos = characterScript.currentPanel.transform.position;
		panelPos.y = panelPos.y + 6f;
		movementTask = new Task( characterMovementScript.StartMove( 0.009f, panelPos, 50f) );
		var hopAnim = gameObject.tag == "Enemy" ? "hop" : characterMovementScript.hopAnim;
		var idleAnim = gameObject.tag == "Enemy" ? "idle" : characterMovementScript.idleAnim;
		
		skeletonAnimation.state.SetAnimation(0, hopAnim, false );

		skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0 );
	}
	IEnumerator moveBackward( float waitTime, Vector2 origPosVar ){
		var currentPosition = characterScript.currentPosition;
		while( currentPosition != origPosVar  ){
			float step = 70f * Time.deltaTime;
			transform.position = Vector2.MoveTowards(transform.position, origPosVar, step);		
			yield return null;
		}
	}

	public void StartMovement( float movementSpeedVar ) {
		movementSpeed = movementSpeedVar;
		skeletonAnimation.state.Event -= OnEventMove;
		skeletonAnimation.state.Event += OnEventMove;
	}

	public void OnEventMove(Spine.TrackEntry state, Spine.Event e ){
		if( e.Data.name == "movementStart" ){
			//print ("move start" + e.Data.name + "" + gameObject.name);
			moveToTarget( movementSpeed );
		} else if( e.Data.name == "movementBack" && characterScript.currentPosition != characterScript.origPosition ){
			//print ("move back" + e.Data.name + "" + gameObject.name);
			moveToHome();
		}
	}

	// Use this for initialization
	void Awake () {
		characterScript = GetComponent<character_data>();
		characterMovementScript = GetComponent<characterMovementController>();
		effectControllerScript = GetComponent<effectsController>();
		if ( this.transform.Find("Animations") ){
			skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			playerAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
			//origSortingOrder = skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingOrder;
		}
		equipedWeaponScript = GetComponent<equipedWeapons>();
		soundContScript = GetComponent<soundController>();
	}

	void Start() {
	}

	// Update is called once per frame
	void Update () {
		//moveToTarget();
	}
}
