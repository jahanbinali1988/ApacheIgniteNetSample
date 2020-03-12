using System;
using System.Collections.Generic;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Multicast;
using Apache.Ignite.Core.PersistentStore;
using IgniteWebApi.Bootstrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IgniteWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<EfIgniteContext>
                (options => options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Environment.SetEnvironmentVariable("IGNITE_HOME", @"C:\IGNITE_HOME");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            FirstValues.GeoPointCacheName = ("GeoPointCache" + DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "");
            FirstValues.ZoneCacheName = ("ZoneCache" + DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "");
            FirstValues.DeviceCacheName = ("DeviceCache" + DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "");
            FirstValues.VehicleCacheName = ("VehicleCache" + DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "");
            FirstValues.IgniteInstanceName = ("ignite-node" + DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "");
            var cfg = new IgniteConfiguration
            {
                JvmDllPath = @"C:\Program Files\Java\jdk-11.0.1\bin\server\jvm.dll",
                IsActiveOnStart = true,
                ClientMode = false,
                IgniteInstanceName = "Test",// FirstValues.IgniteInstanceName,
                WorkDirectory = @"C:\IGNITE_HOME\workspace",
                GridName = Guid.NewGuid().ToString(),
                DataStorageConfiguration = new DataStorageConfiguration()
                {
                    DefaultDataRegionConfiguration = new DataRegionConfiguration()
                    {
                        PersistenceEnabled = true,
                        Name = "inMemoryRegion",
                        //CheckpointPageBufferSize = 1024,
                    },
                    WriteThrottlingEnabled = true
                },
                BinaryConfiguration = new BinaryConfiguration()
                {
                    CompactFooter = false,
                    KeepDeserialized = true
                }
            };
            
            cfg = FirstValues.cacheConfigAll(cfg);
            cfg = FirstValues.setupDiscoveryConfig(cfg);
            Environment.SetEnvironmentVariable("IGNITE_H2_DEBUG_CONSOLE", "true");
            var ignite = Ignition.TryGetIgnite() ?? Ignition.Start(cfg);
            ignite.GetCluster().SetActive(true);
            ignite.SetActive(true);
            var nodes = ignite.GetCluster().ForServers().GetNodes();
            ignite.GetCluster().SetBaselineTopology(nodes);
            DatabaseInitializer.InitializeEfDb();
            DatabaseInitializer.Initializer(ignite);
        }
    }
}
