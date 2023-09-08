using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace task1
{
    [Serializable]
    [XmlRoot("channel")]
    public class channel
    {
        [XmlElement("item")]
        public List<item> items { get; set; }
    }
}
