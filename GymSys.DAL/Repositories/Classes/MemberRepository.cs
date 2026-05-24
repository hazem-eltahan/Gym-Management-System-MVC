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
    public class MemberRepository : IMemberRepository
    {
        private readonly GymDbContext dbContext;
        public MemberRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<int> AddAsync(Member member, CancellationToken ct = default)
        {
            dbContext.Members.Add(member);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(Member member, CancellationToken ct = default)
        {
            dbContext.Members.Remove(member);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Member>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<Member> query = tracking ? dbContext.Members : dbContext.Members.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<Member?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await dbContext.Members.FindAsync(id, ct);
        }

        public async Task<int> UpdateAsync(Member member, CancellationToken ct = default)
        {
            dbContext.Members.Update(member);
            return await dbContext.SaveChangesAsync(ct);
        }
    }
}
