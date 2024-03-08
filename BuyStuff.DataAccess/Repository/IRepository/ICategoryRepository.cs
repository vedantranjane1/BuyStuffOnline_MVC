using BuyStuffOnline.DataAccess.Data;
using BuyStuffOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyStuffOnline.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {        
        void Update(Category obj);
        void Save(); 
    }
}
