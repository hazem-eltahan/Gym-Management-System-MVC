using GymSys.BLL.Common;
using GymSys.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct = default);
        Task<Result<PlanViewModel?>> GetPlanDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<Result<UpdatePlanViewModel?>> GetPlanToUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> ToggleActivationAsync(int id, CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
