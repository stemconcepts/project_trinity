using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class sortbuttondata : MonoBehaviour {
	//weapon skill data
	public skill_effects tankSkillData;
	public skill_effects healerSkillData;
	public skill_effects dpsSkillData;

	public List<string> tankSkillNames = new List<string>();
	public List<Sprite> tankSkillIcons = new List<Sprite>();
	public List<SkillModel> tankSkillClass = new List<SkillModel>();
	public List<int> tankSkillIDs = new List<int>();
	public List<int> tankSkillCosts = new List<int>();

	public List<SkillModel> healerSkillClass = new List<SkillModel>();
	public List<string> healerSkillNames = new List<string>();
	public List<Sprite> healerSkillIcons = new List<Sprite>();
	public List<int> healerSkillIDs = new List<int>();
	public List<int> healerSkillCosts = new List<int>();

	public List<string> dpsSkillNames = new List<string>();
	public List<Sprite> dpsSkillIcons = new List<Sprite>();
	public List<SkillModel> dpsSkillClass = new List<SkillModel>();
	public List<int> dpsSkillIDs = new List<int>();
	public List<int> dpsSkillCosts = new List<int>();

	public SkillModel GetClassSkill( string role ){
		if( role == "Tank" ){
			return tankSkillData.classskill;
		} else if( role == "Healer" ){
			return healerSkillData.classskill;
		} else if ( role == "Dps" ){
			return dpsSkillData.classskill;
		} else {
			print ("fail");
			return null;
		}
	}

	public void SkillSet () {
		//clear tank lists
		tankSkillIDs.Clear();
		tankSkillIcons.Clear();
		tankSkillCosts.Clear();
		tankSkillNames.Clear();
		tankSkillClass.Clear();
		//clear healer lists
		healerSkillIDs.Clear();
		healerSkillIcons.Clear();
		healerSkillCosts.Clear();
		healerSkillNames.Clear();
		healerSkillClass.Clear();
		//clear dps lists
		dpsSkillIDs.Clear();
		dpsSkillIcons.Clear();
		dpsSkillCosts.Clear();
		dpsSkillNames.Clear();
		dpsSkillClass.Clear();
		for( int i = 0; i < tankSkillData.skilllistMain.Count; i++ ){
			List<SkillModel> activeSkills;
			if( tankSkillData.weaponSlot.ToString() == "Main" ){
				activeSkills = tankSkillData.skilllistMain;
			//	print("main skills set");
			} else {
				activeSkills = tankSkillData.skilllistAlt;
				//print("alt skills set");
			}
					tankSkillNames.Add( activeSkills[i].displayName );
					tankSkillIcons.Add( activeSkills[i].skillIcon );
					tankSkillIDs.Add( i );
					tankSkillClass.Add( activeSkills[i] );
					tankSkillCosts.Add( activeSkills[i].skillCost );
		}
		for( int i = 0; i < healerSkillData.skilllistMain.Count; i++ ){
			List<SkillModel> activeSkills;
			if( healerSkillData.weaponSlot == skill_effects.weaponSlotEnum.Main ){
				activeSkills = healerSkillData.skilllistMain;
			} else {
				activeSkills = healerSkillData.skilllistAlt;
			}
					healerSkillNames.Add( activeSkills[i].displayName );
					healerSkillIcons.Add( activeSkills[i].skillIcon );
					healerSkillIDs.Add( i );
					healerSkillClass.Add( activeSkills[i] );
					healerSkillCosts.Add ( activeSkills[i].skillCost );
		}
		for( int i = 0; i < dpsSkillData.skilllistMain.Count; i++ ){
			List<SkillModel> activeSkills;
			if( dpsSkillData.weaponSlot == skill_effects.weaponSlotEnum.Main ){
				activeSkills = dpsSkillData.skilllistMain;
			} else {
				activeSkills = dpsSkillData.skilllistAlt;
			}
					dpsSkillNames.Add( activeSkills[i].displayName );
					dpsSkillIcons.Add( activeSkills[i].skillIcon );
					dpsSkillIDs.Add( i );
					dpsSkillClass.Add( activeSkills[i] );
					dpsSkillCosts.Add ( activeSkills[i].skillCost );
		}
	}

	// Use this for initialization
	void Awake(){
		SkillSet();
	}

	// Update is called once per frame
	void Start () {
        //battleManager.BattleStart( 5f );
	}
}
