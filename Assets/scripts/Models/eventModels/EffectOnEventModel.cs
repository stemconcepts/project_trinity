using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssemblyCSharp
{
    public class EffectOnEventModel : ScriptableObject {
        public GameObject owner;
        public BaseCharacterManager target;
        public bool affectSelf;
        public float power;
        public float coolDown;
        public bool ready = true;
        public int turnDuration;
        public bool dispellable;
        public effectGrp effect;
        //private Task effectCDTask;
        public enum effectGrp {
            None,
            Heal,
            Damage,
            Status,
            StatChange,
            EquipmentStatChange,
            Reset
        };
        public string focusAttribute;
        public string trigger;
        public float triggerChance = 1.0f;
        [Header("Status Effects:")]
        public List<SingleStatusModel> singleStatusGroup = new List<SingleStatusModel>();
        public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        public List<SingleStatusModel> singleStatusGroupFriendly = new List<SingleStatusModel>();
        public bool statusFriendlyDispellable = true;
        /*[Header("Reset Effect:")]
        public ResetEvent resetEvent;*/

        public bool CheckChance(float chance)
        {
            var chanceNum = Random.Range(0.0f, 1.0f);
            bool result = chance >= chanceNum ? true : false;
            return result;
        }

        void RunEffect(RoleEnum role)
        {
            switch (effect)
            {
                case effectGrp.None:
                    break;
                case effectGrp.Heal:
                    ExploreManager.AddToSliderHealth((int)power, role);
                    break;
                case effectGrp.Damage:
                    ExploreManager.AddToSliderHealth(-(int)power, role);
                    break;
                case effectGrp.StatChange:
                    var r = new List<RoleEnum>() { role };
                    BattleManager.AddToPlayerStatus(r, singleStatusGroupFriendly);
                    break;
            }
        }


        /// <summary>
        /// Run an effect from a used item
        /// </summary>
        public void RunEffectFromItem(List<RoleEnum> roles)
        {
            if (roles.Count == 0)
            {
                roles = new List<RoleEnum>() { RoleEnum.tank, RoleEnum.healer, RoleEnum.dps };
                roles.ForEach(role =>
                {
                    RunEffect(role);
                });
            } else
            {
                roles.ForEach(role =>
                {
                    RunEffect(role);
                });
            }
        }

        public void RunEffect(){
            if ( BattleManager.eventManager.eventModel.eventName == trigger && owner.name == BattleManager.eventManager.eventModel.eventCaller.name && ready && CheckChance( triggerChance ) ){
                target = affectSelf ? BattleManager.eventManager.eventModel.eventCaller : BattleManager.eventManager.eventModel.extTarget;
                var baseManager = target.GetComponent<BaseCharacterManagerGroup>();
                var extraPower = BattleManager.eventManager.eventModel.extraInfo != 0 ? BattleManager.eventManager.eventModel.extraInfo * power : power;
                var targetDmgCalc = baseManager.damageManager; //add functionality to effect multiple targets
                var targetStatus = baseManager.statusManager;
                PlayerDamageModel dm = new PlayerDamageModel();
                switch( effect ) {
                    case effectGrp.None:
                        break;
                    case effectGrp.Reset:
                        var origAttribute = baseManager.characterManager.GetAttributeValue("original" + focusAttribute, baseManager.characterManager.characterModel);
                        baseManager.characterManager.SetAttribute(focusAttribute, origAttribute, baseManager.characterManager.characterModel);
                        break;
                    case effectGrp.Heal:
                        dm.incomingHeal = extraPower != 0 ? extraPower : power;
                        dm.damageImmidiately = true;
                        targetDmgCalc.calculateHdamage( dm );
                        break;
                    case effectGrp.Damage:
                        dm.incomingDmg = power;
                        dm.skillSource = "event: "+effect.ToString();
                        dm.damageImmidiately = true;
                        targetDmgCalc.calculateFlatDmg( dm );
                        break;
                    case effectGrp.StatChange:
                        baseManager.characterManager.SetAttribute(focusAttribute, power, baseManager.characterManager.characterModel);
                        break;
                    case effectGrp.EquipmentStatChange:
                        baseManager.characterManager.SetAttribute("original" + focusAttribute, power, baseManager.characterManager.characterModel);
                        baseManager.characterManager.SetAttribute(focusAttribute, power, baseManager.characterManager.characterModel);
                        break;
                    case effectGrp.Status:
                        if( singleStatusGroupFriendly.Count > 0 ){
                            for( int i = 0; i < singleStatusGroupFriendly.Count; i++ ){
                                var sm = new StatusModel
                                {
                                    singleStatus = singleStatusGroupFriendly[i],
                                    power = power,
                                    turnDuration = turnDuration,
                                    baseManager = target.baseManager
                                };
                                sm.singleStatus.dispellable = dispellable;
                                targetStatus.RunStatusFunction(sm);
                            }
                        }
                        if( singleStatusGroup.Count > 0 ){
                            for( int i = 0; i < singleStatusGroup.Count; i++ ){
                                var sm = new StatusModel
                                {
                                    singleStatus = singleStatusGroup[i],
                                    power = power,
                                    turnDuration = turnDuration,
                                    baseManager = target.baseManager
                                };
                                sm.singleStatus.dispellable = dispellable;
                                targetStatus.RunStatusFunction(sm);
                            }
                        }
                        break;
                }
                BattleManager.taskManager.CallTask(coolDown, () =>
                {
                    ready = true;
                });
            }
        }

        public void RunResetEffect()
        {
            if (BattleManager.eventManager.eventModel.eventName == trigger && owner.name == BattleManager.eventManager.eventModel.eventCaller.name && ready && CheckChance(triggerChance))
            {
                target = affectSelf ? BattleManager.eventManager.eventModel.eventCaller : BattleManager.eventManager.eventModel.extTarget;
                var baseManager = target.GetComponent<BaseCharacterManagerGroup>();
                var extraPower = BattleManager.eventManager.eventModel.extraInfo != 0 ? BattleManager.eventManager.eventModel.extraInfo * power : power;
                var targetDmgCalc = baseManager.damageManager; //add functionality to effect multiple targets
                var targetStatus = baseManager.statusManager;
                PlayerDamageModel dm = new PlayerDamageModel();
                switch (effect)
                {
                    case effectGrp.None:
                        break;
                    case effectGrp.Reset:
                        var origAttribute = baseManager.characterManager.GetAttributeValue("original" + focusAttribute, baseManager.characterManager.characterModel);
                        baseManager.characterManager.SetAttribute(focusAttribute, origAttribute, baseManager.characterManager.characterModel);
                        break;
                    
                }
                BattleManager.taskManager.CallTask(coolDown, () =>
                {
                    ready = true;
                });
            }
        }

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
        public void LoadEffectData(float power, int turnDuration, int triggerChance, bool ready, effectGrp effectType, bool affectSelf, GameObject owner, float cooldown)
        {
            this.power = power;
            this.turnDuration = turnDuration;
            this.trigger = "OnTakingDmg";
            this.triggerChance = triggerChance; //comes from status
            this.ready = ready;
            effect = effectType;
            this.affectSelf = affectSelf; //comes from status
            this.owner = owner;
            this.coolDown = cooldown;
        }
    }
}