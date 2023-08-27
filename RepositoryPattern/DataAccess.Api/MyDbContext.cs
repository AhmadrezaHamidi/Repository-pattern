using DataAccess.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataAccess.Api
{

    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
        public MyDbContext()
        {

        }
    }
}
