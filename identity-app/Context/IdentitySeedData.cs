using identity_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace identity_app.Context
{
	public class IdentitySeedData
	{
	
		private const string adminUser = "Admin";
		
		private const string adminPassword = "Admin_123";

		public static async void IdentityTestUser(IApplicationBuilder app)
		{
			var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

			if (context.Database.GetAppliedMigrations().Any())
			{
				context.Database.Migrate();
			}

			var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();

			var user = await userManager.FindByNameAsync(adminUser);

			if (user == null)
			{
				user = new AppUser
				{
					UserName = adminUser,
					FullName = "Şevval Yıldırım",
					Email = "admin@sevval.com",
					PhoneNumber = "444444"
				};

				await userManager.CreateAsync(user, adminPassword); //kullanıcı olustururken parola atansın mı
			}
		}
	}
}
