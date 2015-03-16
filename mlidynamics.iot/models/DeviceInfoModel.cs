using System;
using System.Runtime.Serialization;

namespace mlidynamics.iot.models
{
    [DataContract]
    public class DeviceInfoModel
    {
        [DataMember]
        public Guid DeviceId { get; set; }

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public Guid LocationId { get; set; }
    }
}