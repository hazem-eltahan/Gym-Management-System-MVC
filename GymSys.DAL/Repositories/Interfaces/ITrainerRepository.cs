using GymSys.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Repositories.Interfaces
{
    public interface ITrainerRepository
    {
        Task<IEnumerable<Trainer>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        Task<Trainer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(Trainer trainer, CancellationToken ct = default);
        Task<int> UpdateAsync(Trainer trainer, CancellationToken ct = default);
        Task<int> DeleteAsync(Trainer trainer, CancellationToken ct = default);
    }
}
