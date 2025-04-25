using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Utilities;
using System.Linq.Expressions;

namespace PeliculasAPI.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly string cacheTag;

        public CustomBaseController(ApplicationDBContext context, IMapper mapper, IOutputCacheStore outputCacheStore, string cacheTag)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.cacheTag = cacheTag;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>([FromQuery] PaginationDTO pagination,
            Expression<Func<TEntity, object>> orderParm)
            where TEntity : class
        {
            IQueryable<TEntity> queryable = context.Set<TEntity>().AsQueryable();

            await HttpContext.InsertPaginationParametersOnHeader(queryable);
            return await queryable
                .OrderBy(orderParm)
                .Paginate(pagination)
                .Paginate(pagination)
                .ProjectTo<TDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id)
            where TEntity : class, IId
            where TDTO : IId
        {
            TDTO? entity = await context.Set<TEntity>()
                .ProjectTo<TDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            return (entity is not null) ? entity : NotFound();
        }
    
        protected async Task<IActionResult> Post<TEntity, TDTO, TCreationDTO>(TCreationDTO entityCreationDTO, string routeName)
            where TEntity : class, IId
        {
            TEntity entity = mapper.Map<TEntity>(entityCreationDTO);

            context.Add(entity);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            TDTO entityDTO = mapper.Map<TDTO>(entity);

            return CreatedAtRoute(routeName, new { id = entity.Id }, entityDTO);
        }

        protected async Task<IActionResult> Put<TEntity, TCreationDTO>(int id, TCreationDTO entityUpdate)
            where TEntity : class, IId
        {
            IActionResult response = NotFound();
            Boolean entityExist = await context.Set<TEntity>().AnyAsync(e => e.Id == id);

            if (entityExist is true)
            {
                TEntity entity = mapper.Map<TEntity>(entityUpdate);
                entity.Id = id;

                context.Update(entity);
                await context.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync(cacheTag, default);

                response = NoContent();
            }

            return response;
        }

        protected async Task<IActionResult> Delete<TEntity>(int id)
            where TEntity : class, IId
        {
            IActionResult response = NotFound();
            int deletedRows = await context.Set<TEntity>().Where(e => e.Id == id).ExecuteDeleteAsync();

            if (deletedRows > 0)
            {
                await outputCacheStore.EvictByTagAsync(cacheTag, default);
                response = NoContent();
            }

            return response;
        }
    }
}
