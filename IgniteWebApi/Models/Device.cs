using Apache.Ignite.Core.Cache.Configuration;
using System;

namespace IgniteWebApi.Models
{
    [Serializable]
    public class Device
    {
        [QuerySqlField]
        public string Id { get; set; }
        [QuerySqlField]
        public string Name { get; set; }
        [QuerySqlField]
        public string VehicleId { get; set; }
        [QuerySqlField]
        public string SerialNumber { get; set; }
    }
}
