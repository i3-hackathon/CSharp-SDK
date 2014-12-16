using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [CollectionNameAttribute(typeof(Event))]
    public class TemperatureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeedEvent"/> class.
        /// </summary>
        public TemperatureEvent()
        {
            EventType = Events.EventType.Temperature;
        }
    }
}
