using System;

namespace IgniteWebApi.Dto
{
    public class GeoPointDto
    {
        public Point Point { get; set; }
        public string SerialNumber { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
