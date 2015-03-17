using System;
using System.Runtime.Serialization;

namespace mlidynamics.iot.models
{
    [DataContract]
    public class TelemetryModel
    {
        [DataMember]
        public Guid DeviceId { get; set; }

        [DataMember]
        public string Channel { get; set; }

        [DataMember]
        public long Location { get; set; }

        [DataMember]
        public double Temperature { get; set; }

        [DataMember]
        public double Humidity { get; set; }

        [DataMember]
        public short PresenceDetected { get; set; }
    }
}