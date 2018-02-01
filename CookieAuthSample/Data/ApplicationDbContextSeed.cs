using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using CookieAuthSample.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CookieAuthSample.Data
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManager;

        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider service)
        {
            if (!context.Users.Any())
            {
                _userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                var identityUser = new ApplicationUser()
                {
                    Email = "13228526@qq.com",
                    UserName = "Administrator",
                    NormalizedUserName = "admin"
                };
                var result = await _userManager.CreateAsync(identityUser, "123456");
                if (!result.Succeeded)
                {
                    throw new Exception("初始用户失败");
                }
            }
        }
    }
}
