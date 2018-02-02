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
        private RoleManager<ApplicationUserRole> _roleManager;

        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider service)
        {
            if (!context.Roles.Any())
            {
                _roleManager = service.GetRequiredService<RoleManager<ApplicationUserRole>>();
                var role = new ApplicationUserRole()
                {
                    Name = "Administrators",
                    NormalizedName = "Administrators"
                };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("初始默认角色失败："+result.Errors.Select(e=>e.Description));
                }
            }

            if (!context.Users.Any())
            {
                _userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
             
                
                var defaultUser = new ApplicationUser()
                {
                    Email = "13228526@qq.com",
                    UserName = "Administrator",
                    NormalizedUserName = "admin",
                    SecurityStamp="admin",
                    Avatar= "https://chocolatey.org/content/packageimages/aspnetcore-runtimepackagestore.2.0.0.png"
                };
                var result = await _userManager.CreateAsync(defaultUser, "123456");
                await _userManager.AddToRoleAsync(defaultUser, "Administrators");
    
         
                if (!result.Succeeded)
                {
                    throw new Exception("初始默认用户失败");
                }
            }
        }
    }
}
