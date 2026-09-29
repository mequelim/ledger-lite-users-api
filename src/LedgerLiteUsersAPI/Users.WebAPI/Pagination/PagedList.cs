namespace Users.WebAPI.Pagination
{
    public class PagedList<T> : List<T> where T : class
    {
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }

        // Constructor
        public PagedList(int pageNumber, int pageSize, int count, List<T> items)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        // Methods:
        public bool HasPrevious => (CurrentPage > 1);

        public bool HasNext => (CurrentPage < TotalPages);

        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedList<T>(pageNumber, pageSize, count, items);
        }
    }
}