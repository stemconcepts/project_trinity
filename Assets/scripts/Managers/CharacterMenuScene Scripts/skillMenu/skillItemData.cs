using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class skillItemData
	{
		public string skillName;
		public string displayName;
		[Multiline]
		public string skillDetails;
		public string skillCost;
		public enum skillQuality
		{
			Common,
			Rare,
			Epic,
			Legendary
		};
		public enum classType
		{
			guardian,
			stalker,
			walker
		};
		public skillQuality quality;
		public int itemNumber;
		public classType type;
		public bool owned;
		public bool isEquipped;
	}
}