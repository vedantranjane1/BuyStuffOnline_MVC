using Microsoft.AspNetCore.Mvc;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.DataAccess.Repository;
using BuyStuffOnline.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using BuyStuffOnline.Models.ViewModels;
using System.Runtime.Serialization.Formatters;

namespace BuyStuff.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository _db, ICategoryRepository CategoryRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = _db;
            _categoryRepo = CategoryRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
         {
            List<Product> objProductList = _productRepo.GetAll(includeProperties: "Category").ToList();
            GetAll();
            return View(objProductList);
        }


        public IActionResult UpSert(int? ID) //Update+Insert
        {
            IEnumerable<SelectListItem> CategoryList = _categoryRepo.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString()
            });

            ProductVM productVM = new()
            {
                objProduct = new Product(),
                Category = CategoryList,
            };

            if (ID != null && ID != 0)
            {
                productVM.objProduct = _productRepo.Get(x => x.ID == ID);
            }
            //ViewBag.CategoryList = CategoryList;
            return View(productVM);

        }
        [HttpPost]
        public IActionResult UpSert(ProductVM productVM, IFormFile? ImgFile)
        {
            if (ModelState.IsValid)
            {
                if (ImgFile != null)
                {
                    string ProductFileName = Guid.NewGuid() + Path.GetExtension(ImgFile.FileName);
                    //string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string FilePath = Path.Combine(_webHostEnvironment.WebRootPath, @"image\product");
                    FileStream ProductFile;

                    if (!string.IsNullOrEmpty(productVM.objProduct.ImageUrl))
                    {
                        string OldFile = Path.Combine(_webHostEnvironment.WebRootPath, productVM.objProduct.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(OldFile))
                            System.IO.File.Delete(OldFile);

                        TempData["success"] = "Product Updated Successfully";
                    }

                    using (ProductFile = new FileStream(Path.Combine(FilePath, ProductFileName), FileMode.Create))
                    {
                        ImgFile.CopyTo(ProductFile);
                        productVM.objProduct.ImageUrl = @"\image\product\" + ProductFileName;
                    }


                }

                if (productVM.objProduct.ID == 0)
                {
                    _productRepo.Add(productVM.objProduct);
                }
                else
                    _productRepo.Update(productVM.objProduct);
                //
                _productRepo.Save();

                return RedirectToAction("Index");
            }
            TempData["error"] = "Failed To Add Product";
            return View();
        }

        #region Edit

        //Edit region is not in use as we have combined Create and Edit pages.
        //The action for create and edit pages in UpSert()
        public IActionResult Edit(int ID)
        {
            if (ID <= 0)
            {
                return NotFound();
            }
            Product? _objProduct = _productRepo.Get(x => x.ID == ID);

            if (_objProduct == null)
                return NotFound();

            IEnumerable<SelectListItem> CategoryList = _categoryRepo.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString()
            });
            ViewBag.CategoryList = CategoryList;
            return View(_objProduct);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _productRepo.Update(obj);
                _productRepo.Save();
                TempData["success"] = "Product Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        #endregion


        //Delete Action is not being used as we are routing the request through DeleteConfirmation() which is present in product.js
        //DeleteConfirmation() uses SweetAlert
        public IActionResult Delete(int ID)
        {

            if (ID == 0)
            {
                return NotFound();
            }
            Product? _objProduct = _productRepo.Get(x => x.ID == ID);

            if (_objProduct == null)
                return NotFound();


            return View(_objProduct);
        }

        //[HttpPost, ActionName("Delete")]
        //This fucntion is called using Ajax call from the DeleteConfirmation() which is present in product.js
        public IActionResult DeletePost(int ID)
        {
            if (ID == 0)
            {
                return Json(new { success = false, message = "Error while deleting" });
                //return NotFound();
            }
            Product? _objProduct = _productRepo.Get(x => x.ID == ID);

            if (_objProduct == null) 
            {
                return Json(new { success = false, message =  "Error while deleting"});
            }

            if (_objProduct != null)
            {
                string OldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, _objProduct.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(OldImagePath)) 
                {
                    System.IO.File.Delete(OldImagePath);
                }

                _productRepo.Remove(_objProduct);
                _productRepo.Save();
                TempData["success"] = "Product Deleted Successfully";
            }
            return Json(new { success = true, message = "Product Deleted" });
            //return RedirectToAction("Index");
        }

        #region API
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Product> objProductList = _productRepo.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        #endregion
    }
}
