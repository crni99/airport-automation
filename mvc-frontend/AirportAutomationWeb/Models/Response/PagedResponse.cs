namespace AirportAutomation.Web.Models.Response
{
	public class PagedResponse<T>
	{
		public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
		{
			Data = data;
			PageNumber = pageNumber;
			PageSize = pageSize;
			LastPage = totalCount;
			TotalCount = totalCount;
			CalculatePageCount();
		}

		public IEnumerable<T> Data { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int LastPage { get; set; }
		public int TotalCount { get; set; }
		public int TotalPages { get; private set; }

		private void CalculatePageCount()
		{
			TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
		}
	}
}
