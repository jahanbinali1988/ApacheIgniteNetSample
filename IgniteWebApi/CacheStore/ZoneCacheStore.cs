using System;
using System.Collections.Generic;
using System.Linq;
using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using IgniteWebApi.Bootstrapper;
using IgniteWebApi.Models;

namespace IgniteWebApi.CacheStore
{
    public class ZoneCacheStore : ICacheStore<string, Zone>
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

        public Zone Load(string key)
        {
            using (var ctx = GetDbContext())
            {
                return ctx.Zones.Find(key);
            }
        }

        public IEnumerable<KeyValuePair<string, Zone>> LoadAll(IEnumerable<string> keys)
        {
            using (var ctx = GetDbContext())
            {
                return keys.Cast<string>().ToDictionary(key => key, key => ctx.Zones.Find(key));
            }
        }

        public void LoadCache(Action<string, Zone> act, params object[] args)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in ctx.Zones)
                {
                    act(item.Id, item);
                }
            }
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }

        public void Write(string key, Zone val)
        {
            using (var ctx = GetDbContext())
            {
                if (ctx.Zones.Any(c => c.Id == (val).Id))
                    ctx.Zones.Update(val);
                else
                    ctx.Zones.Add(val);

                ctx.SaveChanges();
            }
        }

        public void WriteAll(IEnumerable<KeyValuePair<string, Zone>> entries)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in entries)
                {
                    if (ctx.Zones.Any(c => c.Id == ((Zone)item.Value).Id))
                        ctx.Zones.Update((Zone)item.Value);
                    else
                        ctx.Zones.Add((Zone)item.Value);

                    ctx.SaveChanges();
                }
            }
        }
    }

    [Serializable]
    public class ZoneCacheStoreFactory : IFactory<ICacheStore>
    {
        public ICacheStore CreateInstance()
        {
            return new ZoneCacheStore();
        }
    }
}
