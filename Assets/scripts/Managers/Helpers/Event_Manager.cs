using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Event_Manager : MonoBehaviour
    {
        public delegate void CheckEvent();
        public event CheckEvent EventAction;
        public EventModel eventModel;

        public void BuildEvent( EventModel eventModel ){
            this.eventModel = eventModel;
            if( EventAction != null )
                EventAction();
        }
    }
}

