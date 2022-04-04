using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class EventModel
    {
        public string eventName;
        public BaseCharacterManager eventCaller;
        public BaseCharacterManager extTarget;
        public float extraInfo;
        public EventModel()
        {
        }
    }
}

