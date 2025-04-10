using PeliculasAPI.DTOs;

namespace PeliculasAPI.Utilities
{
    public static class IQueryableExtensions
    {
       public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RowsPerPage)
                .Take(pagination.RowsPerPage);
        }
    }
}
