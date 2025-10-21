using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class ApiUserRepository : IApiUserRepository
	{
		protected readonly DatabaseContext _context;

		public ApiUserRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<ApiUserEntity>> GetApiUsers(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _context.ApiUser
				.OrderBy(c => c.ApiUserId)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<ApiUserEntity?> GetApiUser(int id)
		{
			return await _context.ApiUser.FindAsync(id);
		}

		public async Task<IList<ApiUserEntity?>> GetApiUsersByRole(CancellationToken cancellationToken, int page, int pageSize, string role)
		{
			return await _context.ApiUser
				.Where(a => a.Roles == role)
				.OrderBy(c => c.ApiUserId)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<IList<ApiUserEntity?>> GetApiUsersByFilter(
			CancellationToken cancellationToken,
			int page,
			int pageSize,
			ApiUserSearchFilter filter)
		{
			IQueryable<ApiUserEntity> query = _context.ApiUser.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(filter.UserName))
			{
				var lowerCaseUserName = filter.UserName.ToLower();
				query = query.Where(a => a.UserName.ToLower().Contains(lowerCaseUserName));
			}
			if (!string.IsNullOrWhiteSpace(filter.Roles))
			{
				query = query.Where(a => a.Roles == filter.Roles);
			}
			return await query.OrderBy(a => a.ApiUserId)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}

		public async Task PutApiUser(ApiUserEntity apiUser)
		{
			_context.Entry(apiUser).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<bool> DeleteApiUser(int id)
		{
			var apiUser = await GetApiUser(id);
			_context.ApiUser.Remove(apiUser);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ApiUserExists(int id)
		{
			return await _context.ApiUser.AsNoTracking().AnyAsync(e => e.ApiUserId == id);
		}

		public async Task<int> ApiUsersCount(CancellationToken cancellationToken, string? role = null)
		{
			IQueryable<ApiUserEntity> query = _context.ApiUser.AsNoTracking();
			if (!string.IsNullOrEmpty(role))
			{
				query = query.Where(a => a.Roles.Contains(role));
			}
			return await query.CountAsync(cancellationToken).ConfigureAwait(false);
		}

		public async Task<int> ApiUsersCountFilter(
			CancellationToken cancellationToken,
			ApiUserSearchFilter filter)
		{
			IQueryable<ApiUserEntity> query = _context.ApiUser.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(filter.UserName))
			{
				var lowerCaseUserName = filter.UserName.ToLower();
				query = query.Where(a => a.UserName.ToLower().Contains(lowerCaseUserName));
			}
			if (!string.IsNullOrWhiteSpace(filter.Roles))
			{
				query = query.Where(a => a.Roles == filter.Roles);
			}
			return await query.CountAsync(cancellationToken).ConfigureAwait(false);
		}

	}
}
