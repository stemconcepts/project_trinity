using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Damage_Manager : BasicManager
    {
        public DamageModel damageModel { get; set; }
        public Damage_Manager(){
            damageModel = new DamageModel();
        }
    }
}