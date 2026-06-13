using AutoMapper;
using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.MemberViewModels;
using GymSys.DAL.Data.DbContexts;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);

            if (emailExist || phoneExist) return false;

            var member = _mapper.Map<Member>(model);
            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            var membersVM = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>> (members);
            return membersVM;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.Id == id, ct: ct);
            if (healthRecord == null) return null;

            var healthRecordVM = _mapper.Map<HealthRecord, HealthRecordViewModel> (healthRecord);
            return healthRecordVM;
        }

        public async Task<MemberDetailsViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;

            var memberDetailsVM = _mapper.Map<Member, MemberDetailsViewModel>(member);

            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(x => x.MemberId == id && x.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);
                memberDetailsVM.PlanName = activePlan.Name;
                memberDetailsVM.MembershipStartDate = activeMembership.CreatedAt.ToString();
                memberDetailsVM.MembershipEndDate = activeMembership.EndDate.ToString();
            }
            return memberDetailsVM;
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;

            var memberToUpdateVM = _mapper.Map<Member, MemberToUpdateViewModel>(member);
            return memberToUpdateVM;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(id,ct);
            if (member == null) return false;

            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(e=>e.Email == model.Email && e.Id != id);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(e=>e.Phone == model.Phone && e.Id != id);

            if(emailExist || phoneExist) return false;

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return false;

            var existingBooking = await _unitOfWork.GetRepository<Booking>().AnyAsync(b=>b.MemberId == id && b.Session.StartDate > DateTime.Now, ct);
            if(existingBooking) return false;

            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
