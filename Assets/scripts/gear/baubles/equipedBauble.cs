using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipedBauble : MonoBehaviour {
    private character_data characterScript;
    public bauble bauble;

    private void CalculatePower(){
        if( bauble ){
            List<EffectOnEvent> baubleEffects = new List<EffectOnEvent>();
            foreach (var effect in bauble.effectsOnEvent)
            {
                baubleEffects.Add( Object.Instantiate( effect ) as EffectOnEvent );
            }
        
            foreach (var effect in baubleEffects)
            {
                var attrValue = characterScript.GetAttributeValue( bauble.focusAttribute );
                var stat = bauble.flatAmount != 0 ? 0 : attrValue;
                effect.power = bauble.flatAmount != 0 ? bauble.flatAmount + attrValue : stat * 0.25f;
                effect.duration = bauble.duration;
                effect.trigger = bauble.trigger.ToString();
                effect.triggerChance = bauble.triggerChance;
                effect.focusAttribute = bauble.focusAttribute;
                effect.owner = gameObject;
                effect.coolDown = bauble.coolDown;
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
