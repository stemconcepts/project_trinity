using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class EffectOnEventModel : ScriptableObject {
        public GameObject owner;
        public GameObject target;
        public bool affectSelf;
        public float power;
        public float coolDown;
        public bool ready = true;
        public float duration;
        public effectGrp effect;
        private Task effectCDTask;
        public enum effectGrp {
            None,
            Heal,
            Damage,
            Status
        };
        public string focusAttribute;
        public string trigger;
        public float triggerChance = 1.0f;
        [Header("Status Effects:")]
        public List<StatusModel> singleStatusGroup = new List<StatusModel>();
        public bool statusDispellable = true;
        [Header("Friendly Status Effects:")]
        public List<StatusModel> singleStatusGroupFriendly = new List<StatusModel>();
        public bool statusFriendlyDispellable = true;
    
        public bool CheckChance( float chance ){
            var chanceNum = Random.Range( 0.0f, 1.0f );
            bool result = chance >= chanceNum ? true : false;
            return result;
        }
    
        public void RunEffect(){
            if ( Battle_Manager.eventManager.eventModel.eventName == trigger.ToString() && owner == Battle_Manager.eventManager.eventModel.eventCaller && ready && CheckChance( triggerChance ) ){
                target = affectSelf ? Battle_Manager.eventManager.eventModel.eventCaller : Battle_Manager.eventManager.eventModel.extTarget;
                var baseManager = target.GetComponent<Base_Character_Manager>();
                var extraPower = Battle_Manager.eventManager.eventModel.extraInfo != 0 ? Battle_Manager.eventManager.eventModel.extraInfo * power : power;
                var targetDmgCalc = baseManager.damageManager;
                var targetStatus = baseManager.statusManager;
                DamageModel dm = new DamageModel(baseManager);
                switch( effect ) {
                case effectGrp.None:
                        break;
                case effectGrp.Heal:
                        dm.incomingHeal = extraPower != 0 ? extraPower : power;
                        targetDmgCalc.calculateHdamage( dm );
                        break;
                case effectGrp.Damage:
                        dm.incomingDmg = power;
                        dm.skillSource = "event: "+effect.ToString();
                        targetDmgCalc.calculateFlatDmg( dm );
                        break;
                case effectGrp.Status:
                        if( singleStatusGroupFriendly.Count > 0 ){
                            for( int i = 0; i < singleStatusGroupFriendly.Count; i++ ){
                                targetStatus.RunStatusFunction( singleStatusGroupFriendly[i] );
                            }
                        }
                        if( singleStatusGroup.Count > 0 ){
                            for( int i = 0; i < singleStatusGroup.Count; i++ ){
                                targetStatus.RunStatusFunction( singleStatusGroup[i] );
                            }
                        }
                        break;
                }
                effectCDTask = new Task( cooldown( coolDown ) );
            }
        }
    
        IEnumerator cooldown(float waitTime)
        {
            ready = false;
            yield return new WaitForSeconds(waitTime);
            ready = true;
        }
    }
}