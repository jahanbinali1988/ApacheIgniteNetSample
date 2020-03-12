using Apache.Ignite.Core.Cache.Configuration;
using System;

namespace IgniteWebApi.Models
{
    [Serializable]
    public class Vehicle
    {
        [QuerySqlField]
        public string Id { get; set; }
        [QuerySqlField]
        public string Name { get; set; }
        [QuerySqlField]
        public string ZoneId { get; set; }
        
    }
}
