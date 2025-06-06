﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyStuffOnline.Models.ViewModels
{
    public class ProductVM
    {
        public Product objProduct { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Category { get; set; }
    }
}
