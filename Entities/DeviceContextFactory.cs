using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vnext.Function.Entities;


namespace Vnext.Function
{

    public class DeviceContextFactory : IDesignTimeDbContextFactory<DeviceContext>
    {

        public DeviceContext CreateDbContext(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var optionsBuilder = new DbContextOptionsBuilder<DeviceContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new DeviceContext(optionsBuilder.Options);
        }
    }

}