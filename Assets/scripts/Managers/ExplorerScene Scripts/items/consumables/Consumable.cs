using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Consumable : ItemBase
    {
        public List<EffectOnEventModel> effectsOnUse = new List<EffectOnEventModel>();
        public bool affectAll;
        public float power;
    }
}