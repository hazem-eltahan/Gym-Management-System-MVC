using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.MemberViewModels;
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
        private readonly IGenericRepository<Plan> _planRepository;

        public PlanService(IGenericRepository<Plan> planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _planRepository.GetAllAsync(ct: ct);
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
    }
}
