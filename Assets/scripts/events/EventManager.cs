using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    public delegate void CheckEvent();
    public static event CheckEvent EventAction;
    public static string eventName;
    public static GameObject eventCaller;
    public static GameObject extTarget;

    public static void BuildEvent( string eventNameVar, GameObject extTargetVar, GameObject eventCallerVar){
        eventName = eventNameVar;
        extTarget = extTargetVar; 
        eventCaller = eventCallerVar;
        if( EventAction != null )
            EventAction();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
