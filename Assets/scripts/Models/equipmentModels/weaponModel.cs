using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.Helpers.Utility;
using System.Linq;
using Assets.scripts.Models.statusModels;
using System;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class WeaponModel : ScriptableObject {
        public bool enablesAutoAttacks;
    	public string DisplayName
        {
            get
            {
                return LabelConverter.ConvertCamelCaseToWord(this.name);
            }
        }
    	public bool owned;
        public Sprite itemIcon;
    	public bool isEquipped;
    	public int itemNumber;
        [TextArea]
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
        public WeaponTalent weaponTalentTree;
    }
}