using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class WeaponEffect
    {
        [Header("Passive Effects Triggers:")]
        public triggerGrp trigger;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public float triggerChance;

        [Header("Status Effects affected by triggers:")]
        [ConditionalHide("trigger", (int)triggerGrp.None, true)]
        public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        [ConditionalHide("trigger", (int)triggerGrp.None, true)]
        public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        public bool statusFriendlyDispellable = true;

        [Header("Permanent Equipment Bonuses:")]
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public CharacterStats focusAttribute;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public float flatAmount;
        public bool dispellable;

        [Header("Duration and Cooldown:")]
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public int turnDuration;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public float coolDown;
    }

    [System.Serializable]
    public class ResetEvent
    {
        [Header("Reset Effects:")]
        public triggerGrp trigger;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public List<EffectOnEventModel> resetOnEvent = new List<EffectOnEventModel>();
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public CharacterStats focusAttribute;
    }

    public class EquipmentManager : MonoBehaviour
    {
        public CharacterManagerGroup baseManager;
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
            baseManager = this.gameObject.GetComponent<CharacterManagerGroup>();
            PopulateSkills();
            CalculateEquipmentPower();
        }

        public void PopulateSkills( ){
            if ( primaryWeapon != null ){
                var intPSkill1 = Object.Instantiate(primaryWeapon.skillOne) as SkillModel;
                var intPSkill2 = Object.Instantiate(primaryWeapon.skillTwo) as SkillModel;
                var intPSkill3 = Object.Instantiate(primaryWeapon.skillThree) as SkillModel;
                ((PlayerSkillManager)baseManager.skillManager).primaryWeaponSkills.Add(intPSkill1);
                ((PlayerSkillManager)baseManager.skillManager).primaryWeaponSkills.Add(intPSkill2);
                ((PlayerSkillManager)baseManager.skillManager).primaryWeaponSkills.Add(intPSkill3);
                baseManager.characterManager.characterModel.canAutoAttack = primaryWeapon.enablesAutoAttacks;
            } else {
                print ("no Primary weapons" + gameObject);
            }
            if ( secondaryWeapon != null ){
                var intPSkill1 = Object.Instantiate(secondaryWeapon.skillOne) as SkillModel;
                var intPSkill2 = Object.Instantiate(secondaryWeapon.skillTwo) as SkillModel;
                var intPSkill3 = Object.Instantiate(secondaryWeapon.skillThree) as SkillModel;
                ((PlayerSkillManager)baseManager.skillManager).secondaryWeaponSkills.Add(intPSkill1);
                ((PlayerSkillManager)baseManager.skillManager).secondaryWeaponSkills.Add(intPSkill2);
                ((PlayerSkillManager)baseManager.skillManager).secondaryWeaponSkills.Add(intPSkill3);
            } else {
                print ("no Secondary weapons" + gameObject);
            }
            if( classSkill != null ){
                ((PlayerSkillManager)baseManager.skillManager).skillModel = Object.Instantiate( classSkill ) as SkillModel;
            }
        }

        private void CalculateEquipmentPower()
        {
            if (bauble)
            {
                List<EffectOnEventModel> baubleEffects = new List<EffectOnEventModel>();
                foreach (var effect in bauble.effectsOnEvent)
                {
                    baubleEffects.Add(Object.Instantiate(effect));
                }

                foreach (var effect in baubleEffects)
                {
                    SetEffectAndRun(effect, bauble);
                }
            }
            if (primaryWeapon || secondaryWeapon)
            {
                List<EffectOnEventModel> primaryWeaponEffects = new List<EffectOnEventModel>();
                List<EffectOnEventModel> secondaryWeaponEffects = new List<EffectOnEventModel>();
                foreach(var weaponEffect in primaryWeapon.weaponEffects)
                {
                    foreach (var effect in weaponEffect.effectsOnEvent)
                    {
                        //effect.resetEvent = primaryWeapon.resetEvent;
                        primaryWeaponEffects.Add(Object.Instantiate(effect));
                    }
                    foreach (var effect in primaryWeaponEffects)
                    {
                        SetEffectAndRun(effect, weaponEffect);
                    }
                }

                foreach (var weaponEffect in secondaryWeapon.weaponEffects)
                {
                    foreach (var effect in weaponEffect.effectsOnEvent)
                    {
                        //effect.resetEvent = secondaryWeapon.resetEvent;
                        secondaryWeaponEffects.Add(Object.Instantiate(effect));
                    }
                    foreach (var effect in secondaryWeaponEffects)
                    {
                        SetEffectAndRun(effect, weaponEffect);
                    }
                }
            }
        }

        private void SetEffectAndRun(EffectOnEventModel effect, bauble bauble)
        {
            var attrValue = !string.IsNullOrEmpty(bauble.focusAttribute) ? baseManager.characterManager.GetAttributeValue(bauble.focusAttribute, baseManager.characterManager.characterModel) : 0;
            var stat = bauble.flatAmount != 0 ? 0 : attrValue;
            effect.power = bauble.flatAmount != 0 ? bauble.flatAmount + attrValue : stat * 0.25f;
            effect.turnDuration = bauble.turnDuration;
            effect.trigger = bauble.trigger.ToString();
            effect.triggerChance = bauble.triggerChance;
            effect.focusAttribute = bauble.focusAttribute;
            effect.owner = gameObject;
            effect.dispellable = bauble.dispellable;
            effect.coolDown = bauble.coolDown;
            BattleManager.eventManager.EventAction += effect.RunEffectFromSkill;
            if (bauble.trigger == triggerGrp.Passive)
            {
                SetEventAndRunEffect(effect, bauble.trigger);
            }
        }

        private void SetEffectAndRun(EffectOnEventModel effect, WeaponEffect weaponEffect)
        {
            var attrValue = weaponEffect.focusAttribute != CharacterStats.None ? baseManager.characterManager.GetAttributeValue(weaponEffect.focusAttribute.ToString(), baseManager.characterManager.characterModel) : 0;
            var stat = weaponEffect.flatAmount != 0 ? 0 : attrValue;
            effect.power = weaponEffect.flatAmount != 0 ? weaponEffect.flatAmount + attrValue : stat * 0.25f;
            effect.turnDuration = weaponEffect.turnDuration;
            effect.trigger = weaponEffect.trigger.ToString();
            effect.triggerChance = weaponEffect.triggerChance;
            effect.focusAttribute = weaponEffect.focusAttribute.ToString();
            effect.owner = gameObject;
            effect.singleStatusGroup = weaponEffect.singleStatusGroup;
            effect.statusDispellable = weaponEffect.statusDispellable;
            effect.singleStatusGroupFriendly = weaponEffect.singleStatusGroupFriendly;
            effect.statusFriendlyDispellable = weaponEffect.statusFriendlyDispellable;
            effect.dispellable = weaponEffect.dispellable;
            effect.coolDown = weaponEffect.coolDown;
            BattleManager.eventManager.EventAction += effect.RunEffectFromSkill;
            if (weaponEffect.trigger == triggerGrp.Passive)
            {
                SetEventAndRunEffect(effect, weaponEffect.trigger);
            }
        }

        void SetEventAndRunEffect(EffectOnEventModel effect, triggerGrp trigger)
        {
            var eventModel = new EventModel
            {
                eventName = trigger.ToString(),
                extTarget = baseManager.characterManager,
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eventModel);
            effect.RunEffectFromSkill();
        }
    }
}

