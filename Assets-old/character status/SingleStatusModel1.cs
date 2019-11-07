using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class SingleStatusModel1 : ScriptableObject
    {
        public string attributeName;
        public string statusName;
        public string displayName;
        public string statusDesc;
        public Sprite labelIcon;
        public bool canStack;
        public int maxStacks = 3;
        public bool debuffable;
        public bool buff;
        public float buffpower;
        public int statusposition;
        public bool active;
        public string hitAnim;
        public string holdAnim;
    }
}

