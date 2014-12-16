using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    public enum BrakeAssistant
    {
        None,
        Tension,
        Emergency
    }

    [CollectionNameAttribute(typeof(Event))]
    public class BrakeAssistantEvent : TripEvent
    {
        public BrakeAssistantEvent()
        {
            EventType = Events.EventType.BrakeAssistant;
        }

        public BrakeAssistant BrakeAssistant { get; set; }
    }
}
