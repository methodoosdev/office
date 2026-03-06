using System.Collections.Generic;
using System.Xml.Serialization;

namespace App.Framework.Models
{
    public partial record BaseNopModel
    {
        public BaseNopModel()
        {
            CustomProperties = new Dictionary<string, object>();
        }

        [XmlIgnore]
        public Dictionary<string, object> CustomProperties { get; set; }

        [XmlIgnore]
        public string __entityType => this.GetType().Name;
    }
}