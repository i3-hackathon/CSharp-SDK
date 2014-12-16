using Mojio.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojio
{
    public enum GearboxType
    {
        Unknown,
        Manual,
        Automatic,
        Steptronic,
        Sequential,
        DSG
    }
    public enum ModelCountryType
    {
        Unknown,
        ECE,
        US
    }

    public enum EngineType
    {
        Unknown,
        Gasoline,
        GasolineInjection,
        Diesel,
        DieselDirect,
        Hybrid,
        Electric
    }

    public class Gyroscope
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }
    }

    public enum SteeringType
    {
        Unknown,
        RHD,
        LHD
    }

    public enum DrivingExperienceControlStatus
    {
        Unknown,
        Comfort,
        Eco,
        Sport,
        EcoPro,
        SportPlus
    }

    public enum Gears
    {
        Unknown,
        N,
        P,
        R,
        D,
        D1,
        D2,
        First, 
        Second, 
        Third, 
        Fourth
    }

    public enum DrivingDirection
    {
        Unknown,
        Stopped,
        Forwards,
        Backwards,
    }

    [Flags]
    public enum PassengerPresence
    {
        Unknown,
        FrontLeft = 1 << 0,
        FrontRight = 1 << 1,
        RearLeft = 1 << 2,
        RearRight = 1 << 3,
        RearCenter = 1 << 4,
    }

    [Observable]
    public partial class Vehicle : GuidEntity, IOwner, IViewers
    {
        public override EntityType Type {
            get { return EntityType.Vehicle; }
        }

        /// <summary>
        /// Owner ID
        /// </summary>
        [Observable (typeof(User))]
        [Parent (typeof(User))]

        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the mojio identifier.
        /// </summary>
        /// <value>
        /// The mojio identifier.
        /// </value>
        [Observable (typeof(Mojio))]
        public Guid? MojioId { get; set; }

        [DefaultSort]
        /// <summary>
        /// Vehicle name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Vehicle Identification Number
        /// </summary>
        public string VIN { get; set; }

        /// <summary>
        /// License plate
        /// </summary>
        /// <value>The license plate.</value>
        public string LicensePlate { get; set; }
        
        /// <summary>
        /// Gets or sets the series. [BMW Specific]
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public string ModelSeries { get; set; }

        /// <summary>
        /// Gets or sets the type of the model country. [BMW Specific]
        /// </summary>
        /// <value>
        /// The type of the model country.
        /// </value>
        public ModelCountryType ModelCountryType { get; set; }

        /// <summary>
        /// Gets or sets the type of the gearbox. [BMW Specific]
        /// </summary>
        /// <value>
        /// The type of the gearbox.
        /// </value>
        public GearboxType GearboxType { get; set; }

        /// <summary>
        /// Gets or sets the type of the engine. [BMW Specific]
        /// </summary>
        /// <value>
        /// The type of the engine.
        /// </value>
        public EngineType EngineType { get; set; }

        /// <summary>
        /// Gets or sets the type of the steering. [BMW Specific]
        /// </summary>
        /// <value>
        /// The type of the steering.
        /// </value>
        public SteeringType SteeringType { get; set; }

        /// <summary>
        /// Gets or sets the driving experience control status. [BMW Specific]
        /// </summary>
        /// <value>
        /// The driving experience control status.
        /// </value>
        public DrivingExperienceControlStatus LastDrivingExperienceControl { get; set; }

        /// <summary>
        /// Gets or sets the door ajar. [BMW Specific]
        /// </summary>
        /// <value>
        /// The door ajar.
        /// </value>
        public bool? DoorAjar { get; set; }

        /// <summary>
        /// Gets or sets the parking break engaged. [BMW Specific]
        /// </summary>
        /// <value>
        /// The parking break engaged.
        /// </value>
        public bool? ParkingBreakEngaged { get; set; }

        /// <summary>
        /// Gets or sets the current gear. [BMW Specific]
        /// </summary>
        /// <value>
        /// The current gear.
        /// </value>
        public Gears LastGear { get; set; }

        /// <summary>
        /// Gets or sets the last driving direction. [BMW Specific]
        /// </summary>
        /// <value>
        /// The last driving direction.
        /// </value>
        public DrivingDirection LastDrivingDirection { get; set; }

        /// <summary>
        /// Gets or sets the last accelerator pedal. [BMW Specific]
        /// </summary>
        /// <value>
        /// The last accelerator pedal, in percentage.
        /// </value>
        public int? LastAcceleratorPedal { get; set; }

        /// <summary>
        /// Gets or sets the last passenger presence. [BMW Specific]
        /// </summary>
        /// <value>
        /// The last passenger presence.
        /// </value>
        public PassengerPresence LastPassengerPresence { get; set; }

        /// <summary>
        /// Gets or sets the last orientation. [BMW Specific]
        /// </summary>
        /// <value>
        /// The last orientation.
        /// </value>
        public Gyroscope LastOrientation { get; set; }

        /// <summary>
        /// Gets or sets the last battery level. [BMW Specific]
        /// </summary>
        /// <value>
        /// The last battery level.
        /// </value>
        public int? LastBatteryLevel { get; set; }

        /// <summary>
        /// Gets or sets the expected range of the vehicle based on fuel/battery level. [BMW Specific]
        /// </summary>
        /// <value>
        /// The expected range in km.
        /// </value>
        public int? LastRange { get; set; }

        /// <summary>
        /// Optional is ignition on?
        /// </summary>
        public bool? IgnitionOn { get; set; }

        /// <summary>
        /// Most recent trip event
        /// </summary>
        public Guid? LastTripEvent { get; set; }

        /// <summary>
        /// Last location time
        /// </summary>
        public DateTime? LastLocationTime { get; set; }

        /// <summary>
        /// Last known location
        /// </summary>
        public Location LastLocation { get; set; }

        /// <summary>
        /// Last known speed
        /// </summary>
        public double LastSpeed { get; set; }

        /// <summary>
        /// Last known fuel level, in Liters.
        /// </summary>
        public double? FuelLevel { get; set; }

        /// <summary>
        /// Last known acceleration/deceleration
        /// </summary>
        public double? LastAcceleration { get; set; }

        /// <summary>Last Accelerometer Value 
        /// </summary>
        public Accelerometer LastAccelerometer { get; set; }

        /// <summary>
        /// Last known Altitude
        /// </summary>
        public double? LastAltitude { get; set; }

        /// <summary>
        /// Last known Battery Voltage
        /// </summary>
        public double? LastBatteryVoltage { get; set; }

        /// <summary>
        /// Last known Distance
        /// </summary>
        public double? LastDistance { get; set; }

        /// <summary>
        /// Last known Heading
        /// </summary>
        public double? LastHeading { get; set; }

        /// <summary>
        /// Last known Real Odometer Value
        /// </summary>
        public double? LastOdometer { get; set; }

        /// <summary>
        /// Last known RPM
        /// </summary>
        public double? LastRpm { get; set; }

        /// 
        public double LastFuelEfficiency { get; set; }

        /// <summary>
        /// Current trip ID
        /// </summary>
        public Guid? CurrentTrip { get; set; }

        /// <summary>
        /// Current trip ID
        /// </summary>
        public Guid? LastTrip { get; set; }

        /// <summary>
        /// Most recent communication time
        /// </summary>
        public DateTime? LastContactTime { get; set; }

        /// <summary>
        /// Malfunction Indicator Lamp (Check Engine Light)
        /// </summary>
        public bool? MilStatus { get; set; }

        public DTCStatus DiagnosticCodes { get; set; }

        /// <summary>
        /// Public boolean flag to indicate DTC faults have been detected
        /// </summary>
        public bool FaultsDetected { get; set; }

        // TODO: Maybe Viewers should be in private?
        /// <summary>
        /// List of viewer IDs
        /// </summary>
        public Guid[] Viewers { get; set; }

        public object Clone()
        {
            Vehicle clone = this.MemberwiseClone() as Vehicle;
            if (clone.LastLocation!=null)
                clone.LastLocation = this.LastLocation.Clone() as Location;
            if (clone.LastAccelerometer != null)
                clone.LastAccelerometer = this.LastAccelerometer.Clone() as Accelerometer;
            if (clone.DiagnosticCodes != null)
                clone.DiagnosticCodes = this.DiagnosticCodes.Clone() as DTCStatus;
            return clone;
        }
    }
}
