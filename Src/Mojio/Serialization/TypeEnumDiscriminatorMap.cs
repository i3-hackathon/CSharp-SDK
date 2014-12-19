using Mojio.Events;
using Mojio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mojio.Serialization
{

    public partial class TypeEnumDiscriminatorMap
    {
        static partial void BuildMap();

        static TypeEnumDiscriminatorMap()
        {
            Maps = new Dictionary<Type, TypeEnumDiscriminatorMap>();

            Map<ObserverToken, Transport>(o => o.Transport)
                .Contains<PubnubObserverToken>(Transport.Pubnub);

            Map<Observer, ObserverType>(o => o.ObserverType)
               .Contains<Observer>(ObserverType.Generic)
               .Contains<EventObserver>(ObserverType.Event)
               .Contains<ScriptObserver>(ObserverType.Script)
               .Contains<GeoFenceObserver>(ObserverType.GeoFence)
               .Contains<ConditionalObserver>(ObserverType.Conditional)
               .Contains<SpeedObserver>(ObserverType.Speed)
               .Contains<RPMObserver>(ObserverType.Rpm)
               .Contains<AccelerationObserver>(ObserverType.Acceleration)
               .Contains<AccelerometerObserver>(ObserverType.Accelerometer)
               .Contains<BatteryVoltageObserver>(ObserverType.BatteryVoltage)
               .Contains<FuelLevelObserver>(ObserverType.FuelLevel)
               .Contains<DistanceObserver>(ObserverType.Distance)
               .Contains<OdometerObserver>(ObserverType.Odometer)
               .Contains<AltitudeObserver>(ObserverType.Altitude)
               .Contains<HeadingObserver>(ObserverType.Heading)
               .Contains<DiagnosticCodeObserver>(ObserverType.Diagnostic)
               .Contains<SmoothVehicleObserver>(ObserverType.SmoothVehicle)
               .Contains<SmoothTripObserver>(ObserverType.SmoothTrip)
               .Contains<SmoothMojioObserver>(ObserverType.SmoothMojio)
               ;

            Map<GuidEntity, EntityType>(e => e.Type)
                .Contains<Token>(EntityType.Token)
                .Contains<Vehicle>(EntityType.Vehicle)
                .Contains<Storage>(EntityType.Storage)
                .Contains<Product>(EntityType.Product)
                .Contains<DTC>(EntityType.DTC)
                .Contains<Trip>(EntityType.Trip)
                .Contains<Invoice>(EntityType.Invoice)
                .Contains<User>(EntityType.User)
                .Contains<Log>(EntityType.Log)
                .Contains<Observer>(EntityType.Observer)
                .Contains<Subscription>(EntityType.Subscription)
                .Contains<Event>(EntityType.Event)
                .Contains<Mojio>(EntityType.Mojio)
                .Contains<App>(EntityType.App)
                .Contains<Access>(EntityType.Access)
                .Contains<SimCard>(EntityType.SimCard)
                .Contains<MojioReport>(EntityType.MojioReport)
                ;

            // The following allows a partial file to continue
            // building the map
            Map<Event, EventType>(e => e.EventType)
                .Contains<BatteryChargingEvent>(EventType.BatteryCharging)
                .Contains<BrakeAssistantEvent>(EventType.BrakeAssistant)
                .Contains<ExperienceControlEvent>(EventType.ExperienceControl)
                .Contains<LaneDepartureEvent>(EventType.LaneDeparture)
                .Contains<NavigationEvent>(EventType.Navigation)
                .Contains<ParkingDistanceControlEvent>(EventType.ParkDistanceControl)
                .Contains<PedestrianAssistantEvent>(EventType.PedestrianAssistant)
                .Contains<TemperatureEvent>(EventType.Temperature)
                .Contains<TurnSignalEvent>(EventType.TurnSignal)
                .Contains<Event>(EventType.DoorStatus)
                .Contains<TripEvent>(EventType.AttentionAssistant)
                .Contains<SpeedLimitDetectedEvent>(EventType.SpeedLimitDetected)
                .Contains<IgnitionEvent>(EventType.IgnitionOn)
                .Contains<IgnitionEvent>(EventType.IgnitionOff)
                .Contains<LowBatteryEvent>(EventType.LowBattery)
                .Contains<FenceEvent>(EventType.FenceEntered)
                .Contains<FenceEvent>(EventType.FenceExited)
                .Contains<TripStatusEvent>(EventType.TripStatus)
                .Contains<Event>(EventType.MILWarning)
                .Contains<TowEvent>(EventType.TowStart)
                .Contains<TowEvent>(EventType.TowStop)
                .Contains<SpeedEvent>(EventType.Speed)
                .Contains<DiagnosticEvent>(EventType.Diagnostic)
                .Contains<OffStatusEvent>(EventType.OffStatus)
                .Contains<AccelerometerEvent>(EventType.Accelerometer)
                .Contains<HeadingChangeEvent>(EventType.HeadingChange)
                .Contains<MileageEvent>(EventType.Mileage)
                .Contains<LowFuelEvent>(EventType.LowFuel)
                .Contains<HeartBeatEvent>(EventType.HeartBeat)
                .Contains<RPMEvent>(EventType.RPM)
                ;
                           
            BuildMap();
            /*Map<Event, EventType>(e => e.EventType)
                .Contains<AccelerationEvent>(EventType.Acceleration)
                .Contains<AccelerometerEvent>(EventType.Accelerometer);*/

        }
    }

}
