using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class bauble : ScriptableObject {
    public string baubleName;
    public bool owned;
    public bool isEquipped;
    public float flatAmount;
    public float duration;
    public triggerGrp trigger;
    public enum triggerGrp {
        None,
        Passive,
        OnTakingDmg,
        OnDealingDmg,
        OnHeal,
        OnMove
    };
    [Multiline]
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
    public List<EffectOnEvent> effectsOnEvent = new List<EffectOnEvent>();    
}
