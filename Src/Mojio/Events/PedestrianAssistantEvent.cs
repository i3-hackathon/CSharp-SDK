﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [CollectionNameAttribute(typeof(Event))]
    public class PedestrianAssistantEvent : TripEvent
    {
        public PedestrianAssistantEvent()
        {
            EventType = Events.EventType.PedestrianAssistant;
        }
    }
}
