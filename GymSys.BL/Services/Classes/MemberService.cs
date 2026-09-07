using AutoMapper;
using GymSys.BLL.Common;
using GymSys.BLL.Services.Attachment;
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
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);

            if (emailExist || phoneExist) return Result.NotFound("Member with this data already exists!");

            //Upload Photo
            var storedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), "MembersPhotos", model.PhotoFile.FileName, ct);
            if (string.IsNullOrWhiteSpace(storedPhotoName.value)) return Result.Fail("Uploaded photo invalid!");


            var member = _mapper.Map<Member>(model);
            member.Photo = storedPhotoName.value;
            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result > 0)
            {
                return Result.OK();
            }
            else
            {
                //Delete uploaded photo
                _attachmentService.Delete("MembersPhotos", storedPhotoName.value);
                return Result.Fail("Failed to create member!");
            }
        }

        public async Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return Result<IEnumerable<MemberViewModel>>.NotFound("Members not found!");

            var mappedMembers = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return Result<IEnumerable<MemberViewModel>>.OK(mappedMembers);
        }

        public async Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.Id == id, ct: ct);
            if (healthRecord == null) return Result<HealthRecordViewModel?>.NotFound("Health record not found!");

            var mappedHealthRecord = _mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord);
            return Result<HealthRecordViewModel?>.OK(mappedHealthRecord);
        }

        public async Task<Result<MemberDetailsViewModel?>> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result<MemberDetailsViewModel?>.NotFound("Member not found!");

            var mappedMember = _mapper.Map<Member, MemberDetailsViewModel>(member);

            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(x => x.MemberId == id && x.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);
                if (activePlan is not null)
                {
                    mappedMember.PlanName = activePlan.Name;
                    mappedMember.MembershipStartDate = activeMembership.CreatedAt.ToString();
                    mappedMember.MembershipEndDate = activeMembership.EndDate.ToString();
                }
            }
            return Result<MemberDetailsViewModel?>.OK(mappedMember);
        }

        public async Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result<MemberToUpdateViewModel?>.NotFound("Member not found!");

            var mappedMember = _mapper.Map<Member, MemberToUpdateViewModel>(member);
            return Result<MemberToUpdateViewModel?>.OK(mappedMember);
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found!");

            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(e => e.Email == model.Email && e.Id != id);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(e => e.Phone == model.Phone && e.Id != id);

            if (emailExist || phoneExist) return Result.Fail("Member with this data already exists!");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to update member!");
        }

        public async Task<Result> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found!");

            var existingBooking = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct);
            if (existingBooking) return Result.Validation("Can not delete member with existing bookings!");

            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            _attachmentService.Delete("MembersPhotos", member.Phone);
            return result > 0 ? Result.OK() : Result.Fail("Failed to delete member!");
        }
    }
}
