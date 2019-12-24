using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StatusModel 
    {
        public SingleStatusModel singleStatus;
        public Base_Character_Manager baseManager; 
        public SkillModel onHitSkill; 
        public float duration;
        public float power;
        public bool turnOff;
        public float triggerChance;
        public int stacks;
    }
}
