using GymSys.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default);
        Task<int> GetCountOfBookedSlotsAsync(int id, CancellationToken ct = default);
    }
}
