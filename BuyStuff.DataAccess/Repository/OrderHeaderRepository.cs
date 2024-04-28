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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository 
    {
        public ApplicationDbContext _orderHeaderRepo;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _orderHeaderRepo = db;
        }

        public void Save()
        {
            _orderHeaderRepo.SaveChanges();
        }

        public void Update(OrderHeader obj)
        {
            _orderHeaderRepo.orderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null) 
        { 
            var orderFromDb = _orderHeaderRepo.orderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb !=null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                { 
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string payementIntentId)
        {
			var orderFromDb = _orderHeaderRepo.orderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb!=null)
            {
                if (!string.IsNullOrEmpty(sessionId)) 
                {
                    orderFromDb.SessionId = sessionId;
                }
				if (!string.IsNullOrEmpty(payementIntentId))
				{
					orderFromDb.PaymentIntentId = payementIntentId;
                    orderFromDb.PaymentDate = DateTime.Now; 
				}
			}
        }
	}
}
