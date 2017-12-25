using UnityEngine;
using System.Collections;

[System.Serializable]
public class counterEventData{
	public float eventDuration;
	public status bossStatus;
	public status tankStatus;
	public status healerStatus;
	public status dpsStatus;
	//public GameObject target;
}

public class counterEvents : MonoBehaviour {

	public void CountEventAP( counterEventData eventData ){
	//	eventData.bossStatus.StunOn( 0f, false, (eventData.eventDuration/2) );
	//	eventData.tankStatus.HasteOn( 2f, false, eventData.eventDuration );
	//	eventData.healerStatus.HasteOn( 2f, false, eventData.eventDuration );
	//	eventData.dpsStatus.HasteOn( 2f, false, eventData.eventDuration );
		//tankStatus.HasteOn( 2f, false, eventData.eventDuration );
		//healerStatus.HasteOn( 2f, false, eventData.eventDuration );
		//dpsStatus.HasteOn( 2f, false, eventData.eventDuration );
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
