using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class SkillModel : GenericSkillModel
    {
        [Header("Skill Role:")]
    	public ClassEnum Class;
    	public enum ClassEnum{
    		Guardian,
    		Walker,
    		Stalker
    	}
    	public Sprite skillIcon;
    	public bool equipped;
    	public bool learned;
    	public bool assigned;
        public int currentCDAmount;
    }
}
