using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectOnEvent : ScriptableObject {
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
    public List<singleStatus> singleStatusGroup = new List<singleStatus>();
    public bool statusDispellable = true;
    [Header("Friendly Status Effects:")]
    public List<singleStatus> singleStatusGroupFriendly = new List<singleStatus>();
    public bool statusFriendlyDispellable = true;

    public bool CheckChance( float chance ){
        var chanceNum = Random.Range( 0.0f, 1.0f );
        bool result = chance >= chanceNum ? true : false;
        return result;
    }

    public void RunEffect(){
        if ( EventManager.eventName == trigger.ToString() && owner == EventManager.eventCaller && ready && CheckChance( triggerChance ) ){
            target = affectSelf ? EventManager.eventCaller : EventManager.extTarget;
            var extraPower = EventManager.extraInfo != 0 ? EventManager.extraInfo * power : power;
            var targetDmgCalc = target.GetComponent<calculateDmg>();
            var targetStatus = target.GetComponent<status>();
            switch( effect ) {
            case effectGrp.None:
                    break;
            case effectGrp.Heal:
                    var finalPower = extraPower != 0 ? extraPower : power;
                    targetDmgCalc.calculateHdamage( finalPower );
                    break;
            case effectGrp.Damage:
                    targetDmgCalc.calculateFlatDmg( "event: "+effect.ToString(), power );
                    break;
            case effectGrp.Status:
                    if( singleStatusGroupFriendly.Count > 0 ){
                        for( int i = 0; i < singleStatusGroupFriendly.Count; i++ ){
                            targetStatus.RunStatusFunction( singleStatusGroupFriendly[i], power, duration, stat: focusAttribute );
                        }
                    }
                    if( singleStatusGroup.Count > 0 ){
                        for( int i = 0; i < singleStatusGroup.Count; i++ ){
                            targetStatus.RunStatusFunction( singleStatusGroup[i], power, duration );
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
