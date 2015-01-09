using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojio.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CollectionNameAttribute(typeof(Event))]
    public class AccidentEvent : TripEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccidentEvent"/> class.
        /// </summary>
        public AccidentEvent()
        {
            EventType = Events.EventType.Accident;
        }
    }
}
