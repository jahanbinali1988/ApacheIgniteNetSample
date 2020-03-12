using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using IgniteWebApi.Bootstrapper;
using IgniteWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IgniteWebApi.CacheStore
{
    public class VehicleCacheStore : ICacheStore<string, Vehicle>
    {
        private static EfIgniteContext GetDbContext()
        {
            return new EfIgniteContext(@"Server=(localdb)\mssqllocaldb;Database=EfIgniteDb;Trusted_Connection=True;ConnectRetryCount=0");
        }

        public void Delete(string key)
        {
            using (var ctx = GetDbContext())
            {
                var blog = ctx.GeoPoints.Find(key);

                if (blog != null)
                {
                    ctx.GeoPoints.Remove(blog);

                    ctx.SaveChanges();
                }
            }
        }

        public void DeleteAll(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Delete(key);
            }
        }

        public Vehicle Load(string key)
        {
            using (var ctx = GetDbContext())
            {
                return ctx.Vehicles.Find(key);
            }
        }

        public IEnumerable<KeyValuePair<string, Vehicle>> LoadAll(IEnumerable<string> keys)
        {
            using (var ctx = GetDbContext())
            {
                return keys.Cast<string>().ToDictionary(key => key, key => ctx.Vehicles.Find(key));
            }
        }

        public void LoadCache(Action<string, Vehicle> act, params object[] args)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in ctx.Vehicles)
                {
                    act(item.Id, item);
                }
            }
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }

        public void Write(string key, Vehicle val)
        {
            using (var ctx = GetDbContext())
            {
                if (ctx.Vehicles.Any(c => c.Id == (val).Id))
                    ctx.Vehicles.Update(val);
                else
                    ctx.Vehicles.Add(val);

                ctx.SaveChanges();
            }
        }

        public void WriteAll(IEnumerable<KeyValuePair<string, Vehicle>> entries)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in entries)
                {
                    if (ctx.Vehicles.Any(c => c.Id == ((Vehicle)item.Value).Id))
                        ctx.Vehicles.Update((Vehicle)item.Value);
                    else
                        ctx.Vehicles.Add((Vehicle)item.Value);

                    ctx.SaveChanges();
                }
            }
        }
    }

    [Serializable]
    public class VehicleCacheStoreFactory : IFactory<ICacheStore>
    {
        public ICacheStore CreateInstance()
        {
            return new VehicleCacheStore();
        }
    }
}
