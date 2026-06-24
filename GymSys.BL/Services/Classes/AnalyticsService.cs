using AutoMapper.Execution;
using GymSys.BLL.Common;
using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.AnalyticsViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Classes;
using GymSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Member = GymSys.DAL.Data.Models.Member;

namespace GymSys.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<int>> GetCountOfMembersAsync(CancellationToken ct = default)
        {
            var countOfMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct);
            return Result<int>.OK(countOfMembers);
        }
        public async Task<Result<int>> GetCountOfActiveMembersAsync(CancellationToken ct = default)
        {
            var countOfActiveMembers = await _unitOfWork.GetRepository<Member>().CountAsync(m => m.Memberships.Any(ms => ms.EndDate > DateTime.Now), ct);
            return Result<int>.OK(countOfActiveMembers);
        }

        public async Task<Result<int>> GetCountOfTrainersAsync(CancellationToken ct = default)
        {
            var countOfTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct);
            return Result<int>.OK(countOfTrainers);
        }

        public async Task<Result<int>> GetCountOfCompletedSessionsAsync(CancellationToken ct = default)
        {
            var countOfCompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s=>s.EndDate < DateTime.Now, ct);
            return Result<int>.OK(countOfCompletedSessions);
        }


        public async Task<Result<int>> GetCountOfOngoingSessionsAsync(CancellationToken ct = default)
        {
            var countOfOngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now, ct);
            return Result<int>.OK(countOfOngoingSessions);
        }

        public async Task<Result<int>> GetCountOfUpcomingSessionsAsync(CancellationToken ct = default)
        {
            var countOfUpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > DateTime.Now, ct);
            return Result<int>.OK(countOfUpcomingSessions);
        }

        public async Task<Result<AnalyticsViewModel>> GetStatsAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var countOfMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct);
            var countOfActiveMembers = await _unitOfWork.GetRepository<Member>().CountAsync(m => m.Memberships.Any(ms => ms.EndDate > now), ct);
            var countOfTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct);
            var countOfCompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate < now, ct);
            var countOfOngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= now && s.EndDate >= now, ct);
            var countOfUpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > now, ct);

            var analyticsVM = new AnalyticsViewModel()
            {
                TotalMembers = countOfMembers,
                ActiveMembers = countOfActiveMembers,
                Trainers = countOfTrainers,
                CompletedSessions = countOfCompletedSessions,
                UpcomingSessions = countOfUpcomingSessions,
                OngoingSessions = countOfOngoingSessions,
            };
            return Result<AnalyticsViewModel>.OK(analyticsVM);
        }

        
    }
}
