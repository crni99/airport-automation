using AirportAutomation.Core.Converters;
using AirportAutomation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Data
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions options) : base(options) { }

		protected override void ConfigureConventions(ModelConfigurationBuilder builder)
		{
			builder.Properties<DateOnly>()
				.HaveConversion<DateOnlyConverter>();
			//	.HaveColumnType("date");

			builder.Properties<TimeOnly>()
				.HaveConversion<TimeOnlyConverter>();
			//	.HaveColumnType("time");

			base.ConfigureConventions(builder);
		}

		public DbSet<PassengerEntity> Passenger { get; set; }
		public DbSet<TravelClassEntity> TravelClass { get; set; }
		public DbSet<DestinationEntity> Destination { get; set; }
		public DbSet<PilotEntity> Pilot { get; set; }
		public DbSet<AirlineEntity> Airline { get; set; }
		public DbSet<FlightEntity> Flight { get; set; }
		public DbSet<PlaneTicketEntity> PlaneTicket { get; set; }
		public DbSet<ApiUserEntity> ApiUser { get; set; }
	}
}
