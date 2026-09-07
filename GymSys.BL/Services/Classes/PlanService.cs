using AutoMapper;
using GymSys.BLL.Common;
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
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            if (!plans.Any()) return Result<IEnumerable<PlanViewModel>>.NotFound("No plans found!");

            var mappedPlans = _mapper.Map<IEnumerable<PlanViewModel>>(plans);
            return Result<IEnumerable<PlanViewModel>>.OK(mappedPlans);
        }

        public async Task<Result<PlanViewModel?>> GetPlanDetailsByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result<PlanViewModel?>.NotFound("Plan not found!");

            var mappedPlan = _mapper.Map<PlanViewModel>(plan);
            return Result<PlanViewModel?>.OK(mappedPlan);
        }

        public async Task<Result<UpdatePlanViewModel?>> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result<UpdatePlanViewModel?>.NotFound("Plan not found!");
            if (!plan.IsActive) return Result<UpdatePlanViewModel?>.Validation("Cannot update an inactive plan!");

            if (await HasActiveMembershipsAsync(id, ct)) return Result<UpdatePlanViewModel?>.Validation("Cannot update a plan with active memberships!");

            var mappedPlan = _mapper.Map<UpdatePlanViewModel>(plan);
            return Result<UpdatePlanViewModel?>.OK(mappedPlan);
        }

        public async Task<Result> ToggleActivationAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result.NotFound("Plan not found!");

            if (plan.IsActive && await HasActiveMembershipsAsync(id, ct)) return Result.Validation("Cannot deactivate a plan with active memberships!");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to change plan status!");
        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result.NotFound("Plan not found!");

            if (await HasActiveMembershipsAsync(id,ct)) return Result.Validation("Cannot update a plan with active memberships!");

            _mapper.Map(model, plan);
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to update plan!");
        }

        #region Helper Methds
        private async Task<bool> HasActiveMembershipsAsync(int id, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == id && m.EndDate > DateTime.Now, ct);
        }
        #endregion
    }
}
