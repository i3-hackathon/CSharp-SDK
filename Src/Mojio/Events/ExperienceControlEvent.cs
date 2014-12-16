﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [CollectionNameAttribute(typeof(Event))]
    public class ExperienceControlEvent : Event
    {
        public ExperienceControlEvent()
        {
            EventType = Events.EventType.ExperienceControl;
        }
    }
}
