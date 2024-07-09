using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class equipedBauble : MonoBehaviour {
        private CharacterManager characterScript;
        public Bauble bauble;
    
        private void CalculatePower(){
            if( bauble ){
                List<EffectOnEventModel> baubleEffects = new List<EffectOnEventModel>();
                foreach (var effect in bauble.effectsOnEvent)
                {
                    baubleEffects.Add(effect);
                }
            
                foreach (var effect in baubleEffects)
                {
                    var attrValue = characterScript.GetAttributeValue(bauble.FocusAttribute.ToString(), characterScript.characterModel);
                    var stat = bauble.flatAmount != 0 ? 0 : attrValue;
                    //effect.effectPower = bauble.flatAmount != 0 ? bauble.flatAmount + attrValue : stat * 0.25f;
                    //effect.turnDuration = bauble.turnDuration;
                    //effect.trigger = bauble.trigger.ToString();
                    effect.triggerChance = bauble.triggerChance;
                    effect.FocusAttribute = bauble.FocusAttribute;
                    effect.owner = gameObject;
                    //effect.coolDown = bauble.coolDown;
                    effect.target = characterScript;
                    BattleManager.eventManager.EventAction += effect.RunEffectFromSkill;
                }
            }
        }
    
    	// Use this for initialization
    	void Awake () {
    		characterScript = GetComponent<CharacterManager>();
    	}
    
        void Start(){
            CalculatePower();
        }
    }
}