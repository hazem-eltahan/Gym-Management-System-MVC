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
    public class TrainerRepository : ITrainerRepository
    {
        private readonly GymDbContext dbContext;
        public TrainerRepository(GymDbContext dbContext){
            this.dbContext = dbContext;
        }
        public async Task<int> AddAsync(Trainer trainer, CancellationToken ct = default)
        {
            dbContext.Trainers.Add(trainer);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(Trainer trainer, CancellationToken ct = default)
        {
            dbContext.Trainers.Remove(trainer);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Trainer>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<Trainer> query = tracking ? dbContext.Trainers : dbContext.Trainers.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<Trainer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await dbContext.Trainers.FindAsync(id, ct);
        }

        public async Task<int> UpdateAsync(Trainer trainer, CancellationToken ct = default)
        {
            dbContext.Trainers.Update(trainer);
            return await dbContext.SaveChangesAsync(ct);
        }
    }
}
