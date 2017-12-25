using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectOnEvent : ScriptableObject {
    public GameObject owner;
    public GameObject target;
    public bool affectSelf;
    public float power;
    public string focusAttribute;
    public float coolDown;
    public float duration;
    public effectGrp effect;
    public enum effectGrp {
        None,
        Heal,
        Damage,
        Status
    };
    public string trigger;
    [Header("Status Effects:")]
    public List<singleStatus> singleStatusGroup = new List<singleStatus>();
    public bool statusDispellable = true;
    [Header("Friendly Status Effects:")]
    public List<singleStatus> singleStatusGroupFriendly = new List<singleStatus>();
    public bool statusFriendlyDispellable = true;

    public void RunEffect(){
        if ( EventManager.eventName == trigger.ToString() && owner == EventManager.eventCaller ){
            target = affectSelf ? EventManager.eventCaller : EventManager.extTarget;
            var targetDmgCalc = target.GetComponent<calculateDmg>();
            var targetStatus = target.GetComponent<status>();
            switch( effect ) {
            case effectGrp.None:
                    break;
            case effectGrp.Heal:
                    targetDmgCalc.calculateHdamage( power );
                    break;
            case effectGrp.Damage:
                    targetDmgCalc.calculateFlatDmg( "event: "+effect.ToString(), power );
                    break;
            case effectGrp.Status:
                    if( singleStatusGroupFriendly.Count > 0 ){
                        for( int i = 0; i < singleStatusGroupFriendly.Count; i++ ){
                            targetStatus.RunStatusFunction( singleStatusGroupFriendly[i], power, duration );
                        }
                    }
                    if( singleStatusGroup.Count > 0 ){
                        for( int i = 0; i < singleStatusGroup.Count; i++ ){
                            targetStatus.RunStatusFunction( singleStatusGroup[i], power, duration );
                        }
                    }
                    break;
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
