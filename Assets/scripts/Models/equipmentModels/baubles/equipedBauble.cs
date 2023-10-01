using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class equipedBauble : MonoBehaviour {
        private CharacterManager characterScript;
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
                    var attrValue = characterScript.GetAttributeValue(bauble.focusAttribute, characterScript.characterModel);
                    var stat = bauble.flatAmount != 0 ? 0 : attrValue;
                    effect.power = bauble.flatAmount != 0 ? bauble.flatAmount + attrValue : stat * 0.25f;
                    effect.turnDuration = bauble.turnDuration;
                    effect.trigger = bauble.trigger.ToString();
                    effect.triggerChance = bauble.triggerChance;
                    effect.focusAttribute = bauble.focusAttribute;
                    effect.owner = gameObject;
                    effect.coolDown = bauble.coolDown;
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