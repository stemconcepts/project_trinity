using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Button_Click_Manager : BasicManager
    {
        private Task holdTimeTask;
        public selectionOverlapControl selectionOverlapScript;
        public Character_Interaction_Manager characterInteractions;
        public Character_Manager characterManager;
    }
}

