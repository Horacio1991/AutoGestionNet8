using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite

{
    [Serializable]
    public class MenuPermiso
    {
        public string Menu { get; set; }

        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<string> Items { get; set; } = new();
    }
}
