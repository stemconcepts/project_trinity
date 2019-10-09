using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class Equipment_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public weaponModel primaryWeapon;
        public weaponModel secondaryWeapon;
        public SkillModel classSkill;
        //private skill_effects skillEffectScript;
        public currentWeapon currentWeaponEnum;
        public enum currentWeapon{
            Primary,
            Secondary
        }
        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            PopulateSkills();
        }

        public void PopulateSkills( ){
            //skillEffectScript = GetComponent<skill_effects>();
            if ( primaryWeapon != null ){
                var intPSkill2 = Object.Instantiate( primaryWeapon.skillTwo ) as SkillModel;
                var intPSkill3 = Object.Instantiate( primaryWeapon.skillThree ) as SkillModel;
                baseManager.skillManager.primaryWeaponSkills.Add(intPSkill2);
                baseManager.skillManager.primaryWeaponSkills.Add(intPSkill3);
            } else {
                print ("no Primary weapons" + gameObject);
            }
            if ( secondaryWeapon != null ){
                var intPSkill2 = Object.Instantiate( secondaryWeapon.skillTwo ) as SkillModel;
                var intPSkill3 = Object.Instantiate( secondaryWeapon.skillThree ) as SkillModel;
                baseManager.skillManager.secondaryWeaponSkills.Add(intPSkill2);
                baseManager.skillManager.secondaryWeaponSkills.Add(intPSkill3);
            } else {
                print ("no Secondary weapons" + gameObject);
            }
            if( classSkill != null ){
                baseManager.skillManager.skillModel = Object.Instantiate( classSkill ) as SkillModel;
            }
        }
    }
}

