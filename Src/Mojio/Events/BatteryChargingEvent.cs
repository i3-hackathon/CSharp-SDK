using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    public enum ChargingStatus
    {
        Disconnected,
        Initial,
        Charging
    }

    [CollectionNameAttribute(typeof(Event))]
    public class BatteryChargingEvent : Event
    {
        public BatteryChargingEvent()
        {
            EventType = EventType.BatteryCharging;
        }

        public ChargingStatus ChargingStatus { get; set; }

        public int ChargingTime { get; set; }
    }
}
