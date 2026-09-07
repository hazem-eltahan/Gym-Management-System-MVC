using GymSys.BLL.Common;
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
        Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<Result<TrainerDetailsViewModel?>> GetTrainerDetailsAsync(int id, CancellationToken ct = default);
        Task<Result<UpdateTrainerViewModel?>> GetTrainerToUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct = default);
        Task<Result> DeleteTrainerAsync(int id, CancellationToken ct);
    }
}
