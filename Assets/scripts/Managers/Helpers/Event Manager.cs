using System;

namespace AssemblyCSharp
{
    public class Event_Manager
    {
        public delegate void CheckEvent();
        public static event CheckEvent EventAction;
        public Event_Manager()
        {
        }

        public static void BuildEvent( EventModel eventModel ){
            if( EventAction != null )
                EventAction();
        }
    }
}

