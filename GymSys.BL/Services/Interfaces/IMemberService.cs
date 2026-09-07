using GymSys.BLL.Common;
using GymSys.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);
        Task<Result<MemberDetailsViewModel?>> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);
        Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> DeleteMemberAsync(int id, CancellationToken ct = default);
    }
}
