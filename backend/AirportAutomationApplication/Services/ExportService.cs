using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AirportAutomation.Application.Services
{
	public class ExportService : IExportService
	{
		public byte[] ExportToPDF<T>(string name, IList<T> data)
		{
			var doc = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(1, Unit.Centimetre);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Header().Height(100).AlignCenter().AlignMiddle().Text(text =>
					{
						text.DefaultTextStyle(x => x.FontSize(14));
						text.Span(name);
					});

					page.Content().Row(row =>
					{
						switch (name)
						{
							case "Airlines" when typeof(T) == typeof(AirlineEntity):
								SetAirlinesRows(row, data.Cast<AirlineEntity>().ToList());
								break;
							case "Destinations" when typeof(T) == typeof(DestinationEntity):
								SetDestinationsRows(row, data.Cast<DestinationEntity>().ToList());
								break;
							case "Passengers" when typeof(T) == typeof(PassengerEntity):
								SetPassengersRows(row, data.Cast<PassengerEntity>().ToList());
								break;
							case "Pilots" when typeof(T) == typeof(PilotEntity):
								SetPilotsRows(row, data.Cast<PilotEntity>().ToList());
								break;
							case "Travel Classes" when typeof(T) == typeof(TravelClassEntity):
								SetTravelClassesRows(row, data.Cast<TravelClassEntity>().ToList());
								break;
							case "Flights" when typeof(T) == typeof(FlightEntity):
								SetFlightsRows(row, data.Cast<FlightEntity>().ToList());
								break;
							case "Plane Tickets" when typeof(T) == typeof(PlaneTicketEntity):
								SetPlaneTicketsRows(row, data.Cast<PlaneTicketEntity>().ToList());
								break;
							default:
								throw new ArgumentException("Unsupported name or type.");
						}
					});

					page.Footer().Height(30).AlignCenter().AlignMiddle().Text(text =>
					{
						text.DefaultTextStyle(x => x.FontSize(14));
						text.Span("AirportAutomationApi - 2024");
					});
				});
			});

			byte[] pdfBytes = doc.GeneratePdf();
			return pdfBytes;
		}

		private static void SetAirlinesRows(RowDescriptor row, List<AirlineEntity> airlines)
		{
			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("ID").Bold();
				foreach (var airline in airlines)
				{
					column.Item().Text(airline.Id.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(3).Column(column =>
			{
				column.Item().Text("Name").Bold();
				foreach (var airline in airlines)
				{
					column.Item().Text(airline.Name).LineHeight(2);
				}
			});
		}

		private static void SetDestinationsRows(RowDescriptor row, List<DestinationEntity> destinations)
		{
			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("ID").Bold();
				foreach (var destination in destinations)
				{
					column.Item().Text(destination.Id.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("City").Bold();
				foreach (var destination in destinations)
				{
					column.Item().Text(destination.City).LineHeight(2);
				}
			});

			row.RelativeItem(3).Column(column =>
			{
				column.Item().Text("Airport").Bold();
				foreach (var destination in destinations)
				{
					column.Item().Text(destination.Airport).LineHeight(2);
				}
			});
		}

		private static void SetPassengersRows(RowDescriptor row, List<PassengerEntity> passengers)
		{
			row.Spacing(20);

			row.RelativeItem(2).Column(column =>
			{
				foreach (var passenger in passengers)
				{
					column.Item().Text(text =>
					{
						text.Span("ID: ").Bold().LineHeight(2);
						text.Span(passenger.Id.ToString()).LineHeight(2);
					});

					column.Item().Text(text =>
					{
						text.Span("First name: ").Bold().LineHeight(2);
						text.Span(passenger.FirstName).LineHeight(2);
					});

					column.Item().Text(text =>
					{
						text.Span("Last name: ").Bold().LineHeight(2);
						text.Span(passenger.LastName).LineHeight(2);
					});

					column.Item().Text(text =>
					{
						text.Span("UPRN: ").Bold().LineHeight(2);
						text.Span(passenger.UPRN).LineHeight(2);
						text.Span("\n").LineHeight(2);
					});
				}
			});

			row.RelativeItem(3).Column(column =>
			{
				foreach (var passenger in passengers)
				{
					column.Item().Text(text =>
					{
						text.Span("\n").LineHeight(2);
						text.Span("Passport: ").Bold().LineHeight(2);
						text.Span(passenger.Passport).LineHeight(2);
					});

					column.Item().Text(text =>
					{
						text.Span("Address: ").Bold().LineHeight(2);
						text.Span(passenger.Address).LineHeight(2);
					});

					column.Item().Text(text =>
					{
						text.Span("Phone: ").Bold().LineHeight(2);
						text.Span(passenger.Phone).LineHeight(2);
						text.Span("\n").LineHeight(2);
					});
				}
			});
		}

		private static void SetPilotsRows(RowDescriptor row, List<PilotEntity> pilots)
		{
			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("ID").Bold();
				foreach (var pilot in pilots)
				{
					column.Item().Text(pilot.Id.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("First Name").Bold();
				foreach (var pilot in pilots)
				{
					column.Item().Text(pilot.FirstName).LineHeight(2);
				}
			});

			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("Last Name").Bold();
				foreach (var pilot in pilots)
				{
					column.Item().Text(pilot.LastName).LineHeight(2);
				}
			});

			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("UPRN").Bold();
				foreach (var pilot in pilots)
				{
					column.Item().Text(pilot.UPRN).LineHeight(2);
				}
			});

			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("Flying Hours").Bold();
				foreach (var pilot in pilots)
				{
					column.Item().Text(pilot.FlyingHours).LineHeight(2);
				}
			});
		}

		private static void SetTravelClassesRows(RowDescriptor row, List<TravelClassEntity> travelClasses)
		{
			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("ID").Bold();
				foreach (var travelClass in travelClasses)
				{
					column.Item().Text(travelClass.Id.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(3).Column(column =>
			{
				column.Item().Text("Name").Bold();
				foreach (var travelClass in travelClasses)
				{
					column.Item().Text(travelClass.Type).LineHeight(2);
				}
			});
		}

		private static void SetFlightsRows(RowDescriptor row, List<FlightEntity> flights)
		{
			row.RelativeItem(2).Column(column =>
			{
				column.Item().Text("ID").Bold();
				foreach (var flight in flights)
				{
					column.Item().Text(flight.Id.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(3).Column(column =>
			{
				column.Item().Text("Departure Date").Bold();
				foreach (var flight in flights)
				{
					column.Item().Text(flight.DepartureDate.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(3).Column(column =>
			{
				column.Item().Text("Departure Time").Bold();
				foreach (var flight in flights)
				{
					column.Item().Text(flight.DepartureTime.ToString()).LineHeight(2);
				}
			});
		}

		private static void SetPlaneTicketsRows(RowDescriptor row, List<PlaneTicketEntity> planeTickets)
		{
			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("ID").Bold();
				foreach (var planeTicket in planeTickets)
				{
					column.Item().Text(planeTicket.Id.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("Price").Bold();
				foreach (var planeTicket in planeTickets)
				{
					column.Item().Text(planeTicket.Price.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("Purchase Date").Bold();
				foreach (var planeTicket in planeTickets)
				{
					column.Item().Text(planeTicket.PurchaseDate.ToString()).LineHeight(2);
				}
			});

			row.RelativeItem(1).Column(column =>
			{
				column.Item().Text("Seat Number").Bold();
				foreach (var planeTicket in planeTickets)
				{
					column.Item().Text(planeTicket.SeatNumber.ToString()).LineHeight(2);
				}
			});
		}

	}
}
