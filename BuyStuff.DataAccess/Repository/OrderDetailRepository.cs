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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository 
    {
        public ApplicationDbContext _orderDetailRepo;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _orderDetailRepo = db;
        }

        public void Save()
        {
            _orderDetailRepo.SaveChanges();
        }

        public void Update(OrderDetail obj)
        {
            _orderDetailRepo.orderDetails.Update(obj);
        }
    }
}
