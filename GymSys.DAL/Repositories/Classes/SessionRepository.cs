using GymSys.DAL.Data.DbContexts;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default)
        {
           var query = _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);

            return await query.ToListAsync();
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int id, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.AsNoTracking().CountAsync(b=>b.SessionId == id);
        }

        public async Task<Session?> GetSessionByIdWithTrainerAndCategoryAsync(int id, CancellationToken ct = default)
        {
            return await _dbContext.Sessions.AsNoTracking().Include(x=>x.Trainer).Include(x=>x.Category).FirstOrDefaultAsync(x=>x.Id == id);
        }
    }
}
