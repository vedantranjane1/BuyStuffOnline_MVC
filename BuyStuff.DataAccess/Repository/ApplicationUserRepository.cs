using BuyStuffOnline.DataAccess.Data;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuyStuffOnline.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationDbContext _categoryRepo;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _categoryRepo = db;
        }

        public void Save()
        {
            _categoryRepo.SaveChanges();
        }

    }
}
