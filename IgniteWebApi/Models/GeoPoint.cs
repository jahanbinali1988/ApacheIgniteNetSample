using Apache.Ignite.Core.Cache.Configuration;
using NetTopologySuite.Geometries;
using System;

namespace IgniteWebApi.Models
{
    [Serializable]
    public class GeoPoint
    {
        [QuerySqlField]
        public Geometry Location { get; set; }
        [QuerySqlField]
        public string SerialNumber { get; set; }
        [QuerySqlField]
        public DateTime CreateDate { get; set; }
    }
}
