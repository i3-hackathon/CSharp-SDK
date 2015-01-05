using Mojio.Converters;
using Mojio.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEvent
    {
        
        /// <summary>
        /// mojio Id
        /// </summary>
        Guid MojioId { get; set; }

        /// <summary>
        /// vehicle Id
        /// </summary>
        Guid VehicleId { get; set; }

        /// <summary>
        /// owner Id
        /// </summary>
        Guid? OwnerId { get; set; }

        /// <summary>
        /// event type
        /// </summary>
        EventType EventType { get; set; }

        /// <summary>
        /// event timestamp
        /// </summary>
        DateTime Time { get; set; }

        /// <summary>
        /// location
        /// </summary>
        Location Location { get; set; }

        /// <summary>
        /// accelerometer
        /// </summary>
        Accelerometer Accelerometer { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The gyroscope.
        /// </value>
        Gyroscope Orientation { get; set; }
    }

    /// <summary>
    /// event
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<Event>))]
    public partial class Event : GuidEntity, IEvent, IOwner
    {
        public override EntityType Type
        {
            get { return EntityType.Event; }
        }
        public Event()
        {
        }

        public Event(EventType type)
        {
            EventType = type;
        }

        /// <summary>
        /// mojio Id
        /// </summary>
        [Observable(typeof(Mojio))]
        public Guid MojioId { get; set; }

        /// <summary>
        /// vehicle Id
        /// </summary>
        [Observable(typeof(Vehicle))]
        [Parent(typeof(Vehicle))]
        public Guid VehicleId { get; set; }

        /// <summary>
        /// owner Id
        /// </summary>
        [Observable(typeof(User))]
        [Parent(typeof(User))]
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// event type
        /// </summary>
        public EventType EventType { get; set; }

        [DefaultSort]
        /// <summary>
        /// event timestamp
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// location
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// location
        /// </summary>
        public Accelerometer Accelerometer { get; set; }

        /// <summary>
        /// TimeIsApproximate
        /// </summary>
        public bool? TimeIsApprox { get; set; }

        /// <summary>
        /// Battery Voltage
        /// </summary>
        public double? BatteryVoltage { get; set; }

        /// <summary>
        /// Connection Lost
        /// </summary>
        public bool? ConnectionLost { get; set; }

        /// <summary>
        /// Gets or sets the orientation. [BMW specific]
        /// </summary>
        /// <value>
        /// The gyroscope.
        /// </value>
        public Gyroscope Orientation { get; set; }

        public double? BatteryLevel { get; set; }

        public double? BatteryCurrent { get; set; }

        public double? RainIntensity { get; set; }

        public double? TemperatureInside { get; set; }

        public double? TemperatureOutside { get; set; }

        public object Clone()
        {
            Event clone = this.MemberwiseClone() as Event;
            if (clone.Location != null)
                clone.Location = this.Location.Clone() as Location;
            if (clone.Accelerometer != null)
                clone.Accelerometer = this.Accelerometer.Clone() as Accelerometer;

            return clone;
        }
    }
}
