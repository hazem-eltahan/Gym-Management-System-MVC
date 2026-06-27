using GymSys.BLL.Common;
using GymSys.BLL.ViewModels.AnalyticsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<Result<int>> GetCountOfMembersAsync(CancellationToken ct = default);
        Task<Result<int>> GetCountOfActiveMembersAsync(CancellationToken ct = default);
        Task<Result<int>> GetCountOfTrainersAsync(CancellationToken ct = default);
        Task<Result<int>> GetCountOfUpcomingSessionsAsync(CancellationToken ct = default);
        Task<Result<int>> GetCountOfOngoingSessionsAsync(CancellationToken ct = default);
        Task<Result<int>> GetCountOfCompletedSessionsAsync(CancellationToken ct = default);
        Task<Result<AnalyticsViewModel>> GetStatsAsync(CancellationToken ct = default);
    }
}
