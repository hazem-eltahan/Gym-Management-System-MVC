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

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);

            if (emailExist || phoneExist) return false;

            var member = new Member()
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
                    BuildingNumber = model.BuildingNumber,
                },
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note
                }
            };
            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            var membersVM = members.Select(m => new MemberViewModel()
            {
                Id = m.Id,
                Photo = m.Photo,
                Name = m.Name,
                Email = m.Email,
                Gender = m.Gender.ToString(),
                Phone = m.Phone
            });
            return membersVM;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.Id == id, ct: ct);
            if (healthRecord == null) return null;

            var healthRecordVM = new HealthRecordViewModel()
            {
                Weight = healthRecord.Weight,
                Height = healthRecord.Height,
                Note = healthRecord.Note,
                BloodType = healthRecord.BloodType
            };
            return healthRecordVM;
        }

        public async Task<MemberDetailsViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;

            var memberDetailsVM = new MemberDetailsViewModel()
            {
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Gender = member.Gender.ToString(),
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}"
            };

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

            var memberToUpdateVM = new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street,
            };
            return memberToUpdateVM;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(id,ct);
            if (member == null) return false;

            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(e=>e.Email == model.Email && e.Id != id);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(e=>e.Phone == model.Phone && e.Id != id);

            if(emailExist || phoneExist) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
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
