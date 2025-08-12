using AirportAutomation.Core.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Core.Filters
{
	public class PlaneTicketSearchFilter
	{
		public decimal? Price { get; set; }

		[ModelBinder(BinderType = typeof(DateOnlyModelBinder))]
		public DateOnly? PurchaseDate { get; set; }
		public int? SeatNumber { get; set; }
	}
}
