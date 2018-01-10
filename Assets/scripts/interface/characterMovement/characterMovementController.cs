using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class characterMovementController : MonoBehaviour {
	private character_data characterScript;
	SkeletonAnimation skeletonAnimation;
	private animationControl playerAnimationControl;
	private calculateDmg calculateDmgScript;
	private skill_effects skillEffectsScript;
	private equipedWeapons equipedWeaponScript;
	public string hopAnim = "hop";
	public string idleAnim = "idle";
	Task movementTask;

    public void ForcedMove( GameObject currentPanel, string direction = "back" ){
        var currentPanelNum = currentPanel.GetComponent<movementPanelController>().panelNumber;
        int targetPanelNum;
        if( direction == "back" ){
            targetPanelNum = currentPanelNum == 2 ? currentPanelNum : currentPanelNum++;
        } else if ( direction == "forward" ){
            targetPanelNum = currentPanelNum == 0 ? currentPanelNum : currentPanelNum--;
        }
        var targetPanel = currentPanel.transform.parent.GetChild(targetPanelNum);
        MoveToPanel( targetPanel, "hit" );
    }

    public void MoveToPanel( GameObject targetPanel, string hopAnim = hopAnim ){
		Vector2 panelPos = targetPanel.transform.position;
		panelPos.y = panelPos.y + 6f;
		if( movementTask != null ){
			movementTask.Stop();
		} 
		movementTask = new Task( StartMove( 0.009f, panelPos, 50f) );
		skeletonAnimation.state.SetAnimation(0, hopAnim, false );
		skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0 );
		//characterScript.target.GetComponent<character_data>().incomingDmg = 0;
		//characterScript.target.GetComponent<calculateDmg>().damageTaken = 0;
	}
	public IEnumerator StartMove( float waitTime, Vector2 panelPosVar, float movementSpeedVar ){
		//Vector2 currentPosition = transform.position;
		while( characterScript.currentPosition != panelPosVar ){
			float step = movementSpeedVar * Time.deltaTime;
			transform.position = Vector2.MoveTowards(transform.position, panelPosVar, step);
			yield return null;
			characterScript.origPosition = characterScript.currentPosition;
			characterScript.isMoving = false;
		}
		//movementTask.Stop();
		//moveToHome();
	}

	public void SetStartingPanel( GameObject targetPanel ){
		Vector2 panelPos = targetPanel.transform.position;
		panelPos.y = panelPos.y + 6f;
		transform.position = new Vector2(panelPos.x, panelPos.y);
		skeletonAnimation.state.SetAnimation(0, hopAnim, false );
		skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0 );
	}

	// Use this for initialization
	void Start () {
		if ( this.transform.Find("Animations") ){
			skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			playerAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
		}
		calculateDmgScript = GetComponent<calculateDmg>();
		//SetStartingPanel( characterScript.currentPanel );
	}

	// Use this for initialization
	void Awake () {
		characterScript = GetComponent<character_data>();
		skillEffectsScript = GetComponent<skill_effects>();
		equipedWeaponScript = GetComponent<equipedWeapons>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
