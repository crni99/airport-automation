namespace AirportAutomation.Application.Dtos.Response
{
	public class PagedResponse<T>
	{
		public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
		{
			Data = data;
			PageNumber = pageNumber;
			PageSize = pageSize;
			TotalCount = totalCount;
			CalculatePageCount();
			CalculateLastPage();
		}

		public IEnumerable<T> Data { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public int TotalPages { get; private set; }
		public int LastPage { get; private set; }

		private void CalculatePageCount()
		{
			TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
		}

		private void CalculateLastPage()
		{
			LastPage = TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;
		}
	}
}
