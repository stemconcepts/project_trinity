using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipedBauble : MonoBehaviour {
    private character_data characterScript;
    public bauble bauble;

    private void CalculatePower(){
        if( bauble ){
            List<EffectOnEvent> baubleEffects = new List<EffectOnEvent>(bauble.effectsOnEvent);
            foreach (var effect in baubleEffects)
            {
                var stat = bauble.flatAmount != 0 ? 0 : characterScript.GetAttributeValue( effect.focusAttribute );
                effect.power = bauble.flatAmount != 0 ? bauble.flatAmount : stat * 0.25f;
                effect.duration = bauble.duration;
                effect.trigger = bauble.trigger.ToString();
                effect.owner = gameObject;
                EventManager.EventAction += effect.RunEffect;
            }
        }
    }

	// Use this for initialization
	void Awake () {
		characterScript = GetComponent<character_data>();
	}

    void Start(){
        CalculatePower();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
