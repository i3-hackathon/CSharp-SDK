﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [Flags]
    public enum TurnSignals
    {
        Off = 0,

        Right = 1 << 0,
        Left = 1 << 1,

        Hazard = Right | Left
    }

    [CollectionNameAttribute(typeof(Event))]
    public class TurnSignalEvent : Event
    {
        public TurnSignalEvent()
        {
            EventType = EventType.TurnSignal;
        }

        public TurnSignals TurnSignal { get; set; }
    }
}
