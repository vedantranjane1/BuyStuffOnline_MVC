using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyStuffOnline.Models
{
    public class ShoppingCart
    {
        [Key]
        public int ID { get; set; }

        public int Count {  get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey(nameof(ApplicationUserID))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductID { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }

        [NotMapped]
        public decimal Price { get; set; }
    }
}
