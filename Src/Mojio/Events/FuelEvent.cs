using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojio.Events
{
    /// <summary>
    /// Fuel Event
    /// </summary>
    [CollectionNameAttribute(typeof(Event))]
    public class LowFuelEvent : TripEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LowFuelEvent"/> class.
        /// </summary>
        public LowFuelEvent()
        {
            EventType = Events.EventType.LowFuel;
        }
    }
}

