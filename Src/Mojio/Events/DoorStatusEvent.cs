﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    public class DoorStatusEvent : Event
    {
        public DoorStatusEvent()
        {
            EventType = Events.EventType.DoorStatus;
        }

        public Doors DoorsAjar { get; set; }
    }
}
