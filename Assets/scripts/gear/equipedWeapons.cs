using UnityEngine;
using System.Collections;

public class equipedWeapons : MonoBehaviour {
	public weapons primaryWeapon;
	public weapons secondaryWeapon;
    public classSkills classSkill;
	private skill_effects skillEffectScript;
	public currentWeapon currentWeaponEnum;
	public enum currentWeapon{
		Primary,
		Secondary
	}

	public void PopulateSkills( ){
		if ( primaryWeapon != null ){
            classSkills intPSkill2 = Object.Instantiate( primaryWeapon.skillTwo ) as classSkills;
            classSkills intPSkill3 = Object.Instantiate( primaryWeapon.skillThree ) as classSkills;
			//skillEffectScript.skilllistMain.Add( primaryWeapon.skillOne );
			skillEffectScript.skilllistMain.Add( intPSkill2);
			skillEffectScript.skilllistMain.Add( intPSkill3 );
			//print("equipped" + gameObject);
		} else {
			print ("no Primary weapons" + gameObject);
		}
		if ( secondaryWeapon != null ){
            var intPSkill2 = Object.Instantiate( secondaryWeapon.skillTwo ) as classSkills;
            var intPSkill3 = Object.Instantiate( secondaryWeapon.skillThree ) as classSkills;
			//skillEffectScript.skilllistAlt.Add( secondaryWeapon.skillOne );
			skillEffectScript.skilllistAlt.Add( intPSkill2 );
			skillEffectScript.skilllistAlt.Add( intPSkill3 );
			//print("equipped" + gameObject);
		} else {
			print ("no Secondary weapons" + gameObject);
		}
		if( classSkill != null ){
			skillEffectScript.classskill = Object.Instantiate( classSkill ) as classSkills;
		}
	}

	void Awake(){
		skillEffectScript = GetComponent<skill_effects>();
		PopulateSkills();
	}

}
