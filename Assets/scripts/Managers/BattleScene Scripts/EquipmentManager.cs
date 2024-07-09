using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class WeaponEffect
    {
        [Header("Passive Effects Triggers:")]
        public EventTriggerEnum trigger;
        [Tooltip("Dont use if Trigger is None")]
        [SerializeField]
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
        //[ConditionalHide("trigger", (int)EventTriggerEnum.None == 0, true)]
        //[Range(0.0f, 1.0f)]
        //public float triggerChance;

        //[Tooltip("Dont use if Trigger is None and GainStatus Effect is not used")]
        //[Header("Status Effects affected by triggers:")]
        //public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        //public List<StatusItem> singleStatusGroup = new List<StatusItem>();
        //public bool statusDispellable = true;

        //[Tooltip("Dont use if Trigger is None and GainStatus Effect is not used")]
        //[Header("Friendly Status Effects:")]
        //public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        //public List<StatusItem> singleStatusGroupFriendly = new List<StatusItem>();
        //public bool statusFriendlyDispellable = true;


        [Header("Permanent Equipment Bonuses:")]
        public CharacterStats focusAttribute;
        [ConditionalHide("focusAttribute", (int)CharacterStats.None == 0, true)]
        public float AttributeIncrease;
        //public bool dispellable;

        /*[Header("Duration and Cooldown:")]
        [ConditionalHide("trigger", (int)EventTriggerEnum.None == 0, true)]
        public int turnDuration;
        [ConditionalHide("trigger", (int)EventTriggerEnum.None == 0, true)]
        public float coolDown;*/
    }

    [System.Serializable]
    public class ResetEvent
    {
        [Header("Reset Effects:")]
        public EventTriggerEnum trigger;
        [ConditionalHide("trigger", (int)EventTriggerEnum.None == 0, true)]
        public List<EffectOnEventModel> resetOnEvent = new List<EffectOnEventModel>();
        [ConditionalHide("trigger", (int)EventTriggerEnum.None == 0, true)]
        public CharacterStats focusAttribute;
    }

    public class EquipmentManager : MonoBehaviour
    {
        public CharacterManagerGroup baseManager;
        public WeaponModel primaryWeapon;
        public WeaponModel secondaryWeapon;
        public SkillModel classSkill;
        public Bauble bauble;
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
                ((PlayerSkillManager)baseManager.skillManager).classSkillModel = Object.Instantiate( classSkill ) as SkillModel;
            }
        }

        private void CalculateEquipmentPower()
        {
            if (bauble)
            {
                List<EffectOnEventModel> baubleEffects = new List<EffectOnEventModel>();
                foreach (var effect in bauble.effectsOnEvent)
                {
                    baubleEffects.Add(effect);
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
                        primaryWeaponEffects.Add(effect);
                    }
                    foreach (var effect in primaryWeaponEffects)
                    {
                        SetEffectAndRun(effect, weaponEffect);
                    }
                    primaryWeaponEffects = new List<EffectOnEventModel>();
                }

                foreach (var weaponEffect in secondaryWeapon.weaponEffects)
                {
                    foreach (var effect in weaponEffect.effectsOnEvent)
                    {
                        secondaryWeaponEffects.Add(effect);
                    }
                    foreach (var effect in secondaryWeaponEffects)
                    {
                        SetEffectAndRun(effect, weaponEffect);
                    }
                    secondaryWeaponEffects = new List<EffectOnEventModel>();
                }
            }
        }

        private void SetEffectAndRun(EffectOnEventModel effect, Bauble bauble)
        {
            var attrValue = baseManager.characterManager.GetAttributeValue(bauble.FocusAttribute.ToString(), baseManager.characterManager.characterModel);
            var stat = bauble.flatAmount != 0 ? 0 : attrValue;
            effect.effectPower = bauble.flatAmount != 0 ? bauble.flatAmount + attrValue : stat * 0.25f;
            //effect.turnDuration = bauble.turnDuration;
            //effect.trigger = bauble.trigger.ToString();
            effect.triggerChance = bauble.triggerChance;
            effect.FocusAttribute = bauble.FocusAttribute;
            effect.owner = gameObject;
            //effect.dispellable = bauble.dispellable;
            //effect.coolDown = bauble.coolDown;
            effect.target = this.baseManager.characterManager;
            //BattleManager.eventManager.EventAction += effect.RunEffectFromSkill;

            baseManager.EventManagerV2.AddDelegateToEvent(bauble.trigger, effect.RunEffectFromSkill);

            if (bauble.trigger == EventTriggerEnum.Passive)
            {
                baseManager.EventManagerV2.CreateEventOrTriggerEvent(bauble.trigger);
            }
        }

        private void SetEffectAndRun(EffectOnEventModel effect, WeaponEffect weaponEffect)
        {
            //var attrValue = weaponEffect.focusAttribute != CharacterStats.None ? baseManager.characterManager.GetAttributeValue(weaponEffect.focusAttribute.ToString(), baseManager.characterManager.characterModel) : 0;
            //var stat = weaponEffect.flatAmount != 0 ? 0 : attrValue;
            //effect.effectPower = weaponEffect.flatAmount != 0 ? weaponEffect.flatAmount + attrValue : stat * 0.25f;
            //effect.turnDuration = weaponEffect.turnDuration;
            //effect.trigger = weaponEffect.trigger.ToString();
            //effect.triggerChance = weaponEffect.triggerChance;
            //effect.FocusAttribute = weaponEffect.FocusAttribute;
            effect.owner = gameObject;
            //effect.singleStatusGroup = weaponEffect.singleStatusGroup;
            //effect.statusDispellable = weaponEffect.statusDispellable;
            //effect.singleStatusGroupFriendly = weaponEffect.singleStatusGroupFriendly;
            //effect.statusFriendlyDispellable = weaponEffect.statusFriendlyDispellable;
            //effect.dispellable = weaponEffect.dispellable;
            //effect.coolDown = weaponEffect.coolDown;
            effect.target = this.baseManager.characterManager;
            //BattleManager.eventManager.EventAction += effect.RunEffectFromSkill;

            baseManager.EventManagerV2.AddDelegateToEvent(weaponEffect.trigger, effect.RunEffectFromSkill);

            if (weaponEffect.trigger == EventTriggerEnum.Passive)
            {
                baseManager.EventManagerV2.CreateEventOrTriggerEvent(bauble.trigger);
            }
        }

        /*void SetEventAndRunEffect(EffectOnEventModel effect, EventTriggerEnum trigger)
        {
            var eventModel = new EventModel
            {
                eventName = trigger.ToString(),
                extTarget = baseManager.characterManager,
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eventModel);

            //effect.RunEffectFromSkill();
            baseManager.EventManagerV2.CreateEventOrTrigger(
                trigger);
            baseManager.EventManagerV2.AddDelegateToEvent(trigger, effect.RunEffectFromSkill);
            
        }*/
    }
}

