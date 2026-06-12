using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.PlanViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            if (!plans.Any()) return [];

            var planViewModel = plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Duration = p.DurationDays,
                Description = p.Description,
                IsActive = p.IsActive
            });
            return planViewModel;
        }

        public async Task<PlanViewModel?> GetPlanDetailsByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return null;

            var planVM = new PlanViewModel()
            {
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };
            return planVM;
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null || !plan.IsActive) return null;

            if (await HasActiveMembershipsAsync(id, ct)) return null;

            var updatePlanVM = new UpdatePlanViewModel()
            {
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description
            };
            return updatePlanVM;
        }

        public async Task<bool> ToggleActivationAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return false;

            if (plan.IsActive && await HasActiveMembershipsAsync(id, ct)) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return false;

            if (await HasActiveMembershipsAsync(id,ct)) return false;

            plan.Price = model.Price;
            plan.DurationDays = model.Duration;
            plan.Description = model.Description;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        #region Helper Methds
        private async Task<bool> HasActiveMembershipsAsync(int id, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == id && m.EndDate > DateTime.Now, ct);
        }
        #endregion
    }
}
