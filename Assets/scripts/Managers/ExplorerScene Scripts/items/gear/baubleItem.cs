using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class baubleItem : ItemBase
    {
        public bauble bauble;
        [Range(0.0f, 1.0f)]
        public float dropChancePercentage;
    }
}
