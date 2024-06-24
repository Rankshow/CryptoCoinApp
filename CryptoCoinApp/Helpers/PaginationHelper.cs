using CryptoCoinApp.Common;

namespace CryptoCoinApp.Helpers
{
    public static class PaginationHelper
    {
        public static PaginatedResponse<T> Paginate<T>(
            IEnumerable<T> data, 
            int pageNumber, 
            int pageSize) where T : class
        {
            var totalCount = data.Count();
            var pagedData = data
                .OrderBy(x => x.GetType().GetProperty("id")?.GetValue(x, null))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResponse<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = pagedData
            };
        }
    }
}
