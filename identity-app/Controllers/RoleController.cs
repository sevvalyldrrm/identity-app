using identity_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity_app.Controllers
{
	public class RoleController : Controller
	{
		private UserManager<AppUser> _userManager;
		private RoleManager<AppRole> _roleManager;

		public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public IActionResult Index()
		{
			return View(_roleManager.Roles);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(AppRole model)
		{
			if (ModelState.IsValid)
			{
				var result = await _roleManager.CreateAsync(model);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}

				foreach (var err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}

			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);

			if (role != null && role.Name != null)
			{
				ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
				return View(role);
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Edit(AppRole model)
		{
			if (ModelState.IsValid)
			{
				var role = await _roleManager.FindByIdAsync(model.Id);

				if (role != null)
				{
					role.Name = model.Name;

					var result = await _roleManager.UpdateAsync(role);

					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}

					foreach(var err in result.Errors)
					{
						ModelState.AddModelError("", err.Description);
					}

					if (role.Name != null)
					{
						ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
					}
				}
			}
			return View(model);
		}
	}
}
