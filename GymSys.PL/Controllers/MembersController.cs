using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.MemberViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymSys.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        //Index() - Displays member listing page
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        //MemberDetails(int id) - Displays member profile page
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);

            if (member is null)
            {
                TempData["FailMessage"] = "Member not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }
        //HealthRecordDetails(int id) - Shows health record page
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecordDetails = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (healthRecordDetails == null)
            {
                TempData["FailMessage"] = "Health Record not found!";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecordDetails);
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

            if (result)
                TempData["SuccessMessage"] = "Member created successfully!";
            else
                TempData["FailMessage"] = "Failed to create member!";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        //GET Edit(id) - Displays edit form
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if(member == null)
            {
                TempData["FailMessage"] = "Member not found!";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        //POST Edit(member) - Submits form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member updated successfully!";
            else
                TempData["FailMessage"] = "Failed to update member!";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        //Delete(int id) - Shows deletion confirmation page
        //DeleteConfirmed(int id) - Processes deletion 
        #endregion
    }
}
