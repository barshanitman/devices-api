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
            Console.WriteLine(connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<DeviceContext>();

            optionsBuilder.UseSqlServer("Server=tcp:sg4oyrzzuggcy.database.windows.net,1433;Initial Catalog=DevicesDB;Persist Security Info=False;User ID=alohauser;Password=!Aloha27;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new DeviceContext(optionsBuilder.Options);
        }
    }

}