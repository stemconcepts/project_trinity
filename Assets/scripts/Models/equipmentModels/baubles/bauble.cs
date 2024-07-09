using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Bauble : ScriptableObject {
        public string baubleName;
        public bool dispellable;
        public bool owned;
        public Sprite itemIcon;
        public bool isEquipped;
        public CharacterStats FocusAttribute;
        public float flatAmount;
        public int turnDuration;
        public float coolDown;
        public float triggerChance;
        public EventTriggerEnum trigger;
        [TextArea]
        public string baubleDesc;
        public itemQuality quality;
        public enum itemQuality {
            Common,
            Rare,
            Epic,
            Legendary
        };
        public classRestriction classReq;
        public enum classRestriction {
            None,
            Guardian,
            Stalker,
            Walker
        };
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();    
    }
}
