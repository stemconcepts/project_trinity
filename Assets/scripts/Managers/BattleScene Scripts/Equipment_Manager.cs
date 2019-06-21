using System;

namespace AssemblyCSharp
{
    public class Equipment_Manager : Base_Character_Manager
    {
        public Equipment_Manager()
        {
        }
        public weapons primaryWeapon;
        public weapons secondaryWeapon;
        public SkillModel classSkill;
        private skill_effects skillEffectScript;
        public currentWeapon currentWeaponEnum;
        public enum currentWeapon{
            Primary,
            Secondary
        }
    
        public void PopulateSkills( ){
            skillEffectScript = GetComponent<skill_effects>();
            if ( primaryWeapon != null ){
                var intPSkill2 = Object.Instantiate( primaryWeapon.skillTwo ) as SkillModel;
                var intPSkill3 = Object.Instantiate( primaryWeapon.skillThree ) as SkillModel;
                //skillEffectScript.skilllistMain.Add( primaryWeapon.skillOne );
                skillEffectScript.skilllistMain.Add( intPSkill2);
                skillEffectScript.skilllistMain.Add( intPSkill3 );
                //print("equipped" + gameObject);
            } else {
                print ("no Primary weapons" + gameObject);
            }
            if ( secondaryWeapon != null ){
                var intPSkill2 = Object.Instantiate( secondaryWeapon.skillTwo ) as SkillModel;
                var intPSkill3 = Object.Instantiate( secondaryWeapon.skillThree ) as SkillModel;
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
    }
}

