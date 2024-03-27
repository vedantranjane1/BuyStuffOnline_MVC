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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _companyRepository;
        public CompanyRepository(ApplicationDbContext db): base(db)
        {
            _companyRepository = db;
        }

        public void Save()
        {
            _companyRepository.SaveChanges();
        }

        public void Update(Company obj)
        {
            _companyRepository.Companies.Update(obj);
        }

    }
}
