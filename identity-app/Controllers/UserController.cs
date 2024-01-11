using identity_app.Models;
using identity_app.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity_app.Controllers
{
	public class UserController : Controller
	{
		private UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
		{
			return View(_userManager.Users);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateViewModel model)
		{
			if (ModelState.IsValid)
			{

				if (await _userManager.FindByEmailAsync(model.Email) != null)
				{
					ModelState.AddModelError("", "Bu e-posta adresi zaten kullanımda.");
					return View(model);
				}

				if (await _userManager.FindByNameAsync(model.UserName) != null)
				{
					ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanımda.");
					return View(model);
				}

				var user = new AppUser
				{
					UserName = model.UserName,
					Email = model.Email,
					FullName = model.FullName,
				};


				
				IdentityResult result = await _userManager.CreateAsync(user, model.Password);

				//kullanıcı basarılı bir sekilde olusturulmus ise yönlendirilecek
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}

				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}

			}
			return View(model);
		}


		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
			{
				return RedirectToAction(nameof(Index));
			}

			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return View(new EditViewModel());
			}

			return View(new EditViewModel
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email
			});
		}
	}
}
