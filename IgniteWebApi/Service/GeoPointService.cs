using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Query;
using CoordinateSharp;
using IgniteWebApi.Dto;
using IgniteWebApi.Models;
using NetTopologySuite.IO;
using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Services;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Diagnostics;

namespace IgniteWebApi.Service
{
    public class GeoPointService
    {
        private ICache<string, GeoPoint> cache;
        public GeoPointService(ICache<string, GeoPoint> _cache)
        {
            this.cache = _cache;
        }

        public void Create(GeoPointDto Entity)
        {
            Coordinate c1 = new Coordinate(Entity.Point.Latitude, Entity.Point.Longitude);
            var cartesianPoint = c1.Cartesian;

            WKTReader reader = new WKTReader();
            var geoPoint = reader.Read("POINT(" + cartesianPoint.X + " " + cartesianPoint.Y + " " + cartesianPoint.Z + ")");
            const string query = "insert into geoPoint(location, createDate, SerialNumber) values(?, ?, ?)";
            cache.Query(new SqlFieldsQuery(query, false, geoPoint, Entity.CreateDate, Entity.SerialNumber));
        }

        public GeoPointDto Get(string key)
        {
            GeoPointDto result = new GeoPointDto();

            string queryString = "select top(1) Location, createDate, SerialNumber from geoPoint where SerialNumber like (?) order by createDate desc";
            IFieldsQueryCursor query2 = cache.Query(new SqlFieldsQuery(queryString, key));

            var query3 = cache.QueryFields(new SqlFieldsQuery(queryString, key));
            var resQ3 = query3.GetAll();
            
            foreach (var item in resQ3)
            {
                var tempRow = item[0].ToString().Remove(0, 6).Replace("(", "").Replace(")", "").Split(" ");
                Coordinate c = Cartesian.CartesianToLatLong(double.Parse(tempRow[0]), double.Parse(tempRow[1]), double.Parse(tempRow[2]));
                var point = new Dto.Point() {
                    elevation = 0,
                    Latitude = c.Latitude.DecimalDegree,
                    Longitude = c.Longitude.DecimalDegree
                };

                result.Point = point;
                result.CreateDate = DateTime.Parse(item[1].ToString());
                result.SerialNumber = item[2].ToString();
                
            }

            return result;
        }
    }
}
