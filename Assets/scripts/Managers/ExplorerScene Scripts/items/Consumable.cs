using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class Consumable : ItemBase
    {
        public List<EffectOnEventModel> effectsOnUse = new List<EffectOnEventModel>();
    }
}