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
			//skillEffectScript.skilllistMain.Add( primaryWeapon.skillOne );
			skillEffectScript.skilllistMain.Add( primaryWeapon.skillTwo );
			skillEffectScript.skilllistMain.Add( primaryWeapon.skillThree );
			//print("equipped" + gameObject);
		} else {
			print ("no Primary weapons" + gameObject);
		}
		if ( secondaryWeapon != null ){
			//skillEffectScript.skilllistAlt.Add( secondaryWeapon.skillOne );
			skillEffectScript.skilllistAlt.Add( secondaryWeapon.skillTwo );
			skillEffectScript.skilllistAlt.Add( secondaryWeapon.skillThree );
			//print("equipped" + gameObject);
		} else {
			print ("no Secondary weapons" + gameObject);
		}
		if( classSkill != null ){
			skillEffectScript.classskill = classSkill;
		}
	}

	void Awake(){
		skillEffectScript = GetComponent<skill_effects>();
		PopulateSkills();
	}

}
