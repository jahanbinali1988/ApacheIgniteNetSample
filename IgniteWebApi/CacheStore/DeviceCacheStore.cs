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
    public class DeviceCacheStore : ICacheStore<string, Device>
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

        public Device Load(string key)
        {
            using (var ctx = GetDbContext())
            {
                return ctx.Find<Device>(key);
            }
        }

        public IEnumerable<KeyValuePair<string, Device>> LoadAll(IEnumerable<string> keys)
        {
            using (var ctx = GetDbContext())
            {
                return keys.Cast<string>().ToDictionary(key => key, key => ctx.Devices.Find(key));
            }
        }

        public void LoadCache(Action<string, Device> act, params object[] args)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in ctx.Devices)
                {
                    act(item.SerialNumber, item);
                }
            }
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }

        public void Write(string key, Device val)
        {
            using (var ctx = GetDbContext())
            {
                if (ctx.Devices.Any(c => c.SerialNumber == (val).SerialNumber))
                    ctx.Devices.Update(val);
                else
                    ctx.Devices.Add(val);

                ctx.SaveChanges();
            }
        }

        public void WriteAll(IEnumerable<KeyValuePair<string, Device>> entries)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var item in entries)
                {
                    if (ctx.Devices.Any(c => c.SerialNumber == ((Device)item.Value).SerialNumber))
                        ctx.Devices.Update((Device)item.Value);
                    else
                        ctx.Devices.Add((Device)item.Value);

                    ctx.SaveChanges();
                }
            }
        }
    }

    [Serializable]
    public class DeviceCacheStoreFactory : IFactory<ICacheStore>
    {
        public ICacheStore CreateInstance()
        {
            return new DeviceCacheStore();
        }
    }
}
