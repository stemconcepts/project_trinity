using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class weaponModel : ScriptableObject {
        public bool enablesAutoAttacks;
    	public string WeaponName;
    	public string DisplayName;
    	public bool owned;
        public Sprite itemIcon;
    	public bool isEquipped;
    	public int itemNumber;
    	[Multiline]
    	public string WeaponDescription;
    	public enum itemQuality {
    		Common,
    		Rare,
    		Epic,
    		Legendary
    	};
    	public itemQuality quality;
        public enum weaponType {
    		bladeAndBoard,
    		heavyHanded,
    		dualBlades,
    		clawAndCannon,
    		glove,
    		cursedGlove
    	};
    	public weaponType type;
        public SkillModel skillOne;
    	public SkillModel skillTwo;
        public SkillModel skillThree;
        public List<WeaponEffect> weaponEffects;
       //public ResetEvent resetEvent;
        /*public triggerGrp trigger;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public List<EffectOnEventModel> effectsOnEvent = new List<EffectOnEventModel>();
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public float triggerChance;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public string focusAttribute;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public float flatAmount;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public int turnDuration;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public float coolDown;
        [ConditionalHide("trigger", (int)triggerGrp.None == 0, true)]
        public bool dispellable;*/
    }
}