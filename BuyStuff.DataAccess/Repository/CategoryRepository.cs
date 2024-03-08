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
    public class CategoryRepository : Repository<Category>, ICategoryRepository 
    {
        public ApplicationDbContext _categoryRepo;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _categoryRepo = db;
        }

        public void Save()
        {
            _categoryRepo.SaveChanges();
        }

        public void Update(Category obj)
        {
            _categoryRepo.Categories.Update(obj);
        }
    }
}
