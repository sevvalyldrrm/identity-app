using identity_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity_app.Controllers
{
	public class RoleController : Controller
	{
		private RoleManager<AppRole> _roleManager;

        public RoleController(RoleManager<AppRole> roleManager)
        {
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
	}
}
