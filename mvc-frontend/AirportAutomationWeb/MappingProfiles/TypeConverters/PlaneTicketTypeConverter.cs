using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.PlaneTicket;
using AutoMapper;
using System.Globalization;

namespace AirportAutomation.Web.MappingProfiles.TypeConverters
{
	public class PlaneTicketTypeConverter :
		ITypeConverter<PlaneTicketCreateViewModel, PlaneTicketEntity>,
		ITypeConverter<PlaneTicketViewModel, PlaneTicketEntity>
	{
		public PlaneTicketEntity Convert(PlaneTicketCreateViewModel source, PlaneTicketEntity destination, ResolutionContext context)
		{
			var planeTicket = new PlaneTicketEntity
			{
				SeatNumber = source.SeatNumber,
				PassengerId = (int)source.PassengerId,
				TravelClassId = (int)source.TravelClassId,
				FlightId = (int)source.FlightId
			};

			decimal priceDecimal;
			if (decimal.TryParse(source.Price, out priceDecimal))
			{
				planeTicket.Price = priceDecimal;
			}
			else
			{
				planeTicket.Price = new decimal(0);
			}

			if (DateOnly.TryParseExact(source.PurchaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly purchaseDate))
			{
				planeTicket.PurchaseDate = purchaseDate;
			}
			else
			{
				planeTicket.PurchaseDate = new DateOnly(2000, 1, 1);
			}
			return planeTicket;
		}

		public PlaneTicketEntity Convert(PlaneTicketViewModel source, PlaneTicketEntity destination, ResolutionContext context)
		{
			var planeTicket = new PlaneTicketEntity
			{
				Id = source.Id,
				Price = source.Price,
				SeatNumber = source.SeatNumber,
				PassengerId = source.PassengerId,
				TravelClassId = source.TravelClassId,
				FlightId = source.FlightId
			};

			if (DateOnly.TryParseExact(source.PurchaseDate, "dd.MM.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly purchaseDate))
			{
				planeTicket.PurchaseDate = purchaseDate;
			}
			else
			{
				planeTicket.PurchaseDate = new DateOnly(2000, 1, 1);
			}
			return planeTicket;
		}
	}
}