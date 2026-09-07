using GymSys.BLL.Services.Attachment;
using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.MemberViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymSys.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        #region Get Member Photo
        [HttpGet]
        public async Task<IActionResult> Picture(int id, CancellationToken ct)
        {
            var memberResult = await _memberService.GetMemberDetailsByIdAsync(id, ct);

            if (!memberResult.success)
            {
                TempData["FailMessage"] = memberResult.error;
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(memberResult.value!.Photo))
                return NotFound();

            var fileResult = _attachmentService.GetFile("MembersPhotos", memberResult.value.Photo);

            if (!fileResult.success)
            {
                TempData["FailMessage"] = fileResult.error;
                return NotFound();
            }

            return File(fileResult.value!.Value.stream, fileResult.value.Value.contentType);
        } 
        #endregion

        //Index() - Displays member listing page
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _memberService.GetAllMembersAsync(ct);
            return View(result.value);
        }

        //MemberDetails(int id) - Displays member profile page
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);

            if (!result.success)
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

            return View(result.value);
        }
        //HealthRecordDetails(int id) - Shows health record page
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (!result.success)
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        #region Create
        //Create() - Shows member registration form
        [HttpGet]
        public IActionResult Create() => View();

        //CreateMember() - Processes form submission 

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);

            if (result.success)
                TempData["SuccessMessage"] = "Member created successfully!";
            else
                TempData["FailMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        //GET Edit(id) - Displays edit form
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (!result.success)
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        //POST Edit(member) - Submits form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member updated successfully!";
            else
                TempData["FailMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        //GET Delete(int id) - Shows deletion confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //POST DeleteConfirmed(int id) - Processes deletion 
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);

            if (result.success)
                TempData["SuccessMessage"] = "Member deleted successfully!";
            else
                TempData["FailMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
