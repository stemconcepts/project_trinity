﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class equipedBauble : MonoBehaviour {
        private Character_Manager characterScript;
        public bauble bauble;
    
        private void CalculatePower(){
            if( bauble ){
                List<EffectOnEventModel> baubleEffects = new List<EffectOnEventModel>();
                foreach (var effect in bauble.effectsOnEvent)
                {
                    baubleEffects.Add( Object.Instantiate( effect ) as EffectOnEventModel );
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
                    Battle_Manager.eventManager.EventAction += effect.RunEffect;
                }
            }
        }
    
    	// Use this for initialization
    	void Awake () {
    		characterScript = GetComponent<Character_Manager>();
    	}
    
        void Start(){
            CalculatePower();
        }
    }
}