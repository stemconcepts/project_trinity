﻿using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class weaponModel : ScriptableObject {
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
        public SkillModel skillTwo;
    	public SkillModel skillThree;
    }
}