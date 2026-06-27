using AutoMapper;
using GymSys.BLL.Common;
using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.TrainerViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone, ct);

            if (emailExist || phoneExist) return Result.Validation("Trainer with this data already exists!");

            var mappedTrainer = _mapper.Map<Trainer>(model);
           _unitOfWork.GetRepository<Trainer>().Add(mappedTrainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to create trainer!");
        }

        public async Task<Result> DeleteTrainerAsync(int id, CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result.NotFound("Trainer not found!");

            var activeSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s=>s.TrainerId == id && s.StartDate >  DateTime.Now, ct);
            if (activeSessions) return Result.Fail("Can't delete trainer with active session!");

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to delete trainer!");
        }

        public async Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (!trainers.Any()) return Result<IEnumerable<TrainerViewModel>>.NotFound("Trainers not found!");

            var mappedTrainers = _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
            return Result<IEnumerable<TrainerViewModel>>.OK(mappedTrainers);
        }

        public async Task<Result<TrainerDetailsViewModel?>> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result<TrainerDetailsViewModel?>.NotFound("Trainer not found!");

            var mappedTrainer = _mapper.Map<TrainerDetailsViewModel>(trainer);
            return Result<TrainerDetailsViewModel?>.OK(mappedTrainer);
        }

        public async Task<Result<UpdateTrainerViewModel?>> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result<UpdateTrainerViewModel?>.NotFound("Trainer not found!");

            var mappedTrainer = _mapper.Map<UpdateTrainerViewModel>(trainer);

            return Result<UpdateTrainerViewModel?>.OK(mappedTrainer);
        }

        public async Task<Result> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result.NotFound("Trainer not found!");

            var emailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email && x.Id != id, ct);
            var phoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct);

            if (emailExist || phoneExist) return Result.Fail("Trainer with this data exists!");

            _mapper.Map(model, trainer);
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to update trainer!");
        }
    }
}
