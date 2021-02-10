using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class EffectOnEventModel : ScriptableObject {
        public GameObject owner;
        public Character_Manager target;
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
            StatChange
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
    
        public bool CheckChance( float chance ){
            var chanceNum = Random.Range( 0.0f, 1.0f );
            bool result = chance >= chanceNum ? true : false;
            return result;
        }

        public void RunEffect(){
            if ( Battle_Manager.eventManager.eventModel.eventName == trigger && owner.name == Battle_Manager.eventManager.eventModel.eventCaller.name && ready && CheckChance( triggerChance ) ){
                target = affectSelf ? Battle_Manager.eventManager.eventModel.eventCaller : Battle_Manager.eventManager.eventModel.extTarget;
                var baseManager = target.GetComponent<Base_Character_Manager>();
                var extraPower = Battle_Manager.eventManager.eventModel.extraInfo != 0 ? Battle_Manager.eventManager.eventModel.extraInfo * power : power;
                var targetDmgCalc = baseManager.damageManager; //add functionality to effect multiple targets
                var targetStatus = baseManager.statusManager;
                DamageModel dm = new DamageModel();
                switch( effect ) {
                case effectGrp.None:
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
                        baseManager.characterManager.SetAttribute(focusAttribute, power);
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
                Battle_Manager.taskManager.CallTask(coolDown, () =>
                {
                    ready = true;
                });
            }
        }
    
        /*IEnumerator cooldown(float waitTime)
        {
            ready = false;
            yield return new WaitForSeconds(waitTime);
            ready = true;
        }*/
    }
}