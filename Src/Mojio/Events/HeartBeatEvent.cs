using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojio.Events
{
    /// <summary>
    /// HeartBeat Event
    /// </summary>
    [CollectionNameAttribute(typeof(Event))]
    public class HeartBeatEvent : TripEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeartBeatEvent"/> class.
        /// </summary>
        public HeartBeatEvent()
            : this(EventType.HeartBeat)
        {
            
        }

        public HeartBeatEvent(EventType type)
            : base(type)
        {

        }

        /// <summary>
        /// VIN
        /// </summary>
        public string VIN { get; set; }

        public string ModelSeries { get; set; }

        public ModelCountryType? ModelCountryType { get; set; }

        public GearboxType? GearboxType { get; set; }

        public EngineType? EngineType { get; set; }

        public SteeringType? SteeringType { get; set; }

        public DrivingExperienceControlStatus? DrivingExperienceControl { get; set; }
    }
}
