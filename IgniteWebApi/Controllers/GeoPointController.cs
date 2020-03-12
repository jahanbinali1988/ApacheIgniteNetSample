using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.PersistentStore;
using Microsoft.AspNetCore.Mvc;
using IgniteWebApi.Models;
using IgniteWebApi.Bootstrapper;
using IgniteWebApi.Service;
using IgniteWebApi.Dto;

namespace IgniteWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoPointController : ControllerBase
    {
        ICache<string, GeoPoint> cacheGeoPoint;
        IIgnite ignite;
        GeoPointService pointService;

        public GeoPointController()
        {
            this.ignite = Ignition.GetIgnite();
            this.ignite.SetActive(true);
            this.cacheGeoPoint = ignite.GetOrCreateCache<string, GeoPoint>(FirstValues.GeoPointCacheName);
            this.pointService = new GeoPointService(cacheGeoPoint);
        }

        [HttpPost]
        [Route("Create")]
        public ActionResult Create([FromBody] GeoPointDto request)
        {
            request.CreateDate = DateTime.UtcNow;
            this.pointService.Create(request);
            return new ObjectResult(true);
        }

        [HttpGet()]
        [Route("Get")]
        public ActionResult<string> Get(string key)
        {
            var result = this.pointService.Get(key);
            return new ObjectResult(result);
        }
    }
}
