﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController:  ControllerBase
    {
        private readonly IOutputCacheStore outputCacheStore;
        private readonly ApplicationDBContext context;
        private const string cacheTag = "genres";

        public GenresController(IOutputCacheStore outputCacheStore, ApplicationDBContext context)
        {
            this.outputCacheStore = outputCacheStore;
            this.context = context;
        }

        [HttpGet]
        [HttpGet("all")]
        [OutputCache(Tags = [cacheTag])]
        public List<Genre> Get()
        {
            return new List<Genre>() {
                new Genre { Id = 1, Name = "Comedia"},
                new Genre { Id = 2, Name = "Acción"} };
        }

        [HttpGet("{id:int}", Name = "obtainById")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Genre>> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
            return CreatedAtRoute("obtainById", new {id = genre.Id}, genre);
        }

        [HttpPut]
        public void Put()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
