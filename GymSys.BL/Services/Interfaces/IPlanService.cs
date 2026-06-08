using GymSys.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct);
    }
}
