
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataModel;

public static class DbContextConfigurator
{
    public static void ConfigureDbContext(IServiceCollection services, ConnectionStringProvider connectionStringProvider)
    {
        services.AddDbContext<PosContext>(options =>
            options.UseMySql(connectionStringProvider.GetConnectionString(), new MySqlServerVersion(new Version(8, 1, 0))));

    }
}