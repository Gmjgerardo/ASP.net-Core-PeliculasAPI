namespace PeliculasAPI.DTOs
{
    public class MovieFilterDTO
    {
        public int Page { get; set; }
        public int RowsPerPage { get; set; }
        internal PaginationDTO Pagination
        {
            get
            {
                return new PaginationDTO { Page = Page, RowsPerPage = RowsPerPage };
            }
        }

        public string? Title { get; set; }
        public int GenreId { get; set; }
        public bool OnCinemas { get; set; }
        public bool ComingSoon { get; set; }
    }
}
