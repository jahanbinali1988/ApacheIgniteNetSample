using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using IgniteWebApi.Bootstrapper;
using IgniteWebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IgniteWebApi.CacheStore
{
    public class GeoPointCacheStore : ICacheStore<string, GeoPoint>
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

        public GeoPoint Load(string key)
        {
            using (var ctx = GetDbContext())
            {
                return ctx.GeoPoints.Find(key);
            }
        }

        public IEnumerable<KeyValuePair<string, GeoPoint>> LoadAll(IEnumerable<string> keys)
        {
            using (var ctx = GetDbContext())
            {
                return keys.Cast<string>().ToDictionary(key => key, key => ctx.GeoPoints.Find(key));
            }
        }

        public void LoadCache(Action<string, GeoPoint> act, params object[] args)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in ctx.GeoPoints)
                {
                    act(item.SerialNumber, item);
                }
            }
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }

        public void WriteAll(IEnumerable<KeyValuePair<string, GeoPoint>> entries)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in entries)
                {
                    if (ctx.GeoPoints.Any(c => c.SerialNumber == ((GeoPoint)item.Value).SerialNumber))
                        ctx.GeoPoints.Update((GeoPoint)item.Value);
                    else
                        ctx.GeoPoints.Add((GeoPoint)item.Value);

                    ctx.SaveChanges();
                }
            }
        }

        public IDictionary LoadAll(ICollection keys)
        {
            using (var ctx = GetDbContext())
            {
                return keys.Cast<int>().ToDictionary(key => key, key => ctx.GeoPoints.Find(key));
            }
        }

        public void Write(string key, GeoPoint val)
        {
            using (var ctx = GetDbContext())
            {
                if (ctx.GeoPoints.Any(c => c.SerialNumber == (val).SerialNumber))
                    ctx.GeoPoints.Update(val);
                else
                    ctx.GeoPoints.Add(val);

                ctx.SaveChanges();
            }
        }
    }

    [Serializable]
    public class GeoPointCacheStoreFactory : IFactory<ICacheStore>
    {
        public ICacheStore CreateInstance()
        {
            return new GeoPointCacheStore();
        }
    }
}
