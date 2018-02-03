using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using CookieAuthSample.Data;
using Microsoft.EntityFrameworkCore;
using CookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using CookieAuthSample.Services;
using IdentityServer4.Services;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using System.Reflection;
using IdentityServer4.EntityFramework.Mappers;

namespace CookieAuthSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            const string connectionString = "Server=.;Database=aspnet-IdentityServer;User Id=sa;Password=sa;";
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
            
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddInMemoryClients(Config.GetClients())
                //.AddInMemoryApiResources(Config.GetApiResources())
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
              .AddConfigurationStore(options =>
              options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)))
              .AddOperationalStore(options =>
              options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)))
                .AddAspNetIdentity<ApplicationUser>()

              .Services.AddTransient<IProfileService, ProfileService>();




            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(option=>
            //    {
            //        option.LoginPath = "/Account/Login";
            //    });
            services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
            });

            services.AddScoped<ConsentService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();

            InitIdentityServerDatabase(app);
            //app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void InitIdentityServerDatabase(IApplicationBuilder app)
        {
            using (var scope=app.ApplicationServices.CreateScope())//获取所有的依赖
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var congifurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!congifurationDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        congifurationDbContext.Clients.Add(client.ToEntity());
                    }
                    congifurationDbContext.SaveChanges();
                }

                if (!congifurationDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        congifurationDbContext.ApiResources.Add(api.ToEntity());
                    }
                    congifurationDbContext.SaveChanges();
                }

                if (!congifurationDbContext.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        congifurationDbContext.IdentityResources.Add(identity.ToEntity());
                    }
                    congifurationDbContext.SaveChanges();
                }
            }
        }
    }
}
