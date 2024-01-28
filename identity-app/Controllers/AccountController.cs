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
        private IEMailSender _mailSender;
        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IEMailSender mailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mailSender = mailSender;
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

                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabını onaylayınız.");
                        return View(model);
                    }


                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                    if (result.Succeeded)
                    {
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
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

                    //email
                    await _mailSender.SendEmailAsync(user.Email, "Hesap Onayı", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:7061{url}'>tıklayınız</a>");

                    TempData["message"] = "Email hesabınızdaki onay linkine tıklayınız";
                    return RedirectToAction("Login", "Account");
                }

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if (Id == null || token == null)
            {
                TempData["message"] = "Geçersiz token bilgisi";
                return View();
            }

            //bilgiler varsa user alındı
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                //kullanıcıyı token bilgisiyle validate etme
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    TempData["message"] = "Hesabınız onaylandı";
                    return RedirectToAction("Login", "Account");
                }
            }

            TempData["message"] = "Kullanıcı bulunamadı";
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }


		public IActionResult ForgotPassword()
		{
            return View();
		}

        [HttpPost]
		public async Task<IActionResult> ForgotPassword(string Email)
		{
            if (string.IsNullOrEmpty(Email))
            {
				TempData["message"] = "E-posta adresinizi giriniz.";
				return View();
            }

            var user = await _userManager.FindByEmailAsync(Email); 

            if(user == null)
            {
				TempData["message"] = "E-posta adresiyle eşleşen bir kayıt yok.";
				return View();
			}

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var url = Url.Action("ResetPassword", "Account", new { user.Id, token });

            await _mailSender.SendEmailAsync(Email, "Parola Sıfırlama", $"Parolanızı yenilemek için linke <a href='https://localhost:7061{url}'>tıklayınız</a>");

            TempData["message"] = "E-posta adresinize gönderilen link ile şifrenizi sıfırlayabilirsiniz.";
            return View();
		}


		public IActionResult ResetPassword(string Id, string token)
		{
			if (Id == null || token == null)
			{
				return RedirectToAction("Login");
			}
			var model = new ResetPasswordModel
			{
				Token = token
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					TempData["message"] = "Bu mail adresiyle eslesen kullanici yok";
					return RedirectToAction("Login");
				}
				var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
				if (result.Succeeded)
				{
					TempData["message"] = "Sifreniz degistirildi";
					return RedirectToAction("Login");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);

		}
	}
}
