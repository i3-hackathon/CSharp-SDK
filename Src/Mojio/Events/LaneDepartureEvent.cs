using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    public enum LaneDeparture
    {
        Unknown,
        Left,
        Right
    }

    [CollectionNameAttribute(typeof(Event))]
    public class LaneDepartureEvent : TripEvent
    {
        public LaneDepartureEvent()
        {
            EventType = Events.EventType.LaneDeparture;
        }

        public LaneDeparture LaneDeparture { get; set; }
    }
}
