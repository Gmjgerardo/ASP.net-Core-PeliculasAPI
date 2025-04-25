using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
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

        public CustomBaseController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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
    }
}
