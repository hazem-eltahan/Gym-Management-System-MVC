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
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default);
        Task<bool> ToggleActivationAsync(int id, CancellationToken ct = default);
        Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
