namespace Users.WebAPI.Pagination
{
    public class PaginationParameters
    {
        private int pageSize = 10;
        private const int MaximumPageSize = 50;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value >= PaginationParameters.MaximumPageSize)
                ? PaginationParameters.MaximumPageSize
                : value;
        }
    }
}