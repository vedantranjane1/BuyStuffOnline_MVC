using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BuyStuffOnline.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }       

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }

        [Required]
        [Display(Name="List Price")]
        [Range(1,1000)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ListPrice{ get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 1000)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Price50 { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

        [ValidateNever]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        [ValidateNever]
        public Category Category { get; set; }
    }

}
