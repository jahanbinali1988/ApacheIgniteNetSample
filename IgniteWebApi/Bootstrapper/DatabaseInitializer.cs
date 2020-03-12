using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Cache.Store;
using IgniteWebApi.Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IgniteWebApi.Bootstrapper
{
    public static class DatabaseInitializer
    {
        public static void Initializer(IIgnite ignite)
        {
            try
            {
                ICache<string, GeoPoint> cacheZone = ignite.GetOrCreateCache<string, GeoPoint>(FirstValues.ZoneCacheCfgReader());
                cacheZone.Query(new SqlFieldsQuery("Drop table IF EXISTS Zone")).GetAll();
                cacheZone.Query(new SqlFieldsQuery("CREATE TABLE IF NOT EXISTS zone (id varchar(MAX) PRIMARY KEY, name varchar(MAX))")).GetAll();
                // streamer
                cacheZone.LoadCache(null);

                ICache<string, Vehicle> cacheVehicle = ignite.GetOrCreateCache<string, Vehicle>(FirstValues.VehicleCacheCfgReader());
                cacheVehicle.Query(new SqlFieldsQuery("Drop table IF EXISTS Vehicle")).GetAll();
                cacheVehicle.Query(new SqlFieldsQuery("CREATE TABLE IF NOT EXISTS Vehicle (Id varchar(MAX), Name varchar(MAX), ZoneId varchar(MAX), PRIMARY KEY (id, ZoneId))")).GetAll();
                cacheVehicle.Query(new SqlFieldsQuery("CREATE INDEX IF NOT EXISTS zone_idx on Vehicle (ZoneId)")).GetAll();
                // streamer
                cacheVehicle.LoadCache(null);

                ICache<string, Device> cacheDevice = ignite.GetOrCreateCache<string, Device>(FirstValues.DeviceCacheCfgReader());
                cacheDevice.Query(new SqlFieldsQuery("Drop table IF EXISTS Device")).GetAll();
                cacheDevice.Query(new SqlFieldsQuery("CREATE TABLE IF NOT EXISTS Device (Id varchar(MAX), Name varchar(MAX), VehicleId varchar(MAX), SerialNumber varchar(MAX), PRIMARY KEY (SerialNumber))")).GetAll();
                cacheDevice.Query(new SqlFieldsQuery("CREATE INDEX IF NOT EXISTS SerialNumber_Device_idx on Device (SerialNumber)")).GetAll();
                // streamer
                cacheDevice.LoadCache(null);

                ICache<string, GeoPoint> cacheGeoPoint = ignite.GetOrCreateCache<string, GeoPoint>(FirstValues.GeoPointCacheCfgReader());
                cacheGeoPoint.Query(new SqlFieldsQuery("DROP TABLE IF EXISTS GeoPoint")).GetAll();
                cacheGeoPoint.Query(new SqlFieldsQuery(
                     "CREATE TABLE IF NOT EXISTS GeoPoint (Location Geometry, createDate TIMESTAMP, SerialNumber varchar(MAX), PRIMARY KEY (SerialNumber))" +
                     "WITH \"template=replicated\"")).GetAll();
                cacheGeoPoint.Query(new SqlFieldsQuery("CREATE INDEX IF NOT EXISTS SerialNumber_GeoPoint_idx on GeoPoint (SerialNumber)")).GetAll();
                // streamer
                cacheGeoPoint.LoadCache(null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void InitializeEfDb()
        {
            using (var ctx = new EfIgniteContext(@"Server=(localdb)\mssqllocaldb;Database=EfIgniteDb;"))
            {
                if (ctx.Database.EnsureCreated())
                {
                    var zone = new Zone
                    {
                        Name = "Ignite Zone",
                        Id = "ZoneId1"
                    };

                    ctx.Zones.Add(zone);
                }
            }
        }
    }
}
