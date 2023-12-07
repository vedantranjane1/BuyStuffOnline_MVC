using BuyStuff.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace BuyStuff.Data
{
    public class ApplicationDbContext : DbContext
    {



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }
}
