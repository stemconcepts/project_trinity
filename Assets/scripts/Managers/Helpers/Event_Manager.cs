using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public enum EventTriggerEnum
    {
        None,
        Passive,
        OnTakingDmg,
        OnDealingDmg,
        OnEyeSkillCast,
        OnHeal,
        OnMove,
        OnSkillCast,
        OnFirstRow,
        OnMiddleRow,
        OnLastRow,
        OnAction
    };

    public enum GenericEventEnum
    {
        None,
        GameOver,
        EquipmentReady
    };

    public class Event_Manager : MonoBehaviour
    {
        public delegate void CheckEvent();
        public event CheckEvent EventAction;
        public EventModel eventModel;

        public void BuildEvent( EventModel eventModel ){
            this.eventModel = eventModel;

            if( EventAction != null)
            {
                EventAction();
            }
        }
    }

    public class GenericEventManager<T>
    {
        List<GenericEventModel<T>> BattleEventModels = new List<GenericEventModel<T>>();
        public GenericEventModel<T> CreateGenericEventOrTriggerEvent(T eventTrigger)
        {
            GenericEventModel<T> eventModel = null;
            if (!HasEvent(eventTrigger))
            {
                eventModel = new GenericEventModel<T>();
                eventModel.GenericTrigger = eventTrigger;
                BattleEventModels.Add(eventModel);
            }
            else
            {
                eventModel = BattleEventModels.Find(eventItem => eventItem.GenericTrigger.Equals(eventTrigger));
            }

            eventModel?.RunEvent();
            return eventModel;
        }

        public bool HasEvent(T eventTrigger)
        {
            var relevantEvent = BattleEventModels.Find(eventItem => eventItem.GenericTrigger.Equals(eventTrigger));
            return relevantEvent != null;
        }

        public void AddDelegateToEvent(T eventTrigger, Action method, bool force = false)
        {
            var relevantEvent = BattleEventModels.Find(eventItem => eventItem.GenericTrigger.Equals(eventTrigger));
            if (!relevantEvent.HasEvent() || force)
            {
                relevantEvent.Event += method;
            }
        }

        public void RemoveDelegateFromEvent(T eventTrigger, Action method)
        {
            var relevantEvent = BattleEventModels.Find(eventItem => eventItem.GenericTrigger.Equals(eventTrigger));
            relevantEvent.Event -= method;
        }
    }

    public class EventManagerV2
    {
        List<EventModelV2> EventModels = new List<EventModelV2>();

        public EventModelV2 CreateEventOrTriggerEvent(EventTriggerEnum trigger)
        {
            EventModelV2 eventModel = null;
            if (!HasEvent(trigger))
            {
                eventModel = new EventModelV2();
                eventModel.Trigger = trigger;
                EventModels.Add(eventModel);
            } else
            {
                eventModel = EventModels.Find(eventItem => eventItem.Trigger == trigger);
            }

            eventModel?.RunEvent();
            return eventModel;
        }

        public bool HasEvent(EventTriggerEnum trigger)
        {
            var relevantEvent = EventModels.Find(eventItem => eventItem.Trigger == trigger);
            return relevantEvent != null;
        }

        public void AddDelegateToEvent(EventTriggerEnum trigger, Action method, bool force = false)
        {
            if (HasEvent(trigger))
            {
                var relevantEvent = EventModels.Find(eventItem => eventItem.Trigger == trigger);
                if (!relevantEvent.HasEvent() || force)
                {
                    relevantEvent.Event += method;
                }
            } else
            {
                var eventModel = CreateEventOrTriggerEvent(trigger);
                eventModel.Event += method;
            }
        }

        public void RemoveDelegateFromEvent(EventTriggerEnum trigger, Action method)
        {
            var relevantEvent = EventModels.Find(eventItem => eventItem.Trigger == trigger);
            relevantEvent.Event -= method;
        }
    }

    public class EventModelV2
    {
        public EventTriggerEnum Trigger;
        public event Action Event;

        public void RunEvent()
        {
            Event?.Invoke();
        }

        public bool HasEvent()
        {
            return Event != null;
        }
    }

    public class GenericEventModel<T>
    {
        public T GenericTrigger;
        public event Action Event;

        public void RunEvent()
        {
            Event?.Invoke();
        }

        public bool HasEvent()
        {
            return Event != null;
        }
    }
}

