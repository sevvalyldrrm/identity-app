using identity_app.Models;
using identity_app.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity_app.Controllers
{
	public class AccountController : Controller
	{

		private UserManager<AppUser> _userManager;
		private RoleManager<AppRole> _roleManager;
		private SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					await _signInManager.SignOutAsync(); //ilk olarak cookie silindi daha önce giriş yapmıssa diye

					var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

					if (result.Succeeded) { 
						await _userManager.ResetAccessFailedCountAsync(user); //user'ın bilgisi sıfırlandı 
						await _userManager.SetLockoutEndDateAsync(user, null); //süreyi sıfırlar - null veya geçmiş bir tarih verilebilir

						return RedirectToAction("Index", "Home");
					}
					else if (result.IsLockedOut)
					{
						var lockoutDate = await _userManager.GetLockoutEndDateAsync(user); 

						var timeLeft = lockoutDate.Value - DateTime.UtcNow; //5dk bilgisi verilecek

						ModelState.AddModelError("", $"Hesabınız kitlendi, lütfen {timeLeft.Minutes + 1} dakika sonra deneyiniz");
					}
					else
					{
						ModelState.AddModelError("", "parola hatalı");
					}

				}
                else
                {
					ModelState.AddModelError("", "hatalı email");
                }
            }
			return View(model);
		}

	}
}
