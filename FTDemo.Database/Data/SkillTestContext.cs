using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FTDemo.Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTDemo.Database.Data
{
    public class SkillTestContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public SkillTestContext(DbContextOptions<SkillTestContext> options) : base(options)
        {

        }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<tblFileUpload> tblFileUpload { get; set; }
    }
}
