using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class skillData {
	public string skillName;
	public string displayName;
	[Multiline]
	public string skillDetails;
	public string skillCost;
}

[System.Serializable]
public class itemData {
	public string itemName;
	public string displayName;
	[Multiline]
	public string itemDetails;
	public enum itemQuality {
		Common,
		Rare,
		Epic,
		Legendary
	};
	public enum weaponType {
		bladeAndBoard,
		heavyHanded,
		dualBlades,
		clawAndPistol,
		glove,
		cursedGlove
	};
	public itemQuality quality;
	public int itemNumber;
	public weaponType type;
	//public int amount;
	public bool owned;
	public bool isEquipped;
	//private string skillList;
	public List<skillData> attachedSkills = new List<skillData>();

	void Start(){

	}

}
