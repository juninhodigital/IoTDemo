using System;
using System.Runtime.Serialization;

namespace IoT.Demo.SendEvents
{
    [DataContract]
    public class Event
    {
        #region| Properties |

        [DataMember]
        public DateTime EventProcessedUtcTime { get; set; }

        [DataMember]
        public int PartitionId { get; set; }

        [DataMember]
        public DateTime EventEnqueuedUtcTime { get; set; }

        [DataMember]
        public DateTime TimeStamp1 { get; set; }

        [DataMember]
        public string Temperature { get; set; }

        [DataMember]
        public string Pressure { get; set; }

        [DataMember]
        public string DeviceId { get; set; } 

        #endregion
    }
}