using System;

namespace AssemblyCSharp
{
    public class Event_Manager
    {
        public delegate void CheckEvent();
        public event CheckEvent EventAction;
        public EventModel eventModel;
        public Event_Manager()
        {
        }

        public void BuildEvent( EventModel eventModel ){
            this.eventModel = eventModel;
            if( EventAction != null )
                EventAction();
        }
    }
}

