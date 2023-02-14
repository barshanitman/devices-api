using Microsoft.EntityFrameworkCore;

namespace Vnext.Function.Entities
{

    public class DeviceContext : DbContext
    {


        public DeviceContext(DbContextOptions<DeviceContext> options) : base(options) { }


        public DbSet<Devices> Devices { get; set; }


    }

}



