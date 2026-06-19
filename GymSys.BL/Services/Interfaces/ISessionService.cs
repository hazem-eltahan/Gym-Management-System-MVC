using GymSys.BLL.Common;
using GymSys.BLL.Services.Classes;
using GymSys.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectList>> GetTrainerSelectListAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectList>> GetCategorySelectListAsync(CancellationToken ct = default);
        Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default);
        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
        Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default);
    }
}
