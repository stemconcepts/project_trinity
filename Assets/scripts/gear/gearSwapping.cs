using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class gearSwapping : MonoBehaviour {
	Task swapTimer;
	public float time;
	bool swapReady = true;
	public AudioClip gearSwapSound;
	public AudioClip gearSwapReady;
	private soundController soundContScript;

	//gear swap
	public void SwapGear(){
		var skillactive = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<skill_cd>();
		if( swapReady == true && !skillactive.skillActive ){
			var buttonDataScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<sortbuttondata>();
			var allRoles = GameObject.FindGameObjectsWithTag("Player"); 
			for( int i = 0; i < allRoles.Length; i++ ){
				var currentWSlot = allRoles[i].GetComponent<skill_effects>();
				var currentWeaponData = allRoles[i].GetComponent<equipedWeapons>();
				if( currentWSlot.weaponSlot == skill_effects.weaponSlotEnum.Main ){
					currentWSlot.weaponSlot = skill_effects.weaponSlotEnum.Alt;
					print( currentWeaponData.secondaryWeapon.type.ToString() );
					currentWeaponData.currentWeaponEnum = equipedWeapons.currentWeapon.Secondary;
					//CheckGearType();
				} else {
					currentWSlot.weaponSlot = skill_effects.weaponSlotEnum.Main;
					print( currentWeaponData.primaryWeapon.type.ToString() );
					currentWeaponData.currentWeaponEnum = equipedWeapons.currentWeapon.Primary;
					//CheckGearType();
				}
				//restore Action Points - should be changed to GearSwap ability
				var charData = allRoles[i].GetComponent<character_data>();
				charData.actionPoints = charData.originalactionPoints;
			}
			CheckGearType();
			buttonDataScript.SkillSet();
			allRoles[0].GetComponent<button_clicks>().DisplaySkillsSecond();
			swapReady = false;
			GearSwapTimer(time);
			soundContScript.playSound( gearSwapSound );
		} else {
			print ("Gear Swap not Ready");
		}
	}

	void GearSwapTimer( float time ){
		swapTimer = new Task( SwapTimer( time ) );
		soundContScript.playSound( gearSwapReady );
	}

	IEnumerator SwapTimer( float time){
		yield return new WaitForSeconds(time);
		swapReady = true;
	}

	private void CheckGearType(){
		var allRoles = GameObject.FindGameObjectsWithTag("Player"); 
		foreach (var playerRole in allRoles) {
			var currentWeaponData = playerRole.GetComponent<equipedWeapons>();	
			var playerSkeletonAnim = playerRole.GetComponentInChildren<SkeletonAnimation>();
			var AAutoAttack = playerRole.GetComponent<auto_attack>();
			var charMovementScript = playerRole.GetComponent<characterMovementController>();
			var calculateDmgScript = playerRole.GetComponent<calculateDmg>();
			var currentWSlot = playerRole.GetComponent<skill_effects>();
			var weaponType = currentWSlot.weaponSlot == skill_effects.weaponSlotEnum.Main ? currentWeaponData.primaryWeapon : currentWeaponData.secondaryWeapon;
			if( weaponType.type != weapons.weaponType.heavyHanded && weaponType.type != weapons.weaponType.cursedGlove && weaponType.type != weapons.weaponType.clawAndCannon ){
				playerSkeletonAnim.state.AddAnimation(0, "idle", true, 0 );
				AAutoAttack.AAanimation = "attack1";
				charMovementScript.idleAnim = "idle";
				charMovementScript.hopAnim = "hop";
				calculateDmgScript.hitAnimNormal = "hit";
			} else {
				playerSkeletonAnim.state.SetAnimation(0, "toHeavy", false );
				playerSkeletonAnim.state.AddAnimation(0, "idleHeavy", true, 0 );
				AAutoAttack.AAanimation = "attack1Heavy";
				charMovementScript.idleAnim = "idleHeavy";
				charMovementScript.hopAnim = "hopHeavy";
				calculateDmgScript.hitAnimNormal = "hitHeavy";
			}
		}	
	}

	// Use this for initialization
	void Start() {
		CheckGearType();
		GearSwapTimer(time);
		if ( this.transform.Find("Animations") ){
		//	skeletonAnimation = this.transform.FindChild("Animations").GetComponent<SkeletonAnimation>();
		}
	}

	void Awake(){
		soundContScript = GetComponent<soundController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
