using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class BasicManager : MonoBehaviour
    {
        public Battle_Details_Manager battleDetailsManager { get; set;}
        public Damage_Manager damageManager { get; set; }
        public BasicManager()
        {
            damageManager = new Damage_Manager();
            battleDetailsManager = new Battle_Details_Manager();
        }
    }
}