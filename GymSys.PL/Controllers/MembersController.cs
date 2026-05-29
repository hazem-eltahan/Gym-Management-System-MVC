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

        ////MemberDetails(int id) - Displays member profile page
        //public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        //{
        //    var member = await _membersRepository.GetByIdAsync(id, ct);

        //    return View(member);
        //}
        ////HealthRecordDetails(int id) - Shows health record page
        //public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        //{
        //    var healthRecordDetails = await _membersRepository.GetByIdAsync(id, ct);

        //    return View(member);
        //}

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
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        //MemberEdit(int id) - Displays edit form
        //MemberEdit() - Processes update 
        #endregion

        #region Delete
        //Delete(int id) - Shows deletion confirmation page
        //DeleteConfirmed(int id) - Processes deletion 
        #endregion
    }
}
