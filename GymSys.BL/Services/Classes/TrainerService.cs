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
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository,
            IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _trainerRepository.AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _trainerRepository.AnyAsync(x => x.Phone == model.Phone, ct);

            if (emailExist || phoneExist) return false;

            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address()
                {
                    Street = model.Street,
                    City = model.City,
                    BuildingNumber = model.BuildingNumber
                },
                Speciality = model.Specialties
            };
            var result = await _trainerRepository.AddAsync(trainer, ct);
            return result > 0;
        }

        public async Task<bool> DeleteTrainerAsync(int id, CancellationToken ct)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return false;

            var activeSessions = await _sessionRepository.AnyAsync(s=>s.TrainerId == id && s.StartDate >  DateTime.Now, ct);
            if (activeSessions) return false;

            var result = await _trainerRepository.DeleteAsync(trainer);
            return result > 0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);
            if (!trainers.Any()) return [];

            var trainersVM = trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialization = t.Speciality.ToString()
            });
            return trainersVM;
        }

        public async Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return null;

            var trainerVM = new TrainerDetailsViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}",
                Specialization = trainer.Speciality.ToString()
            };
            return trainerVM;
        }

        public async Task<UpdateTrainerViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return null;

            var trainerVM = new UpdateTrainerViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                Speciality = trainer.Speciality
            };

            return trainerVM;
        }

        public async Task<bool> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return false;

            var emailExist = await _trainerRepository.AnyAsync(x => x.Email == model.Email && x.Id != id, ct);
            var phoneExist = await _trainerRepository.AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct);

            if(emailExist || phoneExist) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Speciality = model.Speciality;
            trainer.UpdatedAt = DateTime.Now;

            var result = await _trainerRepository.UpdateAsync(trainer,ct);
            return result > 0;
        }
    }
}
