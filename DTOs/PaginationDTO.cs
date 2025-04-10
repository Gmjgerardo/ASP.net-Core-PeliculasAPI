namespace PeliculasAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; }

        private int rowsPerPage = 10;
        private readonly int maximumRowsPerPage = 50;

        public int RowsPerPage {
            get { return rowsPerPage; } 
            set
            {
                rowsPerPage = (value > maximumRowsPerPage ? maximumRowsPerPage : value);
            }
        }
    }
}
