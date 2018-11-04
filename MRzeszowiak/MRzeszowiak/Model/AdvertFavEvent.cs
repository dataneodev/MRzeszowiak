using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.Model
{
    public class AdvertDeleteFavEvent : PubSubEvent<AdvertShort> { }
    public class AdvertAddFavEvent : PubSubEvent<AdvertShort> { }
}
