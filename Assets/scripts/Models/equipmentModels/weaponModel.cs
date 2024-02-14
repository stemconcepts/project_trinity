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
    }
}