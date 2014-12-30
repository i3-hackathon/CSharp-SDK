using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [CollectionNameAttribute(typeof(Event))]
    public class ParkingDistanceControlEvent : TripEvent
    {
        public ParkingDistanceControlEvent()
        {
            EventType = Events.EventType.ParkingDistanceControl;
        }

        public int? DistanceFront { get; set; }

        public int? DistanceRear { get; set; }
    }
}
