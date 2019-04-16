using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Button_Click_Manager : BasicManager
    {
        private Task holdTimeTask { get; set; }
        public selectionOverlapControl selectionOverlapScript {get; set;}
        public Character_Interaction_Manager characterInteractions {get; set;}
        public Character_Manager characterManager {get; set;}
    }
}

