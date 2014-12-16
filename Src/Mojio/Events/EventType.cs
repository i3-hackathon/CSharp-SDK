using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    /// <summary>
    /// event type
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// for server-side diagnostics
        /// </summary>
        Log = 1,

        /// <summary>
        /// for device communication session.
        /// </summary>
        Message = 2,

        /// <summary>
        /// ignition on (device)
        /// </summary>
        IgnitionOn = 104,

        /// <summary>
        /// ignition off (device)
        /// </summary>
        IgnitionOff = 105,

        /// <summary>
        /// The battery is charging, or uncharging
        /// </summary>
        BatteryCharging = 106,

        /// <summary>
        /// low battery (device)
        /// </summary>
        LowBattery = 107,

        Temperature = 108,

        TurnSignal = 109,

        DoorStatus = 110,

        AttentionAssistant = 111,

        PedestrianAssistant = 112,

        LaneDeparture = 113,

        BrakeAssistant = 114,

        ExperienceControl = 115,

        ParkDistanceControl = 116,

        Navigation = 117,

        /// <summary>
        /// NOT AN ACTUAL EVENT
        /// </summary>
        //TripEvent = 1005,

        /// <summary>
        /// fence enter (device)
        /// </summary>
        FenceEntered = 1006,

        /// <summary>
        /// fence exit (device)
        /// </summary>
        FenceExited = 1007,

        /// <summary>
        /// trip status (device)
        /// </summary>
        TripStatus = 1010,

        /// <summary>
        /// The off status
        /// </summary>
        OffStatus = 1011,

        /// <summary>
        /// warning NOT AN ACTUAL EVENT
        /// </summary>
        //Warning = 30000,

        /// <summary>
        /// malfunction indicator light warning (device)
        /// </summary>
        MILWarning = 30001,

        /// <summary>
        /// alert NOT AN ACTUAL EVENT
        /// </summary>
        //Alert = 100000,

        /// <summary>
        /// tow start (device)
        /// </summary>
        TowStart = 100002,

        /// <summary>
        /// tow stop (device)
        /// </summary>
        TowStop = 100003,

        /// <summary>
        /// device-defined excessive speed (device)
        /// </summary>
        Speed = 100008,

        /// <summary>
        /// device-defined diagnostics event
        /// </summary>
        Diagnostic = 100009,

        /// <summary>
        /// acceleromter
        /// </summary>
        Accelerometer = 100012,

        /// <summary>
        /// HeadingChange
        /// </summary>
        HeadingChange = 100015,

        /// <summary>
        /// Mileage
        /// </summary>
        Mileage = 100016,

        /// <summary>
        /// LowFuel
        /// </summary>
        LowFuel = 100017,

        /// <summary>
        /// RPM
        /// </summary>
        RPM = 100018,

        /// <summary>
        /// HeartBeat
        /// </summary>
        HeartBeat = 100021,

        /// <summary>
        /// Vehicle Idle Event
        /// </summary>
        IdleEvent = 100023,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = -1


    }
}
