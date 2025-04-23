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
    public class CustomBaseController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>([FromQuery] PaginationDTO pagination,
            Expression<Func<TEntidad, object>> orderParm)
            where TEntidad : class
        {
            IQueryable<TEntidad> queryable = context.Set<TEntidad>().AsQueryable();

            await HttpContext.InsertPaginationParametersOnHeader(queryable);
            return await queryable
                .OrderBy(orderParm)
                .Paginate(pagination)
                .Paginate(pagination)
                .ProjectTo<TDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
