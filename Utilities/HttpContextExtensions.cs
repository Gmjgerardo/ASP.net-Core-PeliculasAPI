using Microsoft.EntityFrameworkCore;

namespace PeliculasAPI.Utilities
{
    public static class HttpContextExtensions
    {
        public static async Task InsertPaginationParametersOnHeader<T> (this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Append("total-records-count", count.ToString());
        }
    }
}
