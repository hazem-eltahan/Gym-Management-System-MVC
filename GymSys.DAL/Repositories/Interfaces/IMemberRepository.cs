using GymSys.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Repositories.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        Task<Member?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(Member member, CancellationToken ct = default);
        Task<int> UpdateAsync(Member member, CancellationToken ct = default);
        Task<int> DeleteAsync(Member member, CancellationToken ct = default);
    }
}
