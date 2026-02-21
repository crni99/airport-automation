using AirportAutomation.Application.Services;
using AirportAutomation.Core.Entities;
using ClosedXML.Excel;
// using QuestPDF.Infrastructure;

namespace AirportAutomationApi.Test.Services
{
	public class ExportServiceTests
	{
		#region Excel Export Tests

		[Fact]
		[Trait("Category", "ExportService")]
		public void ExportToExcel_ValidData_ReturnsByteArray()
		{
			// Arrange
			var testData = new List<TestEntity>
			{
				new TestEntity { Id = 1, Name = "Item A", IsActive = true },
				new TestEntity { Id = 2, Name = "Item B", IsActive = false }
			};
			var exportService = new ExportService();

			// Act
			var result = exportService.ExportToExcel("TestSheet", testData);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportService")]
		public void ExportToExcel_NullData_ThrowsArgumentException()
		{
			// Arrange
			IList<TestEntity> nullData = null;
			var exportService = new ExportService();

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => exportService.ExportToExcel("TestSheet", nullData));
			Assert.Equal("No data to export.", exception.Message);
		}

		[Fact]
		[Trait("Category", "ExportService")]
		public void ExportToExcel_EmptyData_ThrowsArgumentException()
		{
			// Arrange
			var emptyData = new List<TestEntity>();
			var exportService = new ExportService();

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => exportService.ExportToExcel("TestSheet", emptyData));
			Assert.Equal("No data to export.", exception.Message);
		}

		[Fact]
		[Trait("Category", "ExportService")]
		public void ExportToExcel_ContentVerification_HeadersAndDataAreCorrect()
		{
			// Arrange
			var testData = new List<TestEntity>
			{
				new TestEntity { Id = 1, Name = "Item A", IsActive = true }
			};
			var exportService = new ExportService();

			// Act
			var excelBytes = exportService.ExportToExcel("TestSheet", testData);
			using var stream = new MemoryStream(excelBytes);
			using var workbook = new XLWorkbook(stream);
			var worksheet = workbook.Worksheet(1);

			// Assert Headers
			Assert.Equal("Id", worksheet.Cell("A1").Value);
			Assert.Equal("Name", worksheet.Cell("B1").Value);
			Assert.Equal("IsActive", worksheet.Cell("C1").Value);

			// Assert Data
			Assert.Equal("1", worksheet.Cell("A2").Value);
			Assert.Equal("Item A", worksheet.Cell("B2").Value);
			Assert.Equal("True", worksheet.Cell("C2").Value);
		}

		#endregion

		#region PDF Export Tests
		/*
		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_AirlinesCase_ReturnsByteArray()
		{
			// Arrange
			var airlines = new List<AirlineEntity>
			{
				new AirlineEntity { Id = 1, Name = "FlyHigh Airlines" },
				new AirlineEntity { Id = 2, Name = "Oceanic Airways" }
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Airlines", airlines);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_DestinationsCase_ReturnsByteArray()
		{
			// Arrange
			var destinations = new List<DestinationEntity>
			{
				new DestinationEntity { Id = 1, City = "New York", Airport = "JFK" },
				new DestinationEntity { Id = 2, City = "London", Airport = "LHR" }
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Destinations", destinations);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_PassengersCase_ReturnsByteArray()
		{
			// Arrange
			var passengers = new List<PassengerEntity>
			{
				new PassengerEntity { Id = 1, FirstName = "Jane", LastName = "Doe", UPRN = "U123", Passport = "P456", Address = "123 Main St", Phone = "555-1234" }
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Passengers", passengers);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_PilotsCase_ReturnsByteArray()
		{
			// Arrange
			var pilots = new List<PilotEntity>
			{
				new PilotEntity { Id = 1, FirstName = "John", LastName = "Smith", UPRN = "UPRN-1", FlyingHours = 5000 }
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Pilots", pilots);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_TravelClassesCase_ReturnsByteArray()
		{
			// Arrange
			var travelClasses = new List<TravelClassEntity>
			{
				new TravelClassEntity { Id = 1, Type = "Economy" },
				new TravelClassEntity { Id = 2, Type = "Business" }
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Travel Classes", travelClasses);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_FlightsCase_ReturnsByteArray()
		{
			// Arrange
			var flights = new List<FlightEntity>
			{
				new FlightEntity
				{
					Id = 1,
					DepartureDate = new DateOnly(2025, 10, 27),
					DepartureTime = new TimeOnly(14, 30, 0),
					Airline = new AirlineEntity { Name = "Global Air" },
					Destination = new DestinationEntity { City = "Paris", Airport = "CDG" },
					Pilot = new PilotEntity { FirstName = "Sarah", LastName = "Conner" }
				}
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Flights", flights);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportToPDF")]
		public void ExportToPDF_PlaneTicketsCase_ReturnsByteArray()
		{
			// Arrange
			var planeTickets = new List<PlaneTicketEntity>
			{
				new PlaneTicketEntity
				{
					Id = 1,
					Price = 150.75m,
					PurchaseDate = new DateOnly(2025, 9, 1),
					SeatNumber = 12,
					Passenger = new PassengerEntity { FirstName = "Tom", LastName = "Hanks" },
					TravelClass = new TravelClassEntity { Type = "Economy" },
					Flight = new FlightEntity { Id = 101 }
				}
			};
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act
			var result = exportService.ExportToPDF("Plane Tickets", planeTickets);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<byte[]>(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		[Trait("Category", "ExportService")]
		public void ExportToPDF_UnsupportedType_ThrowsArgumentException()
		{
			// Arrange
			var unsupportedData = new List<TestEntity> { new TestEntity() };
			var exportService = new ExportService();
			QuestPDF.Settings.License = LicenseType.Community;

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => exportService.ExportToPDF("Unsupported", unsupportedData));
			Assert.Equal("Unsupported name or type.", exception.Message);
		}
		*/

		#endregion

		public class TestEntity
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public bool IsActive { get; set; }
		}

	}
}
