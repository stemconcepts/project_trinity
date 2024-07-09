using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AssemblyCSharp
{
    public enum EffectGrpEnum
    {
        None,
        Heal,
        Damage,
        Status,
        StatChange,
        EquipmentStatChange,
        EditCorruption,
        Reset
    };

    [Serializable]
    public class EffectOnEventModel {
        [HideInInspector]
        public GameObject owner;
        [HideInInspector]
        public BaseCharacterManager target;
        //public bool affectSelf;
        [Tooltip("Dont use apply to Status")]
        [ConditionalHide("effect", (int)EffectGrpEnum.None, true)]
        public float effectPower;
        //public float coolDown;
        //public bool ready = true;
        //public int turnDuration;
        //public bool dispellable;
        public Action eventAction;
        public EffectGrpEnum EffectGrp;
        [ConditionalHide("effect", (int)EffectGrpEnum.StatChange, false)]
        public CharacterStats FocusAttribute;
        //public string trigger;
        [ConditionalHide("effect", (int)EffectGrpEnum.None, true)]
        [Range(0.0f,1.0f)]
        public float triggerChance = 1.0f;
        [Header("Status Effects:")]
        //public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        [Tooltip("Dont use if Effect is not Status")]
        public List<StatusItem> singleStatusGroup = new List<StatusItem>();
        //public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        [Tooltip("Dont use if Effect is not Status")]
        //public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        public List<StatusItem> singleStatusGroupFriendly = new List<StatusItem>();
        //public bool statusFriendlyDispellable = true;

        void RunEffectFromItemToRole(RoleEnum role, ItemBase item)
        {
            switch (EffectGrp)
            {
                case EffectGrpEnum.None:
                    break;
                case EffectGrpEnum.Heal:
                    MainGameManager.instance.exploreManager.AddToSliderHealth((int)effectPower, role);
                    break;
                case EffectGrpEnum.Damage:
                    MainGameManager.instance.exploreManager.AddToSliderHealth(-(int)effectPower, role);
                    break;
                case EffectGrpEnum.EditCorruption:
                    MainGameManager.instance.exploreManager.EditCurroption((int)effectPower);
                    break;
                case EffectGrpEnum.StatChange:
                    var roles = new List<RoleEnum>() { role };
                    BattleManager.AddToPlayerStatus(roles, singleStatusGroupFriendly);
                    break;
            }
            MainGameManager.instance.exploreManager.RemoveObtainedItem(item);
        }


        /// <summary>
        /// Run an effect from a used item
        /// </summary>
        public void RunEffectFromItemToRoles(List<RoleEnum> roles, ItemBase item)
        {
            if (roles.Count == 0)
            {
                roles = new List<RoleEnum>() { RoleEnum.tank, RoleEnum.healer, RoleEnum.dps };
                roles.ForEach(role =>
                {
                    RunEffectFromItemToRole(role, item);
                });
            }
            else
            {
                roles.ForEach(role =>
                {
                    RunEffectFromItemToRole(role, item);
                });
            }
        }

        public void RunEffectFromSkill(){
            /*if ( BattleManager.eventManager.eventModel.eventName == trigger && owner.name == BattleManager.eventManager.eventModel.eventCaller.name && ready 
                && MainGameManager.instance.GetChanceByPercentage(triggerChance)*/
            if (MainGameManager.instance.GetChanceByPercentage(triggerChance))
            {
               // target = affectSelf ? BattleManager.eventManager.eventModel.eventCaller : BattleManager.eventManager.eventModel.extTarget;
                var baseManager = target.GetComponent<BaseCharacterManagerGroup>();
                //var extraPower = BattleManager.eventManager.eventModel.extraInfo != 0 ? BattleManager.eventManager.eventModel.extraInfo * power : power;
                var targetDmgCalc = baseManager.damageManager; //add functionality to effect multiple targets
                var targetStatus = baseManager.statusManager;
                PlayerDamageModel dm = new PlayerDamageModel();
                switch( EffectGrp ) {
                    case EffectGrpEnum.None:
                        break;
                    case EffectGrpEnum.Reset:
                        var origAttribute = baseManager.characterManager.GetAttributeValue("original" + FocusAttribute, baseManager.characterManager.characterModel);
                        baseManager.characterManager.SetAttribute(FocusAttribute.ToString(), origAttribute, baseManager.characterManager.characterModel);
                        break;
                    case EffectGrpEnum.Heal:
                        dm.incomingHeal = effectPower;
                        dm.damageImmidiately = true;
                        targetDmgCalc.calculateHdamage( dm );
                        break;
                    case EffectGrpEnum.Damage:
                        dm.incomingDmg = effectPower;
                        dm.skillSource = "event: "+EffectGrp.ToString();
                        dm.damageImmidiately = true;
                        targetDmgCalc.calculatedamage( dm );
                        break;
                    case EffectGrpEnum.StatChange:
                        baseManager.characterManager.SetAttribute(FocusAttribute.ToString(), effectPower, baseManager.characterManager.characterModel);
                        break;
                    case EffectGrpEnum.EquipmentStatChange:
                        baseManager.characterManager.SetAttribute("original" + FocusAttribute, effectPower, baseManager.characterManager.characterModel);
                        baseManager.characterManager.SetAttribute(FocusAttribute.ToString(), effectPower, baseManager.characterManager.characterModel);
                        break;
                    case EffectGrpEnum.Status:
                        if( singleStatusGroupFriendly.Count > 0 ){
                            foreach (var statusItem in singleStatusGroupFriendly)
                            {
                                var sm = new StatusModel
                                {
                                    singleStatus = statusItem.status,
                                    power = statusItem.power,
                                    turnDuration = statusItem.duration,
                                    baseManager = target.baseManager
                                };
                                sm.singleStatus.dispellable = statusItem.dispellable;
                                targetStatus.RunStatusFunction(sm);
                            }
                        }
                        if( singleStatusGroup.Count > 0 ){
                            foreach (var statusItem in singleStatusGroup)
                            {
                                var sm = new StatusModel
                                {
                                    singleStatus = statusItem.status,
                                    power = statusItem.power,
                                    turnDuration = statusItem.duration,
                                    baseManager = target.baseManager
                                };
                                sm.singleStatus.dispellable = statusItem.dispellable;
                                targetStatus.RunStatusFunction(sm);
                            }
                        }
                        break;
                }
            }
        }

        /*public void RunResetEffect()
        {
            if (BattleManager.eventManager.eventModel.eventName == trigger && owner.name == BattleManager.eventManager.eventModel.eventCaller.name && ready 
                && MainGameManager.instance.GetChanceByPercentage(triggerChance))
            {
                Debug.Log("worked");
                target = affectSelf ? BattleManager.eventManager.eventModel.eventCaller : BattleManager.eventManager.eventModel.extTarget;
                var baseManager = target.GetComponent<BaseCharacterManagerGroup>();
                var extraPower = BattleManager.eventManager.eventModel.extraInfo != 0 ? BattleManager.eventManager.eventModel.extraInfo * power : power;
                var targetDmgCalc = baseManager.damageManager; //add functionality to effect multiple targets
                var targetStatus = baseManager.statusManager;
                PlayerDamageModel dm = new PlayerDamageModel();
                switch (effect)
                {
                    case EffectGrpEnum.None:
                        break;
                    case EffectGrpEnum.Reset:
                        var origAttribute = baseManager.characterManager.GetAttributeValue("original" + focusAttribute, baseManager.characterManager.characterModel);
                        baseManager.characterManager.SetAttribute(focusAttribute, origAttribute, baseManager.characterManager.characterModel);
                        break;
                    
                }
                BattleManager.taskManager.CallTask(coolDown, () =>
                {
                    ready = true;
                });
            }
        }*/

        /// <summary>
        /// Load the starting data for an effectevent
        /// </summary>
        /// <param name="power"></param>
        /// <param name="turnDuration"></param>
        /// <param name="triggerChance"></param>
        /// <param name="ready"></param>
        /// <param name="effectType"></param>
        /// <param name="affectSelf"></param>
        /// <param name="owner"></param>
        /// <param name="cooldown"></param>
        public void LoadEffectData(float power, int turnDuration, int triggerChance, bool ready, EffectGrpEnum effectType, bool affectSelf, GameObject owner, float cooldown)
        {
            this.effectPower = power;
            //this.turnDuration = turnDuration;
            //this.trigger = "OnTakingDmg";
            this.triggerChance = triggerChance; //comes from status
           // this.ready = ready;
            EffectGrp = effectType;
           // this.affectSelf = affectSelf; //comes from status
            this.owner = owner;
            //this.coolDown = cooldown;
        }
    }
}