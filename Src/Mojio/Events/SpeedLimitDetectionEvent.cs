using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [CollectionNameAttribute(typeof(Event))]
    public class SpeedLimitDetectedEvent : TripEvent
    {
        public SpeedLimitDetectedEvent()
        {
            EventType = Events.EventType.SpeedLimitDetected;
        }

        public int? DetectedSpeedLimit { get; set; }
    }
}
