using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Events
{
    [CollectionNameAttribute(typeof(Event))]
    public class NavigationEvent : Event
    {
        public NavigationEvent()
        {
            EventType = Events.EventType.Navigation;
        }

        public Location DestinationLocation { get; set; }

        public Address DestinationAddress { get; set; }
    }
}
