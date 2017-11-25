using UnityEngine;
using System.Collections;

[System.Serializable]
public class weapons : ScriptableObject {
	public string WeaponName;
	public string DisplayName;
	public bool owned;
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
	//public classSkills skillOne;
    public classSkills skillTwo;
	public classSkills skillThree;
}
