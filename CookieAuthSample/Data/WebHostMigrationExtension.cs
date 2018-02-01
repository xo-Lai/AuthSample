using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieAuthSample.Data
{
    public static class WebHostMigrationExtension
    {

        public  static IWebHost MigrationDbContext<TContext>(this IWebHost webHost,Action<TContext,IServiceProvider> sedder) 
            where TContext : DbContext
        {
            using (var scope=webHost.Services.CreateScope())
            {
                var serices = scope.ServiceProvider;
                var logger = serices.GetRequiredService<ILogger<TContext>>();
                var context = serices.GetService<TContext>();
                try
                {
                    context.Database.Migrate();
                    sedder(context, serices);
                    logger.LogInformation("执行成功", $"执行DbConext{typeof(TContext).Name} 成功");
                }
                catch (Exception ex)
                {

                    logger.LogError(ex, $"执行DbConext{typeof(TContext).Name} seed失败");
                }
            }
            return webHost;
        }
    }
}
