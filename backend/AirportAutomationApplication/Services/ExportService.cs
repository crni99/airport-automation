using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Reflection;

namespace AirportAutomation.Application.Services
{
	public class ExportService : IExportService
	{
		private const float CELL_PADDING = 6f;

		#region Excel Export
		public byte[] ExportToExcel<T>(string name, IList<T> data)
		{
			using var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add($"{name} Report");

			if (data == null || data.Count == 0)
				throw new ArgumentException("No data to export.");

			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.PropertyType.IsPrimitive
						 || p.PropertyType.IsValueType
						 || p.PropertyType == typeof(string))
				.ToArray();

			for (int col = 0; col < properties.Length; col++)
			{
				var cell = worksheet.Cell(1, col + 1);
				cell.Value = properties[col].Name;
				cell.Style.Font.Bold = true;
			}

			var headerRange = worksheet.Range(1, 1, 1, properties.Length);
			headerRange.SetAutoFilter();

			worksheet.Row(1).Height += 2;

			for (int row = 0; row < data.Count; row++)
			{
				int rowNumber = row + 2;

				for (int col = 0; col < properties.Length; col++)
				{
					var value = properties[col].GetValue(data[row]);
					worksheet.Cell(rowNumber, col + 1).Value = value?.ToString() ?? string.Empty;
				}
				worksheet.Row(rowNumber).Height += 2;
			}

			for (int col = 1; col <= properties.Length; col++)
			{
				worksheet.Column(col).AdjustToContents();
				worksheet.Column(col).Width += 1;
			}

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			return stream.ToArray();
		}
		#endregion

		#region PDF Export
		public byte[] ExportToPDF<T>(string name, IList<T> data)
		{
			var doc = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(1, Unit.Centimetre);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Content().Column(column =>
					{
						switch (name)
						{
							case "Airlines" when typeof(T) == typeof(AirlineEntity):
								AddCustomHeader(column, "Airlines Report");
								SetAirlinesRows(column, data.Cast<AirlineEntity>().ToList());
								break;

							case "Destinations" when typeof(T) == typeof(DestinationEntity):
								AddCustomHeader(column, "Destinations Report");
								SetDestinationsRows(column, data.Cast<DestinationEntity>().ToList());
								break;

							case "Passengers" when typeof(T) == typeof(PassengerEntity):
								AddCustomHeader(column, "Passengers Report");
								SetPassengersRows(column, data.Cast<PassengerEntity>().ToList());
								break;

							case "Pilots" when typeof(T) == typeof(PilotEntity):
								AddCustomHeader(column, "Pilots Report");
								SetPilotsRows(column, data.Cast<PilotEntity>().ToList());
								break;

							case "Travel Classes" when typeof(T) == typeof(TravelClassEntity):
								AddCustomHeader(column, "Travel Classes Report");
								SetTravelClassesRows(column, data.Cast<TravelClassEntity>().ToList());
								break;

							case "Flights" when typeof(T) == typeof(FlightEntity):
								AddCustomHeader(column, "Flights Report");
								SetFlightsRows(column, data.Cast<FlightEntity>().ToList());
								break;

							case "Plane Tickets" when typeof(T) == typeof(PlaneTicketEntity):
								AddCustomHeader(column, "Plane Tickets Report");
								SetPlaneTicketsRows(column, data.Cast<PlaneTicketEntity>().ToList());
								break;

							default:
								throw new ArgumentException("Unsupported name or type.");
						}
					});

					page.Footer()
						.BorderTop(1)
						.BorderColor(Colors.Grey.Lighten2)
						.PaddingTop(5)
						.PaddingHorizontal(10)
						.Row(row =>
						{
							row.RelativeItem().AlignLeft().Text(text =>
							{
								text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken1));
								text.Span($"AirportAutomationApi © {DateTime.Now.Year}");
							});

							row.RelativeItem().AlignRight().Text(text =>
							{
								text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken1));
								text.CurrentPageNumber();
								text.Span(" / ");
								text.TotalPages();
							});
						});
				});
			});

			byte[] pdfBytes = doc.GeneratePdf();
			return pdfBytes;
		}

		private static void AddCustomHeader(ColumnDescriptor column, string headerText)
		{
			column.Item()
				.Padding(10)
				.AlignCenter()
				.Text(text =>
				{
					text.DefaultTextStyle(x => x.FontColor(Colors.Black).FontSize(20).Bold());
					text.Span(headerText);
				});

			column.Item().Height(20);
		}

		private static void SetAirlinesRows(ColumnDescriptor column, List<AirlineEntity> airlines)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(50);
					columns.RelativeColumn();
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Name").Bold();
				});

				for (int i = 0; i < airlines.Count; i++)
				{
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(airlines[i].Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(airlines[i].Name);
				}
			});
		}

		private static void SetDestinationsRows(ColumnDescriptor column, List<DestinationEntity> destinations)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(50);
					columns.RelativeColumn();
					columns.RelativeColumn();
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("City").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Airport").Bold();
				});

				for (int i = 0; i < destinations.Count; i++)
				{
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(destinations[i].Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(destinations[i].City);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(destinations[i].Airport);
				}
			});
		}

		private static void SetPassengersRows(ColumnDescriptor column, List<PassengerEntity> passengers)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(40);
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("First Name").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Last Name").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("UPRN").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Passport").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Address").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Phone").Bold();
				});

				for (int i = 0; i < passengers.Count; i++)
				{
					var p = passengers[i];
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.FirstName);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.LastName);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.UPRN);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.Passport);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.Address);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(p.Phone);
				}
			});
		}

		private static void SetPilotsRows(ColumnDescriptor column, List<PilotEntity> pilots)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(40);
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.ConstantColumn(70);
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("First Name").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Last Name").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("UPRN").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Flying Hours").Bold();
				});

				for (int i = 0; i < pilots.Count; i++)
				{
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(pilots[i].Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(pilots[i].FirstName);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(pilots[i].LastName);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(pilots[i].UPRN);
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(pilots[i].FlyingHours.ToString());
				}
			});
		}

		private static void SetTravelClassesRows(ColumnDescriptor column, List<TravelClassEntity> travelClasses)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(50);
					columns.RelativeColumn();
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Name").Bold();
				});

				for (int i = 0; i < travelClasses.Count; i++)
				{
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(travelClasses[i].Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(travelClasses[i].Type);
				}
			});
		}

		private static void SetFlightsRows(ColumnDescriptor column, List<FlightEntity> flights)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(40);
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Departure Date").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Departure Time").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Airline").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Destination").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Pilot").Bold();
				});

				for (int i = 0; i < flights.Count; i++)
				{
					var flight = flights[i];
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

					table.Cell().Background(bg).Padding(CELL_PADDING).Text(flight.Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(flight.DepartureDate.ToString("yyyy-MM-dd"));
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(flight.DepartureTime.ToString("HH:mm"));
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(flight.Airline?.Name ?? "N/A");
					table.Cell().Background(bg).Padding(CELL_PADDING)
						.Text($"{flight.Destination?.City ?? "N/A"} ({flight.Destination?.Airport ?? "N/A"})");
					table.Cell().Background(bg).Padding(CELL_PADDING).Text($"{flight.Pilot?.FirstName} {flight.Pilot?.LastName}".Trim());
				}
			});
		}

		private static void SetPlaneTicketsRows(ColumnDescriptor column, List<PlaneTicketEntity> planeTickets)
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(40);
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
					columns.RelativeColumn();
				});

				table.Header(header =>
				{
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("ID").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Price").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Purchase Date").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Seat Number").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Passenger").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Travel Class").Bold();
					header.Cell().Background(Colors.Grey.Lighten3).Padding(CELL_PADDING).Text("Flight ID").Bold();
				});

				for (int i = 0; i < planeTickets.Count; i++)
				{
					var ticket = planeTickets[i];
					var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

					table.Cell().Background(bg).Padding(CELL_PADDING).Text(ticket.Id.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(ticket.Price.ToString("C2"));
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(ticket.PurchaseDate.ToString("yyyy-MM-dd"));
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(ticket.SeatNumber.ToString());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text($"{ticket.Passenger?.FirstName} {ticket.Passenger?.LastName}".Trim());
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(ticket.TravelClass?.Type ?? "N/A");
					table.Cell().Background(bg).Padding(CELL_PADDING).Text(ticket.Flight?.Id.ToString() ?? "N/A");
				}
			});
		}
		#endregion

	}
}
