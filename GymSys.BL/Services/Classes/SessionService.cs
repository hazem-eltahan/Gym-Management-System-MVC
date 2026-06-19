using AutoMapper;
using GymSys.BLL.Common;
using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.SessionViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Data.Models.Enums;
using GymSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.StartDate >= model.EndDate) return Result.Validation("End date must be after start date!");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start date must be in the future!");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity has to be between 1 and 25!");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null) return Result.NotFound("Trainer not found!");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category == null) return Result.NotFound("Category not found!");

            var isValid = Enum.TryParse<Speciality>(category.CategoryName, true, out var speciality);
            if (!isValid) return Result.Validation("Invalid category!");

            if (speciality != trainer.Speciality) return Result.Validation("Trainer does not have this speciality!");

            var sessionVM = _mapper.Map<CreateSessionViewModel, Session>(model);
            _unitOfWork.GetRepository<Session>().Add(sessionVM);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to create session!");
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);
            if (sessions == null || !sessions.Any()) return null;

            var sessionVM = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Description = s.Description,
                Capacity = s.Capacity,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                TrainerName = s.Trainer.Name,
                CategoryName = s.Category.CategoryName,
            });

            foreach (var session in sessionVM)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }

            return sessionVM;
        }

        public async Task<IEnumerable<CategorySelectList>> GetCategorySelectListAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectList>>(categories);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategoryAsync(id, ct);
            if (session == null)
                return Result<SessionViewModel>.NotFound("Session not found!");
            else
            {
                var sessionVM = _mapper.Map<Session, SessionViewModel>(session);
                sessionVM.AvailableSlots = sessionVM.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
                return Result<SessionViewModel>.OK(sessionVM);
            }
        }
        public async Task<IEnumerable<TrainerSelectList>> GetTrainerSelectListAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectList>>(trainers);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound("Session not found!");

            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Can not update a session that has already started!");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Can not update a session with booked slots!");

            var sessionVM = _mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.OK(sessionVM);
        }


        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session == null) return Result.NotFound("Session not found!");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can not update a started session!");

            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after start date!");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0)
                return Result.Fail("Can not update a session with booked slots!");

            if(model.StartDate <= DateTime.Now) 
                return Result.Validation("Start date must be in the future!");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null) return Result.NotFound("Trainer not found!");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
            if (category == null) return Result.NotFound("Category not found!");

            var isValid = Enum.TryParse<Speciality>(category.CategoryName, true, out var speciality);
            if (!isValid) return Result.Validation("Invalid category!");

            if (speciality != trainer.Speciality) return Result.Validation("Trainer does not have this speciality!");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to update session!");
        }
    }
}
