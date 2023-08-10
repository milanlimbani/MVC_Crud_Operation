using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saturday.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saturday.Data
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {

        }
        public DbSet<AddBus> addBuses { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<AddImages> AddImages { get; set; }
    }
}
