using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Equipment_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public weaponModel primaryWeapon;
        public weaponModel secondaryWeapon;
        public SkillModel classSkill;
        public bauble bauble;
        public currentWeapon currentWeaponEnum;
        public enum currentWeapon{
            Primary,
            Secondary
        }
        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            PopulateSkills();
            CalculateEquipmentPower();
        }

        public void PopulateSkills( ){
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

        private void CalculateEquipmentPower()
        {
            if (bauble)
            {
                List<EffectOnEventModel> baubleEffects = new List<EffectOnEventModel>();
                foreach (var effect in bauble.effectsOnEvent)
                {
                    baubleEffects.Add(Object.Instantiate(effect) as EffectOnEventModel);
                }

                foreach (var effect in baubleEffects)
                {
                    var attrValue = !string.IsNullOrEmpty(bauble.focusAttribute) ? baseManager.characterManager.GetAttributeValue(bauble.focusAttribute) : 0;
                    var stat = bauble.flatAmount != 0 ? 0 : attrValue;
                    effect.power = bauble.flatAmount != 0 ? bauble.flatAmount + attrValue : stat * 0.25f;
                    effect.duration = bauble.duration;
                    effect.trigger = bauble.trigger.ToString();
                    effect.triggerChance = bauble.triggerChance;
                    effect.focusAttribute = bauble.focusAttribute;
                    effect.owner = gameObject;
                    effect.dispellable = bauble.dispellable;
                    effect.coolDown = bauble.coolDown;
                    Battle_Manager.eventManager.EventAction += effect.RunEffect;
                    if (bauble.trigger == bauble.triggerGrp.Passive)
                    {
                        var eventModel = new EventModel
                        {
                            eventName = bauble.trigger.ToString(),
                            extTarget = baseManager.characterManager,
                            eventCaller = baseManager.characterManager
                            //extraInfo = damageModel.damageTaken
                        };
                        Battle_Manager.eventManager.BuildEvent(eventModel);
                        effect.RunEffect();
                    }
                }
            }
        }
    }
}

