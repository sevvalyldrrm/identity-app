﻿using identity_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identity_app.Context
{
	public class IdentityContext : IdentityDbContext<AppUser, AppRole, string>
	{
        public IdentityContext(DbContextOptions<IdentityContext> options): base(options) { }
    }
}
