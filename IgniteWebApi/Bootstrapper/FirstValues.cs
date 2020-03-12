using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Multicast;
using IgniteWebApi.CacheStore;
using IgniteWebApi.Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IgniteWebApi.Bootstrapper
{
    public static class FirstValues
    {
        private const string FmsCacheName = "fms_cache";
        public static string IgniteInstanceName { get; set; }
        public static string GeoPointCacheName { get; set; }
        public static string ZoneCacheName { get; set; }
        public static string DeviceCacheName { get; set; }
        public static string VehicleCacheName { get; set; }

        public static CacheConfiguration GeoPointCacheCfgReader()
        {
            return new CacheConfiguration(FmsCacheName)
            {
                Name = FirstValues.GeoPointCacheName,
                SqlSchema = "PUBLIC",
                Backups = 1,
                CacheMode = CacheMode.Partitioned,
                WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                AtomicityMode = CacheAtomicityMode.Atomic,
                WriteBehindEnabled = true,
                CacheStoreFactory = new GeoPointCacheStoreFactory(),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false,
                DataRegionName = "inMemoryRegion",
                QueryEntities = new List<QueryEntity>()
                {
                    new QueryEntity() {
                        TableName = GeoPointCacheName,
                        KeyType = typeof(string),
                        KeyFieldName = "SerialNumber",
                        ValueType = typeof(GeoPoint),
                        Fields = new List<QueryField>() {
                            new QueryField("Location", typeof(Geometry)),
                            new QueryField("SerialNumber", typeof(string)),
                            new QueryField("createDate", typeof(DateTime))
                        }
                    }
                }
            };
        }

        public static CacheConfiguration ZoneCacheCfgReader()
        {
            return new CacheConfiguration(FmsCacheName)
            {
                Name = ZoneCacheName,
                SqlSchema = "PUBLIC",
                Backups = 1,
                CacheMode = CacheMode.Partitioned,
                WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                AtomicityMode = CacheAtomicityMode.Atomic,
                WriteBehindEnabled = true,
                CacheStoreFactory = new ZoneCacheStoreFactory(),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false,
                DataRegionName = "inMemoryRegion",
                QueryEntities = new List<QueryEntity>()
                {
                    new QueryEntity() {
                        TableName = ZoneCacheName,
                        KeyType = typeof(string),
                        KeyFieldName = "Id",
                        ValueType = typeof(Zone),
                        Fields = new List<QueryField>() {
                            new QueryField("Id", typeof(string)),
                            new QueryField("Name", typeof(string))
                        }
                    }
                }
            };
        }

        public static CacheConfiguration DeviceCacheCfgReader()
        {
            return new CacheConfiguration(FmsCacheName)
            {
                Name = DeviceCacheName,
                SqlSchema = "PUBLIC",
                Backups = 1,
                CacheMode = CacheMode.Partitioned,
                WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                AtomicityMode = CacheAtomicityMode.Atomic,
                WriteBehindEnabled = true,
                CacheStoreFactory = new DeviceCacheStoreFactory(),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false,
                DataRegionName = "inMemoryRegion",
                QueryEntities = new List<QueryEntity>()
                {
                    new QueryEntity()
                    {
                        TableName = DeviceCacheName,
                        KeyType = typeof(string),
                        KeyFieldName = "Id",
                        ValueType = typeof(Device),
                        Fields = new List<QueryField>() {
                                new QueryField("Id", typeof(string)),
                                new QueryField("Name", typeof(string)),
                                new QueryField("VehicleId", typeof(string)),
                                new QueryField("SerialNumber", typeof(string))
                        }
                    }
                }
            };
        }

        public static CacheConfiguration VehicleCacheCfgReader()
        {
            return new CacheConfiguration(FmsCacheName)
            {
                Name = VehicleCacheName,
                SqlSchema = "PUBLIC",
                Backups = 1,
                CacheMode = CacheMode.Partitioned,
                WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                AtomicityMode = CacheAtomicityMode.Atomic,
                WriteBehindEnabled = true,
                CacheStoreFactory = new VehicleCacheStoreFactory(),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false,
                DataRegionName = "inMemoryRegion",                
                QueryEntities = new List<QueryEntity>()
                {
                    new QueryEntity()
                    {
                        TableName = VehicleCacheName,
                        KeyType = typeof(string),
                        KeyFieldName = "Id",
                        ValueType = typeof(Vehicle),
                        Fields = new List<QueryField>() {
                                new QueryField("Id", typeof(string)) { IsKeyField = true },
                                new QueryField("Name", typeof(string)),
                                new QueryField("ZoneId", typeof(string))
                        }
                    }
                }
            };
        }

        public static IgniteConfiguration cacheConfigAll(IgniteConfiguration config)
        {
            CacheConfiguration cacheConfigVehicle = VehicleCacheCfgReader();

            CacheConfiguration cacheConfigDevice = DeviceCacheCfgReader();

            CacheConfiguration cacheConfigZone = ZoneCacheCfgReader();

            CacheConfiguration cacheConfigGeoPoint = GeoPointCacheCfgReader();

            List<CacheConfiguration> list = new List<CacheConfiguration>();
            list.Add(cacheConfigVehicle);
            list.Add(cacheConfigDevice);
            list.Add(cacheConfigZone);
            list.Add(cacheConfigGeoPoint);
            config.CacheConfiguration = list;
            return config;
        }

        public static IgniteConfiguration setupDiscoveryConfig(IgniteConfiguration config)
        {
            TcpDiscoverySpi spi = new TcpDiscoverySpi();
            var ipFinder = new TcpDiscoveryMulticastIpFinder();
            ((TcpDiscoveryMulticastIpFinder)ipFinder).MulticastGroup = "228.10.10.157";
            ipFinder.LocalAddress = "127.0.0.1";
            spi.IpFinder = ipFinder;
            config.DiscoverySpi = spi;
            return config;
        }
    }
}
