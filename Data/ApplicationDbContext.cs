using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASPNETIdentityConfig.Models;

namespace ASPNETIdentityConfig.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ASPNETIdentityConfig.Models.ApplicationUser> ApplicationUser { get; set; }
    }
}
