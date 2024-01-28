using identity_app.Models;
using identity_app.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace identity_app.Controllers
{
	[Authorize(Roles = "admin")]
	public class UserController : Controller
	{
		private UserManager<AppUser> _userManager;
		private RoleManager<AppRole> _roleManager;

		public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		//[AllowAnonymous -> bir kimliği olmadan giriş yapabilir. (hepsni kapat(auth) bunu aç demek istedik)
		public IActionResult Index()
		{
			//if (!User.IsInRole("admin"))
			//{
			//	return RedirectToAction("Login", "Account");
			//}
			return View(_userManager.Users);
		}



		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
			{
				return RedirectToAction(nameof(Index));
			}

			var user = await _userManager.FindByIdAsync(id);

			if (user != null)
			{
				ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();

				return View(new EditViewModel
				{
					Id = user.Id,
					FullName = user.FullName,
					UserName = user.UserName,
					Email = user.Email,
					SelectedRoles = await _userManager.GetRolesAsync(user)
				});
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> Edit(string id, EditViewModel model)
		{
			ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();

			if (id != model.Id)
			{
				return RedirectToAction(nameof(Index));
			}

			if (model.Password != null)
			{

				if (ModelState.IsValid)
				{
					var user = await _userManager.FindByIdAsync(model.Id);

					if (user != null)
					{
						user.Email = model.Email;
						user.FullName = model.FullName;
						user.UserName = model.UserName;
					}

					var result = await _userManager.UpdateAsync(user);


					if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
					{
						await _userManager.AddPasswordAsync(user, model.Password);
					}

					if (result.Succeeded)
					{
						await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
						if (model.SelectedRoles != null)
						{
							await _userManager.AddToRolesAsync(user, model.SelectedRoles);
						}
						return RedirectToAction("Index");
					}

					foreach (IdentityError err in result.Errors)
					{
						ModelState.AddModelError("", err.Description);
					}
				}
				else
				{
					ModelState.AddModelError("", "Kullanıcı bulunamadı.");
				}

				return View(model);
			}
			if (model.Password == null)
			{
				ModelState.Remove("Password");
				ModelState.Remove("ConfirmPassword");


				if (ModelState.IsValid)
				{
					var user = await _userManager.FindByIdAsync(model.Id);
					if (user != null)
					{
						user.Email = model.Email;
						user.FullName = model.FullName;
						user.UserName = model.UserName;

						var result = await _userManager.UpdateAsync(user);
						if (result.Succeeded)
						{
							await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
							if (model.SelectedRoles != null)
							{
								await _userManager.AddToRolesAsync(user, model.SelectedRoles);
							}

							return RedirectToAction("Index");
						}
						foreach (IdentityError err in result.Errors)
						{
							ModelState.AddModelError("", err.Description);
						}
					}
				}
				return View(model);

			}
			return RedirectToAction(nameof(Index));

		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
