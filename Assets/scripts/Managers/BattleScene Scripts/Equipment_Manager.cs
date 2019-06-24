using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class Equipment_Manager : Base_Character_Manager
    {
        public Equipment_Manager()
        {
        }
        public weaponModel primaryWeapon;
        public weaponModel secondaryWeapon;
        public SkillModel classSkill;
        //private skill_effects skillEffectScript;
        public currentWeapon currentWeaponEnum;
        public enum currentWeapon{
            Primary,
            Secondary
        }
    
        public void PopulateSkills( ){
            //skillEffectScript = GetComponent<skill_effects>();
            if ( primaryWeapon != null ){
                var intPSkill2 = Object.Instantiate( primaryWeapon.skillTwo ) as SkillModel;
                var intPSkill3 = Object.Instantiate( primaryWeapon.skillThree ) as SkillModel;
                skillManager.primaryWeaponSkills.Add(intPSkill2);
                skillManager.primaryWeaponSkills.Add(intPSkill3);
            } else {
                print ("no Primary weapons" + gameObject);
            }
            if ( secondaryWeapon != null ){
                var intPSkill2 = Object.Instantiate( secondaryWeapon.skillTwo ) as SkillModel;
                var intPSkill3 = Object.Instantiate( secondaryWeapon.skillThree ) as SkillModel;
                skillManager.secondaryWeaponSkills.Add(intPSkill2);
                skillManager.secondaryWeaponSkills.Add(intPSkill3);
            } else {
                print ("no Secondary weapons" + gameObject);
            }
            if( classSkill != null ){
                skillManager.classSkill = Object.Instantiate( classSkill ) as SkillModel;
            }
        }
    }
}

