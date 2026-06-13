using GymSys.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default);
        Task<UpdateTrainerViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct = default);
        Task<bool> DeleteTrainerAsync(int id, CancellationToken ct);
    }
}
